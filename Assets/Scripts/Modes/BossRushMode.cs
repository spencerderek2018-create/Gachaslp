using System.Collections.Generic;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Boss Rush Mode — 10 rounds of escalating bosses.
    ///
    /// Key rules:
    ///   - Hero HP and cooldowns persist between rounds (no full restore)
    ///   - Player selects a 12-hero roster before entering; can swap active 4 between rounds
    ///   - Weekly rotating global modifiers (set externally before mode creation)
    ///   - Scoring: speed, damage taken, surviving heroes
    /// </summary>
    public class BossRushMode : IGameMode
    {
        public const int MaxRounds = 10;

        public int     CurrentRound  { get; private set; } = 0;
        public int     Score         { get; private set; } = 0;
        public string  GlobalModifier { get; set; } = ""; // e.g. "All fire damage +30%"

        // Persisted unit state between rounds
        private List<BattleUnit> persistedPlayerUnits;

        // Scoring factors accumulated per round
        private int totalDamageTaken = 0;

        public BossRushMode(string globalModifier = "")
        {
            GlobalModifier = globalModifier;
        }

        public void OnBattleStart(BattleManager battle)
        {
            Debug.Log($"[Boss Rush] Round {CurrentRound + 1}/{MaxRounds} started. Modifier: {GlobalModifier}");

            // Restore persisted HP/cooldowns from last round
            if (persistedPlayerUnits != null && persistedPlayerUnits.Count > 0)
                battle.RestoreUnits(persistedPlayerUnits);
        }

        public void OnRoundEnd(BattleManager battle, bool playerWon)
        {
            if (!playerWon)
            {
                Debug.Log($"[Boss Rush] Player wiped on round {CurrentRound + 1}. Run over.");
                CurrentRound = MaxRounds; // force IsModeOver = true
                return;
            }

            // Snapshot surviving player units (HP and cooldowns carry over)
            persistedPlayerUnits = new List<BattleUnit>(battle.GetPlayerUnits());

            // Accumulate score for this round
            int roundScore = CalculateRoundScore(battle);
            Score += roundScore;
            Debug.Log($"[Boss Rush] Round {CurrentRound + 1} cleared. Round score: {roundScore}. Total: {Score}");

            CurrentRound++;
        }

        public bool IsModeOver() => CurrentRound >= MaxRounds;

        public ModeRewards CalculateRewards()
        {
            var r = new ModeRewards
            {
                Gold      = Score * 100,
                Skystones = Mathf.Clamp(CurrentRound * 5, 0, 50),
                Bookmarks = CurrentRound >= MaxRounds ? 10 : 0,
                BonusMessage = CurrentRound >= MaxRounds
                    ? "Boss Rush Complete! Platinum rewards unlocked!"
                    : $"Reached Round {CurrentRound}."
            };

            // Exclusive Boss Rush gear drops for completing the gauntlet
            if (CurrentRound >= MaxRounds)
            {
                r.GearDrops.Add(new GearInstance
                {
                    tier = GearTier.Epic,
                    set  = GearSet.Destruction,
                    slot = GearSlot.Necklace,
                    enhanceLevel = 0
                });
            }

            return r;
        }

        // ----------------------------------------------------------------

        private int CalculateRoundScore(BattleManager battle)
        {
            int survivingHeroes = battle.GetPlayerUnits().Count;
            // TODO: factor in time taken and damage received once those metrics are tracked
            return survivingHeroes * 1000 + (10 - CurrentRound) * 200;
        }
    }
}
