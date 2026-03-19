using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Manages the Combat Readiness (CR) gauge for all units.
    /// Units fill their gauge based on Speed. First to reach 100 takes their turn.
    /// CR is float-based — never convert to integer ticks.
    /// </summary>
    public class TurnManager
    {
        private List<BattleUnit> allUnits;

        public TurnManager(List<BattleUnit> units)
        {
            allUnits = units;
        }

        /// <summary>
        /// Advances CR gauges until one living unit reaches 100.
        /// Subtracts 100 from that unit and returns it.
        /// </summary>
        public BattleUnit AdvanceToNextTurn()
        {
            while (true)
            {
                // Check if any unit is already at 100+
                BattleUnit ready = allUnits
                    .Where(u => !u.IsDead)
                    .FirstOrDefault(u => u.CombatReadiness >= 100f);

                if (ready != null)
                {
                    ready.CombatReadiness -= 100f;
                    return ready;
                }

                TickAllUnits();
            }
        }

        /// <summary>
        /// Advances all units by the exact amount needed for the fastest unit to hit 100.
        /// Prevents floating point drift from many small ticks.
        /// </summary>
        private void TickAllUnits()
        {
            var living = allUnits.Where(u => !u.IsDead).ToList();
            if (living.Count == 0) return;

            // Find how many "time units" until the fastest unit hits 100
            float minTimeToReady = living.Min(u => (100f - u.CombatReadiness) / Mathf.Max(1, u.FinalSpeed));

            foreach (var unit in living)
                unit.CombatReadiness += unit.FinalSpeed * minTimeToReady;
        }

        /// <summary>Pushes a unit's CR forward by amount (0–100 scale). Clamps to 100.</summary>
        public void PushCR(BattleUnit target, float amount)
        {
            target.CombatReadiness = Mathf.Min(100f, target.CombatReadiness + amount);
        }

        /// <summary>Pulls a unit's CR back by amount (0–100 scale). Clamps to 0.</summary>
        public void PullCR(BattleUnit target, float amount)
        {
            target.CombatReadiness = Mathf.Max(0f, target.CombatReadiness - amount);
        }

        /// <summary>
        /// Simulates the turn order for the next <paramref name="count"/> turns without
        /// mutating any real unit state. Used by the turn order bar UI.
        /// </summary>
        public List<BattleUnit> PreviewTurnOrder(int count)
        {
            // Clone CR values into a lightweight struct list
            var snapshot = allUnits
                .Where(u => !u.IsDead)
                .Select(u => (unit: u, cr: u.CombatReadiness))
                .ToList();

            var order = new List<BattleUnit>();

            for (int i = 0; i < count; i++)
            {
                // Advance simulation until a unit hits 100
                int safety = 0;
                while (safety++ < 10000)
                {
                    int readyIdx = snapshot.FindIndex(s => s.cr >= 100f);
                    if (readyIdx >= 0)
                    {
                        var s = snapshot[readyIdx];
                        order.Add(s.unit);
                        snapshot[readyIdx] = (s.unit, s.cr - 100f);
                        break;
                    }

                    // Tick simulation
                    float minGap = snapshot.Min(s => (100f - s.cr) / Mathf.Max(1, s.unit.FinalSpeed));
                    for (int j = 0; j < snapshot.Count; j++)
                    {
                        var s = snapshot[j];
                        snapshot[j] = (s.unit, s.cr + s.unit.FinalSpeed * minGap);
                    }
                }
            }

            return order;
        }

        /// <summary>Registers a newly spawned unit (e.g. summoned mid-battle).</summary>
        public void RegisterUnit(BattleUnit unit)
        {
            if (!allUnits.Contains(unit))
                allUnits.Add(unit);
        }

        /// <summary>Removes a unit from CR tracking (on death).</summary>
        public void UnregisterUnit(BattleUnit unit)
        {
            allUnits.Remove(unit);
        }
    }
}
