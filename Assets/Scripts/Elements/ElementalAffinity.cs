namespace GachaRPG
{
    /// <summary>
    /// Handles elemental damage and debuff landing modifiers.
    /// Advantage grants +15% damage and +15% debuff land chance.
    ///
    /// Triangle:  Fire > Earth > Water > Fire
    /// Neutrals:  Light > Dark, Dark > Light
    /// </summary>
    public static class ElementalAffinity
    {
        // [attacker, defender] — index matches ElementType enum values
        private static readonly float[,] affinityMatrix = new float[5, 5]
        {
            //           Fire   Water  Earth  Light  Dark    (defender)
            /* Fire  */ { 1.0f, 0.85f, 1.15f, 1.0f,  1.0f  },
            /* Water */ { 1.15f, 1.0f, 0.85f, 1.0f,  1.0f  },
            /* Earth */ { 0.85f, 1.15f, 1.0f, 1.0f,  1.0f  },
            /* Light */ { 1.0f,  1.0f,  1.0f, 1.0f,  1.15f },
            /* Dark  */ { 1.0f,  1.0f,  1.0f, 1.15f, 1.0f  },
        };

        /// <summary>
        /// Returns the damage multiplier for attacking element vs defending element.
        /// 1.15 = advantage, 0.85 = disadvantage, 1.0 = neutral.
        /// </summary>
        public static float GetMultiplier(ElementType attacker, ElementType defender)
        {
            return affinityMatrix[(int)attacker, (int)defender];
        }

        /// <summary>Returns true if the attacker has elemental advantage over the defender.</summary>
        public static bool HasAdvantage(ElementType attacker, ElementType defender)
        {
            return GetMultiplier(attacker, defender) > 1.0f;
        }

        /// <summary>
        /// Flat bonus added to debuff land chance when the debuff's element has advantage
        /// over the target's element.
        /// </summary>
        public const float DebuffChanceBonus = 0.15f;
    }
}
