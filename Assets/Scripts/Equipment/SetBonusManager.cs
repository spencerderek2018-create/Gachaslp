using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Detects equipped gear sets and applies their bonuses to a unit's StatModifiers.
    ///
    /// Called when a hero equips or unequips gear (not every frame).
    /// Lifesteal and Counter are flagged separately — they are resolved at damage time
    /// in BattleManager, not as stat modifiers.
    /// Immunity (2pc) is flagged on the BattleUnit — triggers at turn start.
    /// </summary>
    public static class SetBonusManager
    {
        // Flags written onto BattleUnit when the relevant sets are equipped
        // (BattleManager reads these during damage resolution)
        public static readonly HashSet<BattleUnit> LifestealUnits = new();
        public static readonly HashSet<BattleUnit> CounterUnits   = new();
        public const float LifestealRate    = 0.20f;
        public const float CounterChance    = 0.20f;

        /// <summary>
        /// Clears all set bonuses from the modifiers, then re-applies based on currently
        /// equipped gear. Call this whenever the loadout changes.
        /// </summary>
        public static void ApplySetBonuses(
            BattleUnit unit,
            List<GearInstance> equippedGear,
            StatModifiers modifiers)
        {
            // Count pieces per set
            var setCounts = equippedGear
                .GroupBy(g => g.set)
                .ToDictionary(g => g.Key, g => g.Count());

            // Clear old set flags
            LifestealUnits.Remove(unit);
            CounterUnits.Remove(unit);

            foreach (var (set, count) in setCounts)
            {
                // 4-piece bonuses (also grant the 2-piece if applicable for sets that have both)
                if (count >= 4)
                    Apply4PieceBonus(set, unit, modifiers);
                else if (count >= 2)
                    Apply2PieceBonus(set, unit, modifiers);
            }
        }

        private static void Apply4PieceBonus(GearSet set, BattleUnit unit, StatModifiers m)
        {
            switch (set)
            {
                case GearSet.Assault:
                    m.AddMultiplier(StatType.ATK, 0.35f);
                    Debug.Log($"[Set] {unit.Data.heroName}: Assault 4pc — +35% ATK");
                    break;

                case GearSet.Velocity:
                    m.AddMultiplier(StatType.Speed, 0.25f);
                    Debug.Log($"[Set] {unit.Data.heroName}: Velocity 4pc — +25% Speed");
                    break;

                case GearSet.Destruction:
                    m.AddFlat(StatType.CritDamage, 0.40f);
                    Debug.Log($"[Set] {unit.Data.heroName}: Destruction 4pc — +40% Crit Damage");
                    break;

                case GearSet.Lifesteal:
                    LifestealUnits.Add(unit);
                    Debug.Log($"[Set] {unit.Data.heroName}: Lifesteal 4pc — 20% damage healed");
                    break;

                case GearSet.Counter:
                    CounterUnits.Add(unit);
                    Debug.Log($"[Set] {unit.Data.heroName}: Counter 4pc — 20% counter chance");
                    break;
            }
        }

        private static void Apply2PieceBonus(GearSet set, BattleUnit unit, StatModifiers m)
        {
            switch (set)
            {
                case GearSet.Vitality:
                    m.AddMultiplier(StatType.HP, 0.15f);
                    Debug.Log($"[Set] {unit.Data.heroName}: Vitality 2pc — +15% HP");
                    break;

                case GearSet.Ironwall:
                    m.AddMultiplier(StatType.DEF, 0.15f);
                    Debug.Log($"[Set] {unit.Data.heroName}: Ironwall 2pc — +15% DEF");
                    break;

                case GearSet.Critical:
                    m.AddFlat(StatType.CritRate, 0.12f);
                    Debug.Log($"[Set] {unit.Data.heroName}: Critical 2pc — +12% Crit Rate");
                    break;

                case GearSet.Resist:
                    m.AddFlat(StatType.EffectResist, 0.20f);
                    Debug.Log($"[Set] {unit.Data.heroName}: Resist 2pc — +20% Effect Resist");
                    break;

                case GearSet.Immunity:
                    unit.GrantImmunityBuff();
                    Debug.Log($"[Set] {unit.Data.heroName}: Immunity 2pc — debuff immunity granted");
                    break;
            }
        }

        /// <summary>
        /// Applies all substat values from a gear piece into the unit's StatModifiers.
        /// </summary>
        public static void ApplySubstats(GearInstance gear, StatModifiers modifiers)
        {
            // Main stat
            bool isPercentStat = gear.mainStat is StatType.CritRate or StatType.CritDamage
                                                or StatType.Effectiveness or StatType.EffectResist;
            if (isPercentStat)
                modifiers.AddFlat(gear.mainStat, gear.ScaledMainStatValue);
            else
                modifiers.AddFlat(gear.mainStat, gear.ScaledMainStatValue);

            // Substats
            foreach (var sub in gear.substats)
            {
                bool subIsPercent = sub.stat is StatType.CritRate or StatType.CritDamage
                                              or StatType.Effectiveness or StatType.EffectResist;
                if (subIsPercent)
                    modifiers.AddFlat(sub.stat, sub.value);
                else
                    modifiers.AddFlat(sub.stat, sub.value);
            }
        }
    }
}
