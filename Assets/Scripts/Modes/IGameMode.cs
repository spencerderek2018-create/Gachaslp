using System.Collections.Generic;

namespace GachaRPG
{
    /// <summary>
    /// Interface all game modes implement.
    /// BattleManager calls these hooks at the appropriate moments.
    /// Each mode is a thin wrapper — real combat logic stays in BattleManager.
    /// </summary>
    public interface IGameMode
    {
        /// <summary>Called once when the battle scene initialises.</summary>
        void OnBattleStart(BattleManager battle);

        /// <summary>Called after each fight ends (single battles call this once).</summary>
        void OnRoundEnd(BattleManager battle, bool playerWon);

        /// <summary>Returns true when the mode's full sequence is complete.</summary>
        bool IsModeOver();

        /// <summary>Calculates and returns rewards based on mode performance.</summary>
        ModeRewards CalculateRewards();
    }

    /// <summary>
    /// Reward container returned by all game modes.
    /// </summary>
    public class ModeRewards
    {
        public int Gold;
        public int Skystones;
        public int Bookmarks;
        public List<GearInstance> GearDrops = new();
        public string BonusMessage; // e.g. "First Clear Bonus!"
    }
}
