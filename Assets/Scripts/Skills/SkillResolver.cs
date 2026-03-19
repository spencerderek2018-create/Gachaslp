using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Executes a SkillData against targets.
    /// Resolves each SkillEffect in order: damage, heal, debuffs, CR manipulation, extra turns.
    /// Handles both normal and Soul Burn execution paths.
    /// </summary>
    public static class SkillResolver
    {
        /// <summary>
        /// Main entry point. Executes all effects of a skill.
        /// </summary>
        /// <param name="caster">Unit using the skill.</param>
        /// <param name="skill">The SkillData asset to execute.</param>
        /// <param name="selectedTargets">Targets chosen by the player or AI (used for single-target effects).</param>
        /// <param name="allUnits">All living units in battle (used for AoE targeting).</param>
        /// <param name="soulManager">Shared Soul resource.</param>
        /// <param name="turnManager">CR manipulation.</param>
        /// <param name="isSoulBurn">Whether to use the Soul Burn effect array instead of normal effects.</param>
        public static void Execute(
            BattleUnit caster,
            SkillData skill,
            List<BattleUnit> selectedTargets,
            List<BattleUnit> allUnits,
            SoulManager soulManager,
            TurnManager turnManager,
            bool isSoulBurn = false)
        {
            if (skill == null)
            {
                Debug.LogWarning($"[SkillResolver] {caster.Data.heroName} tried to use a null skill.");
                return;
            }

            var effects = (isSoulBurn && skill.hasSoulBurn) ? skill.soulBurnEffects : skill.effects;

            if (effects == null || effects.Length == 0)
            {
                Debug.LogWarning($"[SkillResolver] Skill '{skill.skillName}' has no effects defined.");
                return;
            }

            Debug.Log($"[Skill] {caster.Data.heroName} uses {skill.skillName}" +
                      (isSoulBurn ? " (Soul Burn)" : ""));

            foreach (var effect in effects)
                ResolveEffect(effect, caster, selectedTargets, allUnits, soulManager, turnManager);

            // Generate Soul (not from Soul Burn variants — they cost soul, not generate)
            if (!isSoulBurn)
                soulManager.AddSoul(skill.soulGenerated);

            // Set cooldown on the caster
            if (skill.slot != SkillSlot.S4_Passive)
                caster.SetCooldown(skill.slot, skill.cooldownTurns);
        }

        // ----------------------------------------------------------------
        // Effect resolution
        // ----------------------------------------------------------------

        private static void ResolveEffect(
            SkillEffect effect,
            BattleUnit caster,
            List<BattleUnit> selectedTargets,
            List<BattleUnit> allUnits,
            SoulManager soulManager,
            TurnManager turnManager)
        {
            var targets = ResolveTargets(effect.target, caster, selectedTargets, allUnits);

            foreach (var target in targets)
            {
                if (target.IsDead) continue;

                switch (effect.type)
                {
                    case EffectType.Damage:
                        ResolveDamage(caster, target, effect, soulManager);
                        break;

                    case EffectType.Heal:
                        int healAmount = Mathf.RoundToInt(caster.FinalATK * effect.multiplier);
                        target.Heal(healAmount);
                        Debug.Log($"[Heal] {target.Data.heroName} healed for {healAmount}.");
                        break;

                    case EffectType.ApplyDebuff:
                        ResolveDebuff(caster, target, effect, turnManager);
                        break;

                    case EffectType.PushCR:
                        if (turnManager != null)
                        {
                            float pushAmt = effect.crPushAmount * 100f;
                            turnManager.PushCR(target, pushAmt);
                            Debug.Log($"[CR] {target.Data.heroName} CR pushed by {pushAmt:F0}.");
                        }
                        break;

                    case EffectType.PullCR:
                        if (turnManager != null)
                        {
                            float pullAmt = Mathf.Abs(effect.crPushAmount) * 100f;
                            turnManager.PullCR(target, pullAmt);
                            Debug.Log($"[CR] {target.Data.heroName} CR pulled by {pullAmt:F0}.");
                        }
                        break;

                    case EffectType.GrantExtraTurn:
                        // Push caster to 100 CR — they will act again immediately after
                        if (turnManager != null)
                        {
                            turnManager.PushCR(caster, 100f);
                            Debug.Log($"[Extra Turn] {caster.Data.heroName} gains an extra turn.");
                        }
                        break;
                }
            }
        }

        // ----------------------------------------------------------------
        // Damage resolution
        // ----------------------------------------------------------------

        private static void ResolveDamage(
            BattleUnit caster,
            BattleUnit target,
            SkillEffect effect,
            SoulManager soulManager)
        {
            float baseDamage = caster.FinalATK * effect.multiplier;

            // Elemental affinity modifier
            float affinity = ElementalAffinity.GetMultiplier(caster.Data.element, target.Data.element);
            baseDamage *= affinity;

            // Crit check
            bool isCrit = Random.value <= caster.FinalCritRate;
            if (isCrit) baseDamage *= caster.FinalCritDamage;

            int finalDamage = Mathf.RoundToInt(baseDamage);
            target.TakeDamage(finalDamage, isDot: false);

            Debug.Log($"[Damage] {caster.Data.heroName} → {target.Data.heroName}: " +
                      $"{finalDamage} dmg (affinity x{affinity:F2}{(isCrit ? ", CRIT" : "")})");

            // Lifesteal (Counter set lifesteal handled separately in BattleManager)
            // Artifact: HealOnCrit
            if (isCrit && caster.EquippedArtifact != null &&
                caster.EquippedArtifact.effectType == ArtifactEffectType.HealOnCrit)
            {
                int lifesteal = Mathf.RoundToInt(finalDamage * caster.EquippedArtifact.effectMagnitude);
                caster.Heal(lifesteal);
            }
        }

        // ----------------------------------------------------------------
        // Debuff resolution
        // ----------------------------------------------------------------

        private static void ResolveDebuff(
            BattleUnit caster,
            BattleUnit target,
            SkillEffect effect,
            TurnManager turnManager)
        {
            if (effect.debuffToApply == DebuffType.None) return;

            // Base land chance from skill + effectiveness/resistance resolved inside TryApplyDebuff
            var debuff = new DebuffInstance(
                effect.debuffToApply,
                effect.debuffDuration,
                effect.multiplier,
                caster);

            // Artifact: DebuffChanceBonus adds flat % to land chance via effectiveness
            // (handled inside BattleUnit.TryApplyDebuff via FinalEffectiveness)

            target.TryApplyDebuff(debuff, turnManager);
        }

        // ----------------------------------------------------------------
        // Target resolution
        // ----------------------------------------------------------------

        private static List<BattleUnit> ResolveTargets(
            TargetType targetType,
            BattleUnit caster,
            List<BattleUnit> selected,
            List<BattleUnit> all)
        {
            bool isPlayer = caster.IsPlayerTeam;

            return targetType switch
            {
                TargetType.SingleEnemy =>
                    selected.Where(u => !u.IsPlayerTeam == isPlayer && !u.IsDead).Take(1).ToList(),

                TargetType.AllEnemies =>
                    all.Where(u => u.IsPlayerTeam != isPlayer && !u.IsDead).ToList(),

                TargetType.SingleAlly =>
                    selected.Where(u => u.IsPlayerTeam == isPlayer && !u.IsDead).Take(1).ToList(),

                TargetType.AllAllies =>
                    all.Where(u => u.IsPlayerTeam == isPlayer && !u.IsDead).ToList(),

                TargetType.Self =>
                    new List<BattleUnit> { caster },

                _ => new List<BattleUnit>()
            };
        }
    }
}
