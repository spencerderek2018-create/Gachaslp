using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Runtime representation of a hero during battle.
    /// Wraps the immutable HeroData ScriptableObject and tracks all mutable combat state:
    /// current HP, CR gauge, active debuffs, skill cooldowns, and stat modifiers.
    ///
    /// Rule: never mutate HeroData — all runtime state lives here.
    /// </summary>
    public class BattleUnit
    {
        public HeroData Data         { get; private set; }
        public bool IsPlayerTeam     { get; private set; }

        // --- Combat Readiness ---
        public float CombatReadiness { get; set; }

        // --- HP ---
        public int CurrentHP         { get; private set; }
        public bool IsDead           => CurrentHP <= 0;

        // --- Stat modifiers (equipment + buffs + debuffs) ---
        private readonly StatModifiers modifiers = new();

        // --- Active debuffs ---
        private readonly List<DebuffInstance> activeDebuffs = new();

        // --- Skill cooldowns ---
        private readonly Dictionary<SkillSlot, int> cooldowns = new();

        // --- Artifact ---
        public ArtifactData EquippedArtifact { get; private set; }

        // --- Events ---
        public event System.Action<int, int> OnHPChanged;   // (currentHP, maxHP)
        public event System.Action<DebuffInstance> OnDebuffApplied;
        public event System.Action<DebuffInstance> OnDebuffExpired;

        public BattleUnit(HeroData data, bool isPlayerTeam)
        {
            Data         = data;
            IsPlayerTeam = isPlayerTeam;
            EquippedArtifact = data.defaultArtifact;

            // Initialise cooldowns for all skill slots
            foreach (SkillSlot slot in System.Enum.GetValues(typeof(SkillSlot)))
                cooldowns[slot] = 0;

            CurrentHP = FinalHP;
            CombatReadiness = 0f;
        }

        // ----------------------------------------------------------------
        // Final stat computation (base * multiplier + flat)
        // ----------------------------------------------------------------

        public int FinalHP =>
            Mathf.RoundToInt(Data.baseHP * modifiers.GetMultiplier(StatType.HP)
                             + modifiers.GetFlat(StatType.HP));

        public int FinalATK =>
            Mathf.RoundToInt(Data.baseATK * modifiers.GetMultiplier(StatType.ATK)
                             + modifiers.GetFlat(StatType.ATK));

        public int FinalDEF =>
            Mathf.RoundToInt(Data.baseDEF * modifiers.GetMultiplier(StatType.DEF)
                             + modifiers.GetFlat(StatType.DEF));

        public int FinalSpeed =>
            Mathf.RoundToInt(Data.baseSpeed * modifiers.GetMultiplier(StatType.Speed)
                             + modifiers.GetFlat(StatType.Speed));

        public float FinalCritRate =>
            Mathf.Clamp01(Data.baseCritRate + modifiers.GetFlat(StatType.CritRate));

        public float FinalCritDamage =>
            Data.baseCritDamage + modifiers.GetFlat(StatType.CritDamage);

        public float FinalEffectiveness =>
            Mathf.Clamp01(Data.baseEffectiveness + modifiers.GetFlat(StatType.Effectiveness));

        public float FinalEffectResist =>
            Mathf.Clamp01(Data.baseEffectResist + modifiers.GetFlat(StatType.EffectResist));

        // ----------------------------------------------------------------
        // Damage & Healing
        // ----------------------------------------------------------------

        /// <summary>
        /// Applies damage to this unit. Non-DoT damage is reduced by DEF.
        /// DEF formula: damage * (1 - DEF / (DEF + 1000))
        /// </summary>
        public void TakeDamage(int rawAmount, bool isDot = false)
        {
            int amount = rawAmount;

            if (!isDot)
            {
                float defReduction = FinalDEF / (FinalDEF + 1000f);
                amount = Mathf.RoundToInt(amount * (1f - defReduction));
            }

            CurrentHP = Mathf.Max(0, CurrentHP - amount);
            OnHPChanged?.Invoke(CurrentHP, FinalHP);

            if (IsDead)
                Debug.Log($"[Battle] {Data.heroName} has been defeated.");
        }

        /// <summary>
        /// Heals this unit. Ignite debuff reduces healing by 50%.
        /// Healing cannot exceed max HP.
        /// </summary>
        public void Heal(int amount)
        {
            float healMult = HasDebuff(DebuffType.Ignite) ? 0.5f : 1f;
            int healed = Mathf.RoundToInt(amount * healMult);
            CurrentHP = Mathf.Min(FinalHP, CurrentHP + healed);
            OnHPChanged?.Invoke(CurrentHP, FinalHP);
        }

        // ----------------------------------------------------------------
        // Debuffs
        // ----------------------------------------------------------------

        /// <summary>
        /// Attempts to apply a debuff. Resolves effectiveness vs resistance, applies
        /// elemental bonus to landing chance, then adds to active debuffs if it lands.
        /// Returns true if the debuff was successfully applied.
        /// </summary>
        public bool TryApplyDebuff(DebuffInstance debuff, TurnManager turnManager = null)
        {
            // Immunity set (2pc): if unit has immunity buff, block one debuff and remove the immunity
            if (HasImmunityBuff)
            {
                ConsumeImmunityBuff();
                Debug.Log($"[Debuff] {Data.heroName}'s Immunity blocked {debuff.Type}!");
                return false;
            }

            // Land chance = source effectiveness - target resistance
            float landChance = debuff.Source.FinalEffectiveness - FinalEffectResist;

            // Elemental advantage on the debuff type adds +15% land chance
            ElementType debuffElement = DebuffCategory.GetDebuffElement(debuff.Type);
            if (ElementalAffinity.HasAdvantage(debuffElement, Data.element))
                landChance += ElementalAffinity.DebuffChanceBonus;

            landChance = Mathf.Clamp01(landChance);

            if (Random.value > landChance)
            {
                Debug.Log($"[Debuff] {debuff.Type} on {Data.heroName} resisted! (chance was {landChance:P0})");
                return false;
            }

            activeDebuffs.Add(debuff);
            ApplyStatModifiersFromDebuff(debuff);

            // Instant-effect debuffs (Chill) resolve immediately
            if (turnManager != null)
                debuff.ApplyInstantEffect(this, turnManager);

            Debug.Log($"[Debuff] {Data.heroName} afflicted with {debuff.Type} for {debuff.RemainingTurns} turns.");
            OnDebuffApplied?.Invoke(debuff);
            return true;
        }

        // ----------------------------------------------------------------
        // Turn lifecycle
        // ----------------------------------------------------------------

        /// <summary>
        /// Called at the start of this unit's turn.
        /// Ticks DoT debuffs, removes expired ones, decrements skill cooldowns.
        /// </summary>
        public void OnTurnStart()
        {
            // Tick and expire debuffs
            foreach (var debuff in activeDebuffs.ToList())
            {
                debuff.OnTurnStart(this);

                if (debuff.IsExpired)
                {
                    RemoveStatModifiersFromDebuff(debuff);
                    activeDebuffs.Remove(debuff);
                    OnDebuffExpired?.Invoke(debuff);
                    Debug.Log($"[Debuff] {debuff.Type} expired on {Data.heroName}.");
                }
            }

            // Decrement all skill cooldowns
            foreach (var slot in cooldowns.Keys.ToList())
                if (cooldowns[slot] > 0) cooldowns[slot]--;
        }

        // ----------------------------------------------------------------
        // Skill management
        // ----------------------------------------------------------------

        public bool IsSkillReady(SkillSlot slot) => cooldowns[slot] <= 0;

        public void SetCooldown(SkillSlot slot, int turns) => cooldowns[slot] = turns;

        public int GetCooldown(SkillSlot slot) => cooldowns[slot];

        // ----------------------------------------------------------------
        // Debuff queries
        // ----------------------------------------------------------------

        public bool HasDebuff(DebuffType type) =>
            activeDebuffs.Any(d => d.Type == type);

        public int GetStackCount(DebuffType type) =>
            activeDebuffs.Count(d => d.Type == type);

        public IReadOnlyList<DebuffInstance> ActiveDebuffs => activeDebuffs;

        // Convenience status checks
        public bool IsStunned  => HasDebuff(DebuffType.Stun) || HasDebuff(DebuffType.Freeze);
        public bool IsSilenced => HasDebuff(DebuffType.Silence);
        public bool IsProvoked => HasDebuff(DebuffType.Provoke);

        // ----------------------------------------------------------------
        // Immunity buff (from Immunity gear set 2pc)
        // ----------------------------------------------------------------

        private bool HasImmunityBuff { get; set; }

        public void GrantImmunityBuff()
        {
            HasImmunityBuff = true;
            Debug.Log($"[Buff] {Data.heroName} gained Immunity buff.");
        }

        private void ConsumeImmunityBuff() => HasImmunityBuff = false;

        // ----------------------------------------------------------------
        // Stat modifier application from debuffs
        // ----------------------------------------------------------------

        private void ApplyStatModifiersFromDebuff(DebuffInstance debuff)
        {
            switch (debuff.Type)
            {
                case DebuffType.Curse:   modifiers.AddMultiplier(StatType.ATK,      -debuff.Magnitude); break;
                case DebuffType.Blind:   modifiers.AddFlat(StatType.CritRate,        -debuff.Magnitude); break;
                case DebuffType.Weaken:  modifiers.AddMultiplier(StatType.ATK,      -debuff.Magnitude); break;
                // Silence, Stun, Freeze, Provoke are behavioural — no stat modifier needed
                // Ignite, Burn, Scorch, Chill are handled in OnTurnStart or on application
            }
        }

        private void RemoveStatModifiersFromDebuff(DebuffInstance debuff)
        {
            switch (debuff.Type)
            {
                case DebuffType.Curse:   modifiers.RemoveMultiplier(StatType.ATK,   -debuff.Magnitude); break;
                case DebuffType.Blind:   modifiers.RemoveFlat(StatType.CritRate,     -debuff.Magnitude); break;
                case DebuffType.Weaken:  modifiers.RemoveMultiplier(StatType.ATK,   -debuff.Magnitude); break;
            }
        }

        // ----------------------------------------------------------------
        // Equipment modifier passthrough
        // ----------------------------------------------------------------

        /// <summary>
        /// Exposes the internal StatModifiers for the equipment system to write into.
        /// </summary>
        public StatModifiers Modifiers => modifiers;
    }
}
