using System.Collections.Generic;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// A live gear piece with rolled substats.
    /// Unlike EquipmentData (the template ScriptableObject), GearInstance is a runtime
    /// object representing an actual item in the player's inventory with its specific rolls.
    /// </summary>
    [System.Serializable]
    public class GearInstance
    {
        public GearSlot  slot;
        public GearTier  tier;
        public GearSet   set;
        public int       enhanceLevel;    // 0–15
        public StatType  mainStat;
        public float     mainStatValue;   // e.g. 400 flat ATK, or 0.08 for 8% ATK%

        public List<SubstatRoll> substats = new();

        /// <summary>
        /// How many substats this piece starts with, determined by tier.
        /// </summary>
        public int StartingSubstatCount => (int)tier;

        /// <summary>
        /// Returns the main stat value scaled to enhance level.
        /// Main stat grows linearly from base to ~2x at +15.
        /// </summary>
        public float ScaledMainStatValue =>
            mainStatValue * (1f + enhanceLevel * 0.1f);

        /// <summary>
        /// Enhances the gear by one step (+3, +6, +9, +12, +15).
        /// At each threshold, a random substat is upgraded.
        /// If the piece has fewer than 4 substats, a new one is added instead.
        /// </summary>
        /// <param name="possibleSubstats">Pool of stat types to roll from.</param>
        public void Enhance(StatType[] possibleSubstats)
        {
            if (enhanceLevel >= 15) return;
            enhanceLevel++;

            bool isThreshold = enhanceLevel % 3 == 0;
            if (!isThreshold) return;

            if (substats.Count < 4)
            {
                // Add a new substat that isn't already on this piece
                var existing = new System.Collections.Generic.HashSet<StatType>(
                    substats.ConvertAll(s => s.stat));
                var candidates = System.Array.FindAll(possibleSubstats, s => !existing.Contains(s));

                if (candidates.Length > 0)
                {
                    var newStat = candidates[Random.Range(0, candidates.Length)];
                    substats.Add(new SubstatRoll(newStat, RollSubstatValue(newStat)));
                }
            }
            else
            {
                // Upgrade a random existing substat
                int idx = Random.Range(0, substats.Count);
                substats[idx].Enhance(RollSubstatValue(substats[idx].stat));
            }
        }

        private float RollSubstatValue(StatType stat)
        {
            // Flat stats roll higher values, percentage stats roll lower
            return stat switch
            {
                StatType.HP           => Random.Range(80f,  200f),
                StatType.ATK          => Random.Range(20f,  50f),
                StatType.DEF          => Random.Range(10f,  30f),
                StatType.Speed        => Random.Range(1f,   4f),
                StatType.CritRate     => Random.Range(0.03f, 0.08f),
                StatType.CritDamage   => Random.Range(0.05f, 0.12f),
                StatType.Effectiveness => Random.Range(0.04f, 0.10f),
                StatType.EffectResist => Random.Range(0.04f, 0.10f),
                _                     => Random.Range(0.03f, 0.08f)
            };
        }
    }

    [System.Serializable]
    public class SubstatRoll
    {
        public StatType stat;
        public float    value;

        public SubstatRoll(StatType stat, float value)
        {
            this.stat  = stat;
            this.value = value;
        }

        /// <summary>Increases this substat's value by the given bonus amount.</summary>
        public void Enhance(float bonus) => value += bonus;
    }
}
