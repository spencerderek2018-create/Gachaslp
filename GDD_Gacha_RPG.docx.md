

**GAME DESIGN DOCUMENT**

 

Gacha Slop

Turn-Based Gacha RPG

Version 1.0  —  February 2026

**CONFIDENTIAL**

# **Table of Contents**

# **1\. Executive Summary**

## **1.1 Game Overview**

Gacha Slop is a free-to-play, turn-based gacha RPG designed for mobile platforms (iOS/Android) with potential PC/console ports. The game combines deep tactical combat, hero collection, rich equipment customization, and a narrative-driven campaign with endgame systems designed for long-term player retention.

## **1.2 Core Pillars**

* Strategic Depth — Meaningful turn-based combat where team composition, skill synergies, and equipment builds determine victory, not raw power alone.

* Collection & Progression — A diverse roster of heroes across multiple elements and roles, each with unique skill kits and upgrade paths.

* Narrative Immersion — A fully voiced, chapter-based story mode with branching character arcs, animated cutscenes, and world-building lore.

* Endgame Variety — Boss Rush, Abyss Tower, PvP Arena, and rotating events keep veterans engaged beyond the main campaign.

## **1.3 Target Audience**

Primary: Players aged 18–35 who enjoy gacha RPGs such as Epic Seven, Etheria: Restart, Summoners War, and Genshin Impact. Secondary: Strategy RPG fans seeking tactical depth comparable to Fire Emblem or Final Fantasy Tactics in a mobile-friendly format.

## **1.4 Platform & Monetization**

**Platforms:** iOS, Android (primary); PC via client or emulator (secondary)

**Engine:** Unity or Unreal Engine (TBD based on art direction)

**Monetization:** Free-to-play with gacha summoning, battle pass, cosmetic shop, and optional stamina refills.

# **2\. Core Combat System**

## **2.1 Combat Overview**

Combat is turn-based with a speed-priority system. Each unit’s turn order is determined by their Speed stat, modified by buffs, debuffs, and Combat Readiness (CR) manipulation. Battles take place on a 2D side-view battlefield with up to 4 heroes per team.

## **2.2 Turn Order & Combat Readiness**

Every unit has a Combat Readiness gauge (0–100%). When a unit reaches 100% CR, it takes its turn. CR fills passively based on Speed, but can be pushed or pulled by skills. This creates dynamic turn manipulation as a core strategic layer.

## **2.3 Elements & Affinity**

The game features a 5-element system with an affinity triangle and two neutral elements:

| Element | Strong Against | Weak Against | Role Tendency |
| :---- | :---- | :---- | :---- |
| Fire | Earth | Water | DPS / Debuff |
| Water | Fire | Earth | Tank / Support |
| Earth | Water | Fire | Sustain / Control |
| Light | Dark | Dark | Utility / DPS |
| Dark | Light | Light | Debuff / Assassin |

Difference Between Fire Debuff and Dark Debuff in terms of combat?

Attacking with elemental advantage grants \+15% damage and \+15% chance to trigger the attacker’s elemental bonus effect (e.g., Fire has burn chance, Water has CR reduction chance).

## **2.4 Action Types**

On their turn, a unit can perform one of the following actions:

* Basic Attack (S1) — Always available, no cooldown. Generates Soul resource for the team.

* Skill 2 (S2) — Moderate cooldown (2–4 turns). Stronger effect or utility.

* Skill 3 / Ultimate (S3) — Longer cooldown (4–6 turns) or requires Soul gauge. Powerful team-wide or single-target impact.

* Passive (S4) — Always active or triggers conditionally. Does not consume a turn.

## **2.5 Soul System**

A shared team resource called Souls builds through basic attacks and certain skills. Souls can be spent on powerful Soul Burn variants of skills, providing enhanced effects such as ignoring effect resistance, extra turns, or increased multipliers. Managing Soul economy is a key strategic decision in longer fights.

## **2.6 Dual Attack & Combo System**

When a hero uses their basic attack, there is a base chance (5%) that a random ally will follow up with a Dual Attack. Certain heroes and artifacts increase this chance or guarantee Dual Attacks, enabling combo-focused team compositions.

# **3\. Hero System**

## **3.1 Hero Rarities**

