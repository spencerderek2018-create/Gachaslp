namespace GachaRPG
{
    public enum HeroRole
    {
        Knight,
        Warrior,
        Ranger,
        Mage,
        SoulWeaver,
        Thief
    }

    public enum HeroRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public enum SkillSlot
    {
        S1,
        S2,
        S3,
        S4_Passive
    }

    public enum EffectType
    {
        Damage,
        Heal,
        ApplyDebuff,
        ApplyBuff,
        PushCR,
        PullCR,
        GrantExtraTurn,
        SoulBurn
    }

    public enum TargetType
    {
        SingleEnemy,
        AllEnemies,
        SingleAlly,
        AllAllies,
        Self
    }

    public enum StatType
    {
        HP,
        ATK,
        DEF,
        Speed,
        CritRate,
        CritDamage,
        Effectiveness,
        EffectResist
    }
}
