using System.Collections.Generic;

namespace GachaRPG
{
    /// <summary>
    /// Tracks additive percentage multipliers and flat bonuses per stat.
    /// Final stat = base * (1 + sum of multipliers) + sum of flat bonuses.
    ///
    /// Example: base ATK 1000, Curse (-20% multiplier), Assault set (+35% multiplier)
    ///   → 1000 * (1 + 0.35 - 0.20) = 1150
    /// </summary>
    public class StatModifiers
    {
        private readonly Dictionary<StatType, float> multipliers = new();
        private readonly Dictionary<StatType, float> flats       = new();

        public float GetMultiplier(StatType stat) =>
            1f + multipliers.GetValueOrDefault(stat, 0f);

        public float GetFlat(StatType stat) =>
            flats.GetValueOrDefault(stat, 0f);

        public void AddMultiplier(StatType stat, float value)
        {
            multipliers[stat] = multipliers.GetValueOrDefault(stat, 0f) + value;
        }

        public void RemoveMultiplier(StatType stat, float value)
        {
            multipliers[stat] = multipliers.GetValueOrDefault(stat, 0f) - value;
        }

        public void AddFlat(StatType stat, float value)
        {
            flats[stat] = flats.GetValueOrDefault(stat, 0f) + value;
        }

        public void RemoveFlat(StatType stat, float value)
        {
            flats[stat] = flats.GetValueOrDefault(stat, 0f) - value;
        }

        public void Clear()
        {
            multipliers.Clear();
            flats.Clear();
        }
    }
}
