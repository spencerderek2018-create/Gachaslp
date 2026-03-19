using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Resolves the Dual Attack system.
    ///
    /// After any S1 basic attack, there is a base 5% chance that a random living ally
    /// will follow up with their own S1. Dual attacks:
    ///   - Do NOT generate Soul
    ///   - Do NOT trigger another dual attack (no chain recursion)
    ///   - Do NOT consume or reset cooldowns on the dual attacker
    ///   - Target the same enemy as the original attacker (if alive), else a random enemy
    ///
    /// TODO: Artifact and passive bonuses to dual attack chance should be added here
    ///       by reading EquippedArtifact.effectType == DualAttackChanceBonus.
    /// </summary>
    public class DualAttackResolver
    {
        private const float BaseDualAttackChance = 0.05f;

        /// <summary>
        /// Called after an S1 basic attack. Rolls for and executes a dual attack if triggered.
        /// </summary>
        /// <param name="originalAttacker">The unit that just used S1.</param>
        /// <param name="allies">All living allies of the attacker (excluding the attacker).</param>
        /// <param name="enemies">All living enemies.</param>
        /// <param name="originalTarget">The enemy that was attacked (preferred dual attack target).</param>
        /// <returns>The unit that performed the dual attack, or null if none triggered.</returns>
        public BattleUnit TryTriggerDualAttack(
            BattleUnit originalAttacker,
            List<BattleUnit> allies,
            List<BattleUnit> enemies,
            BattleUnit originalTarget)
        {
            float chance = BaseDualAttackChance;

            // TODO: Add artifact bonus
            // if (originalAttacker.EquippedArtifact?.effectType == ArtifactEffectType.DualAttackChanceBonus)
            //     chance += originalAttacker.EquippedArtifact.effectMagnitude;

            if (Random.value > chance)
                return null;

            // Pick a random living ally (not the original attacker, not dead)
            var eligible = allies
                .Where(a => !a.IsDead && a != originalAttacker)
                .ToList();

            if (eligible.Count == 0)
                return null;

            var dualAttacker = eligible[Random.Range(0, eligible.Count)];

            // Prefer the original target if still alive, otherwise pick a random enemy
            BattleUnit dualTarget = (originalTarget != null && !originalTarget.IsDead)
                ? originalTarget
                : enemies.Where(e => !e.IsDead).OrderBy(_ => Random.value).FirstOrDefault();

            if (dualTarget == null)
                return null;

            Debug.Log($"[Dual Attack] {dualAttacker.Data.heroName} joins the attack!");

            // Execute the dual attacker's S1 with a throwaway SoulManager (no soul gen)
            // and no TurnManager (no CR manipulation from dual attacks)
            SkillResolver.Execute(
                dualAttacker,
                dualAttacker.Data.s1_BasicAttack,
                new List<BattleUnit> { dualTarget },
                enemies,
                new SoulManager(),   // throwaway — dual attacks do not generate Soul
                null,                // no CR manipulation from dual attacks
                isSoulBurn: false);

            return dualAttacker;
        }
    }
}
