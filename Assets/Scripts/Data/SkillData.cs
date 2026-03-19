using UnityEngine;
using GachaRPG;

namespace GachaRPG
{
    [CreateAssetMenu(menuName = "GachaRPG/Skill", fileName = "NewSkill")]
    public class SkillData : ScriptableObject
    {
        [Header("Identity")]
        public string skillName;
        [TextArea(2, 4)]
        public string description;
        public SkillSlot slot;

        [Header("Cooldown & Cost")]
        public int cooldownTurns;    // 0 = no cooldown (S1)
        public int soulCost;         // > 0 means requires Soul gauge to activate
        public int soulGenerated;    // Soul added to team gauge when this skill is used

        [Header("Effects")]
        public SkillEffect[] effects;

        [Header("Soul Burn Variant")]
        public bool hasSoulBurn;
        public int soulBurnCost;
        public SkillEffect[] soulBurnEffects;
    }
}
