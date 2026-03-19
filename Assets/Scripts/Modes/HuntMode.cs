using System.Collections.Generic;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Hunt Mode — repeatable single-fight PvE for gear farming.
    ///
    /// Each Hunt boss is tied to specific gear sets.
    /// Hunt level (1–13) determines gear tier drop rates.
    /// Auto-battle and multi-run are handled at a higher level (not here).
    /// </summary>
    public class HuntMode : IGameMode
    {
        public int HuntLevel;                  // 1–13
        public GearSet[] PossibleDropSets;     // Sets this hunt boss can drop

        private bool playerWon;
        private ModeRewards rewards;

        // Gear tier drop weight tables keyed by hunt level range
        private static readonly Dictionary<int, float[]> TierWeights = new()
        {
            // [Normal, Good, Rare, Heroic, Epic]
            { 1,  new[] { 0.50f, 0.35f, 0.15f, 0.00f, 0.00f } },
            { 5,  new[] { 0.10f, 0.35f, 0.35f, 0.15f, 0.05f } },
            { 10, new[] { 0.00f, 0.10f, 0.35f, 0.35f, 0.20f } },
            { 13, new[] { 0.00f, 0.00f, 0.15f, 0.45f, 0.40f } },
        };

        public HuntMode(int level, GearSet[] dropSets)
        {
            HuntLevel      = Mathf.Clamp(level, 1, 13);
            PossibleDropSets = dropSets;
        }

        public void OnBattleStart(BattleManager battle)
        {
            Debug.Log($"[Hunt] Level {HuntLevel} hunt started.");
        }

        public void OnRoundEnd(BattleManager battle, bool won)
        {
            playerWon = won;
            if (won)
                rewards = BuildRewards();
        }

        public bool IsModeOver() => true; // Single fight

        public ModeRewards CalculateRewards() => rewards ?? new ModeRewards();

        // ----------------------------------------------------------------

        private ModeRewards BuildRewards()
        {
            var r = new ModeRewards
            {
                Gold      = HuntLevel * 500,
                Skystones = HuntLevel >= 10 ? 3 : 0,
            };

            // Roll 1–3 gear drops
            int dropCount = Random.Range(1, 4);
            for (int i = 0; i < dropCount; i++)
                r.GearDrops.Add(RollGearDrop());

            return r;
        }

        private GearInstance RollGearDrop()
        {
            var gear = new GearInstance
            {
                slot = (GearSlot)Random.Range(0, System.Enum.GetValues(typeof(GearSlot)).Length),
                set  = PossibleDropSets[Random.Range(0, PossibleDropSets.Length)],
                tier = RollTier(),
                enhanceLevel = 0,
            };

            // Assign a valid main stat for the slot
            gear.mainStat  = GetMainStat(gear.slot);
            gear.mainStatValue = GetBaseMainStatValue(gear.mainStat);

            // Roll starting substats based on tier
            var pool = new StatType[]
            {
                StatType.HP, StatType.ATK, StatType.DEF, StatType.Speed,
                StatType.CritRate, StatType.CritDamage, StatType.Effectiveness, StatType.EffectResist
            };

            for (int i = 0; i < gear.StartingSubstatCount; i++)
                gear.Enhance(pool);

            return gear;
        }

        private GearTier RollTier()
        {
            // Find the closest tier weight table entry for this hunt level
            int key = HuntLevel >= 13 ? 13 : HuntLevel >= 10 ? 10 : HuntLevel >= 5 ? 5 : 1;
            float[] weights = TierWeights[key];

            float roll = Random.value;
            float cumulative = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative) return (GearTier)i;
            }
            return GearTier.Rare;
        }

        private StatType GetMainStat(GearSlot slot) => slot switch
        {
            GearSlot.Weapon   => StatType.ATK,
            GearSlot.Helmet   => StatType.HP,
            GearSlot.Armor    => StatType.DEF,
            GearSlot.Necklace => RollRightSideMainStat(includeOffensive: true),
            GearSlot.Ring     => RollRightSideMainStat(includeOffensive: false),
            GearSlot.Boots    => RollBootsMainStat(),
            _                 => StatType.ATK
        };

        private StatType RollRightSideMainStat(bool includeOffensive)
        {
            var options = includeOffensive
                ? new[] { StatType.ATK, StatType.HP, StatType.DEF, StatType.CritRate, StatType.CritDamage }
                : new[] { StatType.ATK, StatType.HP, StatType.DEF, StatType.Effectiveness, StatType.EffectResist };
            return options[Random.Range(0, options.Length)];
        }

        private StatType RollBootsMainStat()
        {
            var options = new[] { StatType.ATK, StatType.HP, StatType.DEF, StatType.Speed };
            return options[Random.Range(0, options.Length)];
        }

        private float GetBaseMainStatValue(StatType stat) => stat switch
        {
            StatType.HP           => 800f,
            StatType.ATK          => 200f,
            StatType.DEF          => 100f,
            StatType.Speed        => 45f,
            StatType.CritRate     => 0.08f,
            StatType.CritDamage   => 0.65f,
            StatType.Effectiveness => 0.08f,
            StatType.EffectResist => 0.08f,
            _                     => 0f
        };
    }
}
