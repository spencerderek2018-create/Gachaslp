namespace GachaRPG
{
    public enum GearSlot
    {
        Weapon,   // Left side — fixed main stat: ATK
        Helmet,   // Left side — fixed main stat: HP
        Armor,    // Left side — fixed main stat: DEF
        Necklace, // Right side — flexible main stat
        Ring,     // Right side — flexible main stat
        Boots     // Right side — flexible main stat
    }

    public enum GearTier
    {
        Normal  = 0, // Gray  — 0 starting substats
        Good    = 1, // Green — 1 starting substat
        Rare    = 2, // Blue  — 2 starting substats
        Heroic  = 3, // Purple — 3 starting substats
        Epic    = 4  // Red   — 4 starting substats
    }

    public enum GearSet
    {
        Assault,     // 4pc: +35% ATK
        Vitality,    // 2pc: +15% HP
        Ironwall,    // 2pc: +15% DEF
        Velocity,    // 4pc: +25% Speed
        Critical,    // 2pc: +12% Crit Rate
        Destruction, // 4pc: +40% Crit Damage
        Lifesteal,   // 4pc: +20% damage dealt healed
        Counter,     // 4pc: +20% chance to counterattack
        Immunity,    // 2pc: Immune to first debuff each turn
        Resist       // 2pc: +20% Effect Resistance
    }
}