| Rarity | Stars | Base Stat Multiplier | Skill Count | Availability |
| :---- | :---- | :---- | :---- | :---- |
| Common | ★★★ | 1.0x | 3 (S1, S2, Passive) | Story rewards, friendship summon |
| Rare | ★★★★ | 1.15x | 3–4 | Standard banner, selective summon |
| Epic | ★★★★★ | 1.35x | 4 (S1–S4) | Featured banners, pity system |
| Legendary | ★★★★★ (ML) | 1.35x (unique scaling) | 4 (unique kit) | Moonlight summon, spark system |

## **3.2 Hero Roles**

| Role | Primary Function | Stat Priority | Example Archetype |
| :---- | :---- | :---- | :---- |
| Knight (Tank) | Absorb damage, provoke enemies, shield allies | HP, DEF, Effect Resistance | Paladin with team barrier |
| Warrior (DPS) | Deal sustained or burst damage | ATK, Crit Rate, Crit Damage | Berserker with scaling damage |
| Ranger (DPS/Utility) | Flexible attacker with CR manipulation | SPD, ATK, Effectiveness | Archer with speed control |
| Mage (AoE/Control) | Area damage and crowd control | ATK, Effectiveness, SPD | Elementalist with AoE stun |
| Soul Weaver (Healer) | Heal, cleanse, buff allies | HP, SPD, Effect Resistance | Priest with team-wide heal |
| Thief (Assassin) | High single-target burst, evasion | ATK, Crit Damage, SPD | Shadow with stealth \+ execute |

## **3.3 Hero Progression**

* Level Up (1–60): Standard EXP from battles and Penguins (EXP items).

* Awakening (0–6 stars): Consume elemental runes to unlock stat boosts and skill enhancements at each awakening tier.

* Promotion: Sacrifice same-star fodder units to increase a hero’s star rating (e.g., 5★ to 6★), raising stat caps.

* Skill Enhancement: Use Mola (skill currency) to level individual skills, reducing cooldowns or increasing multipliers.

* Exclusive Equipment: At max awakening, heroes unlock a unique Exclusive Equipment slot that modifies one of their skills.

* Imprint (Memory Concentration): Duplicate heroes provide stat bonuses to the team or the hero itself.

# **4\. Equipment System**

## **4.1 Equipment Slots**

Each hero has 6 equipment slots, divided into two categories:

| Slot | Category | Main Stat Options |
| :---- | :---- | :---- |
| Weapon | Left Side | ATK (fixed) |
| Helmet | Left Side | HP (fixed) |
| Armor | Left Side | DEF (fixed) |
| Necklace | Right Side | ATK%, HP%, DEF%, Crit Rate, Crit Damage |
| Ring | Right Side | ATK%, HP%, DEF%, Effectiveness, Effect Resistance |
| Boots | Right Side | ATK%, HP%, DEF%, Speed |

## **4.2 Equipment Sets**

Equipping 2 or 4 pieces of the same set grants a set bonus. Players mix and match sets to optimize builds.

| Set Name | Pieces | Bonus | Source |
| :---- | :---- | :---- | :---- |
| Assault | 4 | \+35% ATK | Wyvern Hunt |
| Vitality | 2 | \+15% HP | Golem Hunt |
| Ironwall | 2 | \+15% DEF | Golem Hunt |
| Velocity | 4 | \+25% Speed | Wyvern Hunt |
| Critical | 2 | \+12% Crit Rate | Azimanak Hunt |
| Destruction | 4 | \+40% Crit Damage | Azimanak Hunt |
| Lifesteal | 4 | \+20% damage dealt healed | Banshee Hunt |
| Counter | 4 | \+20% chance to counter attack | Banshee Hunt |
| Immunity | 2 | Immune to debuffs for 1 turn | Wyvern Hunt |
| Resist | 2 | \+20% Effect Resistance | Golem Hunt |

## **4.3 Substats & Enhancement**

Each equipment piece rolls 1–4 random substats from: ATK, ATK%, HP, HP%, DEF, DEF%, Speed, Crit Rate, Crit Damage, Effectiveness, Effect Resistance. Enhancing gear to \+3/+6/+9/+12/+15 upgrades a random substat each time. Higher-tier gear (Heroic, Epic) starts with more substats, reducing RNG.

## **4.4 Gear Tiers**

| Tier | Starting Substats | Color Code | Drop Source |
| :---- | :---- | :---- | :---- |
| Normal | 0 | Gray | Early story |
| Good | 1 | Green | Mid story, low hunts |
| Rare | 2 | Blue | Hunt stages 7–10 |
| Heroic | 3 | Purple | Hunt stages 11–13 |
| Epic | 4 | Red | Hunt stages 13+, crafting |

