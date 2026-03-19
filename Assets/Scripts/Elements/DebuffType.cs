namespace GachaRPG
{
    public enum DebuffType
    {
        None,

        // --- Fire debuffs: Damage over Time ---
        Burn,       // Deals ATK-scaled damage each turn
        Scorch,     // Stacking burn — each stack multiplies DoT damage
        Ignite,     // Burn that also reduces healing received by 50%

        // --- Dark debuffs: Stat Reduction / Weakening ---
        Curse,      // Reduces target ATK by magnitude %
        Blind,      // Reduces target Crit Rate by magnitude %
        Weaken,     // Reduces all damage dealt by target by magnitude %
        Silence,    // Prevents use of S2 and S3 (S1 only)

        // --- Water debuffs: CR Reduction ---
        Chill,      // Reduces CR by magnitude % on application (instant, one-shot)
        Freeze,     // Skips the afflicted unit's next turn entirely

        // --- Earth debuffs: Control ---
        Stun,       // Skips the afflicted unit's next turn
        Provoke     // Forces the afflicted unit to target a specific ally of the applier
    }
}
