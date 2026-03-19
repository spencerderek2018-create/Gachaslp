using System;
using GachaRPG;

namespace GachaRPG
{
    [Serializable]
    public class SkillEffect
    {
        public EffectType type;
        public float multiplier;         // damage = ATK * multiplier, or heal = ATK * multiplier
        public TargetType target;
        public DebuffType debuffToApply; // None if not a debuff skill
        public float debuffChance;       // 0.0 – 1.0 base land chance
        public int debuffDuration;       // in turns
        public float crPushAmount;       // 0.0–1.0 (proportion of CR bar); positive = push, negative = pull
    }
}