## **4.5 Artifacts**

Artifacts are equippable items separate from gear. Each hero can equip one Artifact that provides a passive combat effect (e.g., “15% chance to gain an extra turn after killing an enemy”). Artifacts have their own rarity tiers and are obtained through the gacha summon alongside heroes.

# **5\. Gacha & Summoning System**

## **5.1 Summon Types**

| Banner Type | Currency | Pool | Pity |
| :---- | :---- | :---- | :---- |
| Covenant Summon (Standard) | Bookmarks / Skystones | All non-limited heroes \+ artifacts | Soft pity at 100, hard pity at 121 |
| Featured Banner | Bookmarks | Rate-up for 1 featured Epic hero | Hard pity at 121 (guaranteed featured) |
| Moonlight Summon | Galaxy Bookmarks | Light/Dark exclusive heroes | Hard pity at 200 (Mystic) or spark |
| Friendship Summon | Friendship Points | 3★ heroes, fodder, artifacts | None |
| Limited Banner | Bookmarks | Time-limited hero (never in standard pool) | Hard pity at 121 |

## **5.2 Pity & Safety Net**

Every summon without a 5★ result increments a pity counter. At the soft pity threshold, the 5★ rate increases significantly per pull. At the hard pity threshold, the next pull is guaranteed to be the featured 5★ hero. Pity carries over between banners of the same type. A “Spark” system allows players to select a hero after a set number of pulls on Moonlight banners.

## **5.3 Rates**

| Result | Covenant Rate | Featured Banner Rate |
| :---- | :---- | :---- |
| 5★ Hero | 1.25% | 1.00% (featured) \+ 0.25% (off-banner) |
| 4★ Hero | 4.50% | 4.50% |
| 3★ Hero | 40.25% | 40.25% |
| 5★ Artifact | 1.75% | 1.75% |
| 4★ Artifact | 5.50% | 5.50% |
| 3★ Artifact | 46.75% | 46.75% |

## **5.4 Anti-Frustration Features**

* Selective Summon: New players get a special 30-reroll selective summon after chapter 1–10, choosing from a curated pool.

* Daily Free Summon: One free Covenant summon per day.

* Monthly Galaxy Coins: Login rewards and Abyss Tower provide Moonlight summoning currency.

* Pity Transparency: Pity counters are visible in the UI at all times.

# **6\. Story Campaign**

## **6.1 Narrative Structure**

The main campaign is divided into Chapters, each containing 10 stages with a boss encounter at stage 10\. The story follows the player character (a summoner) who awakens in a world caught in a cycle of destruction and rebirth. Allies are recruited through story progression, and major plot beats are delivered through fully animated cutscenes with voice acting.

## **6.2 Chapter Breakdown**

| Chapter | Region | Theme | Major Unlock |
| :---- | :---- | :---- | :---- |
| Prologue | Sanctuary | Tutorial & world introduction | Basic summoning, team of 4 |
| Ch 1–3 | The Verdant Reach | Political intrigue & elemental awakening | Equipment system, Hunts |
| Ch 4–6 | Ashfall Dominion | War & betrayal arc | Abyss Tower, Guild system |
| Ch 7–9 | Abyssal Rift | Corruption & dark forces revealed | Moonlight summon, World Boss |
| Ch 10+ | Empyrean Gate | Endgame story, cosmic threats | Boss Rush, Expedition |

## **6.3 Difficulty Modes**

* Normal: First playthrough. Balanced for story progression with moderate challenge.

* Hard: Unlocked after completing Normal. Enemies are significantly stronger with new mechanics. Additional story dialogue and alternate scene perspectives.

* Hell (Endgame): Unlocked per-chapter after Hard completion. Provides top-tier gear and exclusive cosmetic rewards.

## **6.4 Side Stories & Events**

Rotating side story events run on 2–4 week cycles, featuring limited-time narratives with their own stages, boss fights, and exclusive reward shops. Seasonal events (holiday-themed, collaboration events) provide unique heroes, artifacts, and cosmetics.

# **7\. Boss Rush**

## **7.1 Overview**

Boss Rush is a high-difficulty endgame mode where players face a gauntlet of increasingly powerful boss encounters back-to-back. Unlike standard stages, Boss Rush tests team endurance, resource management, and adaptability across multiple fights without full recovery between rounds.

