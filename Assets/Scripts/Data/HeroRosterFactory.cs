#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace GachaRPG
{
    /// <summary>
    /// Editor utility that generates placeholder HeroData and SkillData ScriptableObject
    /// assets for all 6 roles — one hero per role — for use in the combat prototype.
    ///
    /// Usage: Unity menu bar → GachaRPG → Create Placeholder Heroes
    ///
    /// All assets are written to Assets/Data/Heroes/ and Assets/Data/Skills/.
    /// Replace or extend these assets once the real hero roster is designed.
    /// </summary>
    public static class HeroRosterFactory
    {
        private const string HeroPath = "Assets/Data/Heroes";
        private const string SkillPath = "Assets/Data/Skills";

        [MenuItem("GachaRPG/Create Placeholder Heroes")]
        public static void CreatePlaceholderHeroes()
        {
            Directory.CreateDirectory(HeroPath);
            Directory.CreateDirectory(SkillPath);

            CreateKnight();
            CreateWarrior();
            CreateRanger();
            CreateMage();
            CreateSoulWeaver();
            CreateThief();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[HeroRosterFactory] 6 placeholder heroes created in Assets/Data/Heroes/");
        }

        // ----------------------------------------------------------------
        // Knight (Tank) — Fire element
        // High HP/DEF, provoke, team shield
        // ----------------------------------------------------------------
        private static void CreateKnight()
        {
            var s1 = CreateSkill("Knight_S1_ShieldBash", "Shield Bash",
                "Strikes an enemy with shield. Generates 1 Soul.", SkillSlot.S1,
                cooldown: 0, soulGen: 1,
                new SkillEffect
                {
                    type = EffectType.Damage,
                    multiplier = 0.9f,
                    target = TargetType.SingleEnemy
                });

            var s2 = CreateSkill("Knight_S2_Provoke", "Taunt",
                "Taunts an enemy, forcing them to attack this unit for 2 turns.", SkillSlot.S2,
                cooldown: 3, soulGen: 0,
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff,
                    debuffToApply = DebuffType.Provoke,
                    debuffChance = 0.8f,
                    debuffDuration = 2,
                    multiplier = 0f,
                    target = TargetType.SingleEnemy
                });

            var s3 = CreateSkill("Knight_S3_IronFortress", "Iron Fortress",
                "Deals damage to all enemies and pushes CR of all allies by 15%.", SkillSlot.S3,
                cooldown: 5, soulGen: 0,
                new SkillEffect
                {
                    type = EffectType.Damage,
                    multiplier = 0.7f,
                    target = TargetType.AllEnemies
                },
                new SkillEffect
                {
                    type = EffectType.PushCR,
                    crPushAmount = 0.15f,
                    target = TargetType.AllAllies
                });

            CreateHero("Placeholder_Knight", "Iron Paladin", ElementType.Fire, HeroRole.Knight, HeroRarity.Epic,
                hp: 7500, atk: 900, def: 700, spd: 95,
                critRate: 0.15f, critDmg: 1.5f, effectiveness: 0.3f, resistance: 0.5f,
                s1, s2, s3);
        }

        // ----------------------------------------------------------------
        // Warrior (DPS) — Fire element
        // High ATK, scaling damage, Burn application
        // ----------------------------------------------------------------
        private static void CreateWarrior()
        {
            var s1 = CreateSkill("Warrior_S1_Slash", "Blazing Slash",
                "Slashes an enemy. Has a 50% chance to Burn the target for 2 turns.", SkillSlot.S1,
                cooldown: 0, soulGen: 1,
                new SkillEffect
                {
                    type = EffectType.Damage,
                    multiplier = 1.0f,
                    target = TargetType.SingleEnemy
                },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff,
                    debuffToApply = DebuffType.Burn,
                    debuffChance = 0.5f,
                    debuffDuration = 2,
                    multiplier = 0.15f, // DoT = source ATK * 0.15
                    target = TargetType.SingleEnemy
                });

            var s2 = CreateSkill("Warrior_S2_BurningStrike", "Burning Strike",
                "Deals heavy damage. Inflicts Scorch (stacking burn).", SkillSlot.S2,
                cooldown: 3, soulGen: 0,
                new SkillEffect
                {
                    type = EffectType.Damage,
                    multiplier = 1.6f,
                    target = TargetType.SingleEnemy
                },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff,
                    debuffToApply = DebuffType.Scorch,
                    debuffChance = 0.7f,
                    debuffDuration = 3,
                    multiplier = 0.10f,
                    target = TargetType.SingleEnemy
                });

            var s3 = CreateSkill("Warrior_S3_Inferno", "Inferno",
                "Unleashes a massive fire strike on a single enemy. Soul Burn: also ignites all enemies.", SkillSlot.S3,
                cooldown: 5, soulGen: 0,
                new SkillEffect
                {
                    type = EffectType.Damage,
                    multiplier = 2.8f,
                    target = TargetType.SingleEnemy
                });

            s3.hasSoulBurn    = true;
            s3.soulBurnCost   = 3;
            s3.soulBurnEffects = new[]
            {
                new SkillEffect { type = EffectType.Damage,      multiplier = 2.8f,  target = TargetType.SingleEnemy },
                new SkillEffect { type = EffectType.ApplyDebuff, debuffToApply = DebuffType.Ignite,
                                  debuffChance = 1.0f, debuffDuration = 2, multiplier = 0.12f,
                                  target = TargetType.AllEnemies }
            };

            CreateHero("Placeholder_Warrior", "Ember Berserker", ElementType.Fire, HeroRole.Warrior, HeroRarity.Epic,
                hp: 5800, atk: 1500, def: 400, spd: 100,
                critRate: 0.25f, critDmg: 1.7f, effectiveness: 0.4f, resistance: 0.1f,
                s1, s2, s3);
        }

        // ----------------------------------------------------------------
        // Ranger (DPS/Utility) — Water element
        // Speed-focused, CR pull, Chill debuff
        // ----------------------------------------------------------------
        private static void CreateRanger()
        {
            var s1 = CreateSkill("Ranger_S1_SwiftShot", "Swift Shot",
                "Fires a quick arrow. 40% chance to Chill the target (pulls CR).", SkillSlot.S1,
                cooldown: 0, soulGen: 1,
                new SkillEffect
                {
                    type = EffectType.Damage,
                    multiplier = 0.85f,
                    target = TargetType.SingleEnemy
                },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff,
                    debuffToApply = DebuffType.Chill,
                    debuffChance = 0.4f,
                    debuffDuration = 1,
                    multiplier = 0.20f, // pulls CR by 20%
                    target = TargetType.SingleEnemy
                });

            var s2 = CreateSkill("Ranger_S2_CRDrain", "Drain Flow",
                "Pulls the target's CR back by 30% and pushes the caster's CR by 15%.", SkillSlot.S2,
                cooldown: 3, soulGen: 0,
                new SkillEffect { type = EffectType.Damage,  multiplier = 1.0f, target = TargetType.SingleEnemy },
                new SkillEffect { type = EffectType.PullCR,  crPushAmount = 0.30f, target = TargetType.SingleEnemy },
                new SkillEffect { type = EffectType.PushCR,  crPushAmount = 0.15f, target = TargetType.Self });

            var s3 = CreateSkill("Ranger_S3_TidalBarrage", "Tidal Barrage",
                "Fires at all enemies, Chilling each one. Grants the team an extra turn.", SkillSlot.S3,
                cooldown: 5, soulGen: 0,
                new SkillEffect { type = EffectType.Damage, multiplier = 0.9f, target = TargetType.AllEnemies },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff, debuffToApply = DebuffType.Chill,
                    debuffChance = 0.6f, debuffDuration = 1, multiplier = 0.25f,
                    target = TargetType.AllEnemies
                });

            CreateHero("Placeholder_Ranger", "Tidal Archer", ElementType.Water, HeroRole.Ranger, HeroRarity.Epic,
                hp: 5200, atk: 1200, def: 350, spd: 120,
                critRate: 0.20f, critDmg: 1.6f, effectiveness: 0.5f, resistance: 0.1f,
                s1, s2, s3);
        }

        // ----------------------------------------------------------------
        // Mage (AoE/Control) — Earth element
        // AoE damage, Stun, crowd control
        // ----------------------------------------------------------------
        private static void CreateMage()
        {
            var s1 = CreateSkill("Mage_S1_RockShard", "Rock Shard",
                "Hurls a shard of earth at an enemy.", SkillSlot.S1,
                cooldown: 0, soulGen: 1,
                new SkillEffect { type = EffectType.Damage, multiplier = 0.95f, target = TargetType.SingleEnemy });

            var s2 = CreateSkill("Mage_S2_Tremor", "Tremor",
                "Strikes all enemies with a shockwave, 50% chance to Stun each.", SkillSlot.S2,
                cooldown: 4, soulGen: 0,
                new SkillEffect { type = EffectType.Damage, multiplier = 0.8f, target = TargetType.AllEnemies },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff, debuffToApply = DebuffType.Stun,
                    debuffChance = 0.5f, debuffDuration = 1, multiplier = 0f,
                    target = TargetType.AllEnemies
                });

            var s3 = CreateSkill("Mage_S3_Earthquake", "Earthquake",
                "Massive AoE. Silences all enemies for 1 turn and deals heavy damage.", SkillSlot.S3,
                cooldown: 5, soulGen: 0,
                new SkillEffect { type = EffectType.Damage, multiplier = 1.4f, target = TargetType.AllEnemies },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff, debuffToApply = DebuffType.Silence,
                    debuffChance = 0.7f, debuffDuration = 1, multiplier = 0f,
                    target = TargetType.AllEnemies
                });

            CreateHero("Placeholder_Mage", "Stone Elementalist", ElementType.Earth, HeroRole.Mage, HeroRarity.Epic,
                hp: 5000, atk: 1400, def: 380, spd: 105,
                critRate: 0.15f, critDmg: 1.5f, effectiveness: 0.6f, resistance: 0.1f,
                s1, s2, s3);
        }

        // ----------------------------------------------------------------
        // Soul Weaver (Healer) — Water element
        // Heal, cleanse, CR push support
        // ----------------------------------------------------------------
        private static void CreateSoulWeaver()
        {
            var s1 = CreateSkill("SoulWeaver_S1_HealingLight", "Healing Touch",
                "Attacks an enemy and heals the ally with the lowest HP.", SkillSlot.S1,
                cooldown: 0, soulGen: 1,
                new SkillEffect { type = EffectType.Damage, multiplier = 0.7f, target = TargetType.SingleEnemy },
                new SkillEffect { type = EffectType.Heal,   multiplier = 0.5f, target = TargetType.SingleAlly });

            var s2 = CreateSkill("SoulWeaver_S2_Mend", "Mend",
                "Heals all allies and pushes their CR by 10%.", SkillSlot.S2,
                cooldown: 3, soulGen: 0,
                new SkillEffect { type = EffectType.Heal,   multiplier = 0.6f,  target = TargetType.AllAllies },
                new SkillEffect { type = EffectType.PushCR, crPushAmount = 0.10f, target = TargetType.AllAllies });

            var s3 = CreateSkill("SoulWeaver_S3_MassRestoration", "Mass Restoration",
                "Fully restores a large amount of HP to all allies and grants each an extra turn.", SkillSlot.S3,
                cooldown: 5, soulGen: 0,
                new SkillEffect { type = EffectType.Heal,         multiplier = 1.2f, target = TargetType.AllAllies },
                new SkillEffect { type = EffectType.GrantExtraTurn, target = TargetType.Self });

            CreateHero("Placeholder_SoulWeaver", "Tide Priest", ElementType.Water, HeroRole.SoulWeaver, HeroRarity.Epic,
                hp: 6200, atk: 1000, def: 500, spd: 115,
                critRate: 0.10f, critDmg: 1.3f, effectiveness: 0.2f, resistance: 0.6f,
                s1, s2, s3);
        }

        // ----------------------------------------------------------------
        // Thief (Assassin) — Dark element
        // Burst damage, Curse/Blind debuffs, execute
        // ----------------------------------------------------------------
        private static void CreateThief()
        {
            var s1 = CreateSkill("Thief_S1_PoisonBlade", "Cursed Blade",
                "Strikes an enemy. 40% chance to Curse them (reduces ATK).", SkillSlot.S1,
                cooldown: 0, soulGen: 1,
                new SkillEffect { type = EffectType.Damage, multiplier = 1.0f, target = TargetType.SingleEnemy },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff, debuffToApply = DebuffType.Curse,
                    debuffChance = 0.4f, debuffDuration = 2, multiplier = 0.20f,
                    target = TargetType.SingleEnemy
                });

            var s2 = CreateSkill("Thief_S2_ShadowStrike", "Shadow Strike",
                "Deals massive single-target damage. Blinds the target (reduces Crit Rate).", SkillSlot.S2,
                cooldown: 3, soulGen: 0,
                new SkillEffect { type = EffectType.Damage, multiplier = 1.8f, target = TargetType.SingleEnemy },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff, debuffToApply = DebuffType.Blind,
                    debuffChance = 0.75f, debuffDuration = 2, multiplier = 0.30f,
                    target = TargetType.SingleEnemy
                });

            var s3 = CreateSkill("Thief_S3_NightmareFinisher", "Nightmare Finisher",
                "Executes an enemy — deals 3x damage to targets below 30% HP. Weakens on hit.", SkillSlot.S3,
                cooldown: 5, soulGen: 0,
                new SkillEffect { type = EffectType.Damage, multiplier = 2.5f, target = TargetType.SingleEnemy },
                new SkillEffect
                {
                    type = EffectType.ApplyDebuff, debuffToApply = DebuffType.Weaken,
                    debuffChance = 0.9f, debuffDuration = 2, multiplier = 0.25f,
                    target = TargetType.SingleEnemy
                });

            CreateHero("Placeholder_Thief", "Shadow Reaper", ElementType.Dark, HeroRole.Thief, HeroRarity.Epic,
                hp: 4800, atk: 1600, def: 300, spd: 118,
                critRate: 0.35f, critDmg: 2.0f, effectiveness: 0.45f, resistance: 0.1f,
                s1, s2, s3);
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        private static SkillData CreateSkill(
            string assetName, string skillName, string description,
            SkillSlot slot, int cooldown, int soulGen,
            params SkillEffect[] effects)
        {
            string path = $"{SkillPath}/{assetName}.asset";

            var existing = AssetDatabase.LoadAssetAtPath<SkillData>(path);
            if (existing != null) return existing;

            var skill = ScriptableObject.CreateInstance<SkillData>();
            skill.skillName      = skillName;
            skill.description    = description;
            skill.slot           = slot;
            skill.cooldownTurns  = cooldown;
            skill.soulGenerated  = soulGen;
            skill.effects        = effects;

            AssetDatabase.CreateAsset(skill, path);
            return skill;
        }

        private static void CreateHero(
            string assetName, string heroName,
            ElementType element, HeroRole role, HeroRarity rarity,
            int hp, int atk, int def, int spd,
            float critRate, float critDmg, float effectiveness, float resistance,
            SkillData s1, SkillData s2, SkillData s3)
        {
            string path = $"{HeroPath}/{assetName}.asset";

            if (AssetDatabase.LoadAssetAtPath<HeroData>(path) != null)
            {
                Debug.Log($"[HeroRosterFactory] {assetName} already exists, skipping.");
                return;
            }

            var hero = ScriptableObject.CreateInstance<HeroData>();
            hero.heroName          = heroName;
            hero.element           = element;
            hero.role              = role;
            hero.rarity            = rarity;
            hero.baseHP            = hp;
            hero.baseATK           = atk;
            hero.baseDEF           = def;
            hero.baseSpeed         = spd;
            hero.baseCritRate      = critRate;
            hero.baseCritDamage    = critDmg;
            hero.baseEffectiveness = effectiveness;
            hero.baseEffectResist  = resistance;
            hero.s1_BasicAttack    = s1;
            hero.s2                = s2;
            hero.s3_Ultimate       = s3;

            AssetDatabase.CreateAsset(hero, path);
            Debug.Log($"[HeroRosterFactory] Created {heroName} at {path}");
        }
    }
}
#endif
