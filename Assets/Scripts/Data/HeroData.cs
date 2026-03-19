using UnityEngine;
using GachaRPG;

namespace GachaRPG
{
    [CreateAssetMenu(menuName = "GachaRPG/Hero", fileName = "NewHero")]
    public class HeroData : ScriptableObject
    {
        [Header("Identity")]
        public string heroName;
        public ElementType element;
        public HeroRole role;
        public HeroRarity rarity;

        [Header("Base Stats")]
        public int baseHP;
        public int baseATK;
        public int baseDEF;
        public int baseSpeed;
        [Range(0f, 1f)] public float baseCritRate    = 0.15f;
        public float baseCritDamage                  = 1.5f;
        [Range(0f, 1f)] public float baseEffectiveness = 0f;
        [Range(0f, 1f)] public float baseEffectResist  = 0f;

        [Header("Skills")]
        public SkillData s1_BasicAttack;
        public SkillData s2;
        public SkillData s3_Ultimate;
        public SkillData s4_Passive;   // Passive — not directly executed, applied at unit init

        [Header("Artifact")]
        public ArtifactData defaultArtifact;

        [Header("Progression")]
        [Range(0, 6)] public int awakeningLevel = 0;
        [Range(1, 60)] public int level = 1;
    }
}