## **7.2 Structure**

| Stage | Description | Reward Tier |
| :---- | :---- | :---- |
| Round 1–3 | Mid-difficulty bosses with standard mechanics | Bronze |
| Round 4–6 | Hard bosses with phase transitions and enrage timers | Silver |
| Round 7–9 | Extreme bosses with immunity phases and party-wipe mechanics | Gold |
| Round 10 (Final) | Mythic boss with randomized modifiers | Platinum \+ exclusive rewards |

## **7.3 Mechanics**

* Persistent HP: Hero HP and cooldowns carry over between rounds. Strategic healing and cooldown management are essential.

* 3-Team Roster: Players select 12 heroes before entering. Between rounds, they can swap the active 4-hero team from this roster.

* Boss Modifiers: Each Boss Rush season applies global modifiers (e.g., “All fire damage \+30%”, “Healing reduced by 50%”) that shift the meta.

* Enrage Timer: Bosses enrage after a set number of turns, dramatically increasing damage output.

* Scoring: Players earn a score based on speed, damage taken, and heroes surviving. Leaderboards determine bonus rewards.

## **7.4 Rewards**

Boss Rush rewards include exclusive equipment sets only obtainable through this mode, rare enhancement materials, Moonlight summon currency, and seasonal cosmetic titles. The mode resets weekly with new modifier combinations.

# **8\. Abyss Tower**

## **8.1 Overview**

The Abyss Tower is a permanent, progressively difficult PvE challenge consisting of 120 floors. Each floor presents a unique encounter with specific puzzle-like mechanics that require tailored team compositions. Progress is saved, and floors can be attempted unlimited times. The Tower serves as the primary benchmark for account progression.

## **8.2 Floor Structure**

| Floor Range | Difficulty | Key Mechanic Focus | Notable Rewards |
| :---- | :---- | :---- | :---- |
| 1–20 | Beginner | Basic combat fundamentals | Skystones, Bookmarks |
| 21–40 | Intermediate | Elemental counters, buff management | Galaxy Bookmarks, Gold |
| 41–60 | Advanced | Debuff immunity, damage checks | Epic gear, Mola |
| 61–80 | Expert | Multi-phase bosses, heal checks | Moonlight hero selector (Floor 80\) |
| 81–100 | Master | One-shot mechanics, precise timing | Legendary artifact selector (Floor 100\) |
| 101–120 | Abyss | Combination mechanics, DPS races | Exclusive cosmetics, title, currency |

## **8.3 Design Principles**

* No Hero Reuse: Once a hero clears a floor, they are locked out for subsequent floors within the same 20-floor block, forcing roster diversity.

* Puzzle Encounters: Many floors have gimmick mechanics (e.g., “Boss heals to full if you use AoE attacks,” or “Only debuffed enemies take damage”), rewarding game knowledge.

* Permanent Progress: Cleared floors stay cleared. Rewards are one-time per floor.

* Seasonal Reset Floors: Floors 101–120 reset each season with new bosses and mechanics, providing recurring endgame challenges.

# **9\. Additional Game Modes**

## **9.1 Hunt System (Gear Farming)**

Hunts are repeatable PvE stages that drop equipment. Each Hunt boss is tied to specific gear sets. Hunts scale from level 1–13, with higher levels dropping better gear tiers. Auto-battle and multi-run (up to 20 runs) are available for convenience.

## **9.2 PvP Arena**

* Arena (Asynchronous): Attack AI-controlled defense teams to climb weekly rankings. Rewards include Skystones and Glory Crests for exclusive gear.

* Real-Time Arena (RTA): Live pick-and-ban PvP with seasonal rankings. Each player drafts 5 heroes, bans 1 from the opponent’s team, and fights with the remaining 4\. Provides exclusive cosmetic skins as seasonal rewards.

## **9.3 Guild Content**

* Guild Wars: Guild vs. Guild territory battles. Members attack and defend structures on a hex-grid map. Winning earns Mystic Medals (Moonlight summon currency).

* Guild Boss: A weekly cooperative boss with a shared HP pool. Members contribute damage over multiple attempts. Rewards scale with total guild damage.

## **9.4 World Boss / Expedition**

A server-wide raid boss that all players contribute damage to over 48 hours. Rewards are based on individual contribution percentage. Expeditions are 3-player cooperative bosses with specific element/role requirements, encouraging social play.

## **9.5 Labyrinth**

