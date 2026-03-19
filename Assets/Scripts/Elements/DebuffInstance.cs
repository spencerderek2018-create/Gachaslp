using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// A live debuff applied to a BattleUnit during combat.
    /// Tracks remaining duration and ticks DoT effects each turn.
    /// </summary>
    public class DebuffInstance
    {
        public DebuffType Type           { get; private set; }
        public int RemainingTurns        { get; private set; }
        public float Magnitude           { get; private set; } // e.g. 0.2 = 20% stat reduction, or DoT multiplier
        public BattleUnit Source         { get; private set; } // who applied it (DoT scales off their ATK)

        public bool IsExpired => RemainingTurns <= 0;

        public DebuffInstance(DebuffType type, int duration, float magnitude, BattleUnit source)
        {
            Type          = type;
            RemainingTurns = duration;
            Magnitude     = magnitude;
            Source        = source;
        }

        /// <summary>
        /// Called at the start of the afflicted unit's turn.
        /// Ticks DoT damage and decrements duration.
        /// </summary>
        public void OnTurnStart(BattleUnit afflicted)
        {
            if (DebuffCategory.IsDoT(Type))
            {
                float dotDamage = Source.FinalATK * Magnitude;

                // Scorch stacks: each additional stack multiplies damage by stack count
                if (Type == DebuffType.Scorch)
                    dotDamage *= afflicted.GetStackCount(DebuffType.Scorch);

                int dmg = Mathf.RoundToInt(dotDamage);
                afflicted.TakeDamage(dmg, isDot: true);

                Debug.Log($"[DoT] {afflicted.Data.heroName} takes {dmg} {Type} damage. ({RemainingTurns - 1} turns remaining)");
            }

            RemainingTurns--;
        }

        /// <summary>
        /// Chill is an instant CR reduction — applied on landing, not per turn.
        /// Call this once immediately after TryApplyDebuff succeeds.
        /// </summary>
        public void ApplyInstantEffect(BattleUnit afflicted, TurnManager turnManager)
        {
            if (Type == DebuffType.Chill)
                turnManager.PullCR(afflicted, Magnitude * 100f);
        }
    }
}
