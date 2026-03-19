namespace GachaRPG
{
    /// <summary>
    /// Classifies debuffs into elemental categories and mechanical types.
    ///
    /// Fire debuffs   = Damage over Time  — stronger vs Earth targets
    /// Dark debuffs   = Stat Reduction    — stronger vs Light targets
    /// Water debuffs  = CR Reduction      — stronger vs Fire targets (Water > Fire)
    /// Earth debuffs  = Control/Stun      — stronger vs Water targets (Earth > Water)
    /// </summary>
    public static class DebuffCategory
    {
        public static bool IsDoT(DebuffType type) =>
            type == DebuffType.Burn   ||
            type == DebuffType.Scorch ||
            type == DebuffType.Ignite;

        public static bool IsStatReduction(DebuffType type) =>
            type == DebuffType.Curse   ||
            type == DebuffType.Blind   ||
            type == DebuffType.Weaken  ||
            type == DebuffType.Silence;

        public static bool IsCRReduction(DebuffType type) =>
            type == DebuffType.Chill  ||
            type == DebuffType.Freeze;

        public static bool IsControl(DebuffType type) =>
            type == DebuffType.Stun    ||
            type == DebuffType.Provoke;

        /// <summary>
        /// Returns the element that "owns" this debuff type.
        /// Used to apply elemental advantage bonus to debuff landing chance.
        /// </summary>
        public static ElementType GetDebuffElement(DebuffType type)
        {
            if (IsDoT(type))          return ElementType.Fire;
            if (IsStatReduction(type)) return ElementType.Dark;
            if (IsCRReduction(type))  return ElementType.Water;
            if (IsControl(type))      return ElementType.Earth;
            return ElementType.Fire;
        }
    }
}