A grid-based exploration mode where players navigate rooms, fight enemies, collect treasure, and manage morale (a resource that decays with each encounter). Features branching paths, hidden bosses, and unique gear rewards.

# **10\. Progression & Economy**

## **10.1 Currencies**

| Currency | Source | Use |
| :---- | :---- | :---- |
| Gold | Stages, Hunts, selling gear | Gear enhancement, crafting, shop purchases |
| Skystones (Premium) | Story, Arena, achievements, purchase | Summon bookmarks, stamina refills, cosmetics |
| Bookmarks | Skystone exchange, events, login | Covenant/Featured summoning |
| Galaxy Bookmarks | Abyss Tower, Mystic pity, events | Moonlight summoning |
| Mystic Medals | Guild Wars, Secret Shop | Moonlight Mystic summoning |
| Stamina | Natural regen, gifts, purchase | Enter all PvE stages |
| Mola (Molagora) | Weekly shop, events, Tower | Skill enhancement |
| Runes | Spirit Altar stages | Hero awakening |

## **10.2 Stamina & Energy**

Stamina regenerates at 1 point per 5 minutes, with a soft cap of 160\. Overflow stamina from gifts and mail can exceed the cap. Story stages cost 8–12 stamina; Hunts cost 20–25. Stamina can be refilled with Skystones (up to 3 times daily at discounted rates). Boss Rush and Abyss Tower do not consume stamina.

## **10.3 Monetization Details**

* Monthly Pack: $5/month for daily Skystones and bonus stamina. Best value for light spenders.

* Battle Pass: $10/season. Progression-based rewards including bookmarks, Mola, and exclusive cosmetic.

* Skystone Packs: Direct premium currency purchases at various price points ($1–$100).

* Cosmetic Shop: Character skins, UI themes, lobby pets. Purely cosmetic, no gameplay advantage.

* Rank-Up Packs: One-time purchase packs offered at account level milestones (good value, limited quantity).

# **11\. Technical & UX Considerations**

## **11.1 Auto-Battle & QoL**

Auto-battle with adjustable AI priorities (skill usage order, target selection). Multi-run support for farming stages. Skip tickets for previously cleared content (optional, can be a progression unlock). Gear management tools: lock, filter, auto-equip optimizer, substat comparison.

## **11.2 Social Features**

Friend system with support hero lending. Global and guild chat. Player profiles with showcase heroes and achievement display. Mentor system pairing new players with veterans for mutual rewards.

## **11.3 Accessibility**

Colorblind modes for element indicators. Adjustable text size and UI scaling. Full controller support on PC. Adjustable animation speed (1x, 2x, 3x) for combat.

## **11.4 Live Service Cadence**

| Cycle | Content | Frequency |
| :---- | :---- | :---- |
| Weekly | Arena reset, Guild War matchmaking, Boss Rush rotation | Every Monday |
| Bi-weekly | New/rerun hero banner | Every 2 weeks |
| Monthly | Side story event, balance patch, QoL update | 1st of month |
| Seasonal (3 mo.) | New story chapter, RTA season, Abyss 101–120 reset, collab event | Quarterly |
| Anniversary | Major content expansion, free summons, guaranteed selectors | Yearly |

# **12\. Roadmap & Next Steps**

## **12.1 Pre-Production Phase**

* Finalize art direction (2D Live2D vs. 3D chibi vs. hybrid).

* Prototype core combat loop with 6–8 placeholder heroes across all roles.

* Design and test gacha economy simulation (pull rates, currency generation, spending projections).

* Write narrative outline for Chapters 1–10 and key character arcs.

## **12.2 Key Decisions Pending**

| Decision | Options | Impact |
| :---- | :---- | :---- |
| Art Style | 2D Live2D (Epic 7 style) vs. 3D (Etheria style) | Defines pipeline, budget, and audience perception |
| PvP Model | Asynchronous only vs. Real-Time Arena at launch | Server infrastructure, balance priority |
| Engine | Unity (faster iteration) vs. Unreal (visual fidelity) | Team hiring, platform performance |
| Regional Launch | Global simultaneous vs. soft launch in SEA/KR first | Localization scope, server architecture |

## **12.3 Success Metrics**

* Day 1 Retention: Target 45%+

* Day 7 Retention: Target 25%+

* Day 30 Retention: Target 12%+

* ARPDAU: $0.15–$0.30 at scale

* Conversion Rate (F2P to Payer): 3–5%

 

*End of Document*