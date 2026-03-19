using UnityEngine;
using GachaRPG;

namespace GachaRPG
{
    public enum ArtifactRarity { Rare, Epic, Legendary }

    public enum ArtifactEffectType
    {
        ExtraTurnOnKill,           // % chance to gain extra turn after killing an enemy
        DualAttackChanceBonus,     // flat % added to dual attack chance
        SoulGenerationBonus,       // extra soul generated per S1
        HealOnCrit,                // heals self for % of damage dealt on crit
        ShieldOnBattleStart,       // grants a shield at the start of battle
        CRPushOnSkillUse,          // pushes CR by % when using S2 or S3
        DebuffChanceBonus,         // flat % added to all debuff land chances
        DamageReductionVsAoE,      // reduces damage taken from AoE skills
    }

    [CreateAssetMenu(menuName = "GachaRPG/Artifact", fileName = "NewArtifact")]
    public class ArtifactData : ScriptableObject
    {
        [Header("Identity")]
        public string artifactName;
        [TextArea(2, 4)]
        public string description;
        public ArtifactRarity rarity;

        [Header("Passive Effect")]
        public ArtifactEffectType effectType;
        public float effectMagnitude; // Meaning depends on effectType (e.g. 0.15 = 15%)

        [Header("Enhancement")]
        [Tooltip("Magnitude at max enhancement level")]
        public float maxEnhanceMagnitude;
        public int currentEnhanceLevel; // 0–15
    }
}
