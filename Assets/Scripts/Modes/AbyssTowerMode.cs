using System.Collections.Generic;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Abyss Tower Mode — 120 progressively difficult floors.
    ///
    /// Key rules:
    ///   - No hero reuse within a 20-floor block
    ///   - Progress is saved permanently (floors stay cleared)
    ///   - Floors 101–120 reset each season with new bosses
    ///   - Each floor can have a unique puzzle mechanic (defined externally per floor)
    /// </summary>
    public class AbyssTowerMode : IGameMode
    {
        public int FloorNumber { get; private set; }

        // Tracks which heroes have been used in the current 20-floor block
        // Key: hero name, Value: true if used
        private HashSet<string> usedHeroesThisBlock = new();

        // Saved floor progress (persisted across sessions)
        private static readonly string ProgressKey = "AbyssTower_HighestFloor";

        public static int SavedHighestFloor =>
            PlayerPrefs.GetInt(ProgressKey, 0);

        public AbyssTowerMode(int floorNumber)
        {
            FloorNumber = floorNumber;

            // If crossing into a new 20-floor block, reset used hero tracker
            int currentBlock = (floorNumber - 1) / 20;
            int savedBlock    = (SavedHighestFloor - 1) / 20;
            if (currentBlock != savedBlock)
                usedHeroesThisBlock.Clear();
        }

        public void OnBattleStart(BattleManager battle)
        {
            Debug.Log($"[Abyss Tower] Floor {FloorNumber} started.");

            // Validate that no locked-out heroes are in the player's team
            foreach (var unit in battle.GetPlayerUnits())
            {
                if (usedHeroesThisBlock.Contains(unit.Data.heroName))
                {
                    Debug.LogWarning($"[Abyss Tower] {unit.Data.heroName} was already used " +
                                     $"in this 20-floor block and should not be selectable.");
                }
            }
        }

        public void OnRoundEnd(BattleManager battle, bool playerWon)
        {
            if (!playerWon)
            {
                Debug.Log($"[Abyss Tower] Floor {FloorNumber} failed. No progress saved.");
                return;
            }

            // Lock out the heroes used on this floor for the rest of the 20-floor block
            foreach (var unit in battle.GetPlayerUnits())
                usedHeroesThisBlock.Add(unit.Data.heroName);

            // Save highest floor reached
            if (FloorNumber > SavedHighestFloor)
            {
                PlayerPrefs.SetInt(ProgressKey, FloorNumber);
                PlayerPrefs.Save();
            }

            Debug.Log($"[Abyss Tower] Floor {FloorNumber} cleared! Progress saved.");
        }

        public bool IsModeOver() => true; // One floor at a time

        public ModeRewards CalculateRewards()
        {
            var r = new ModeRewards();

            // Milestone rewards
            if (FloorNumber == 80)
            {
                r.Bookmarks    = 5;
                r.BonusMessage = "Floor 80 Cleared — Moonlight Hero Selector unlocked!";
            }
            else if (FloorNumber == 100)
            {
                r.Bookmarks    = 10;
                r.BonusMessage = "Floor 100 Cleared — Legendary Artifact Selector unlocked!";
            }
            else
            {
                // Standard per-floor rewards
                r.Gold      = FloorNumber * 200;
                r.Skystones = FloorNumber % 10 == 0 ? 30 : 5;
            }

            return r;
        }

        /// <summary>Returns true if a hero is locked out of this 20-floor block.</summary>
        public bool IsHeroLocked(string heroName) =>
            usedHeroesThisBlock.Contains(heroName);

        /// <summary>Resets the seasonal floors (101–120) progress.</summary>
        public static void ResetSeasonalFloors()
        {
            int current = SavedHighestFloor;
            if (current > 100)
            {
                PlayerPrefs.SetInt(ProgressKey, 100);
                PlayerPrefs.Save();
                Debug.Log("[Abyss Tower] Seasonal reset — floors 101–120 reset to 100.");
            }
        }
    }
}
