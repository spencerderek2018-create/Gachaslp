# My Understanding of Gacha Slop — GDD Analysis

## What This Game Is

**Gacha Slop** is a free-to-play mobile turn-based RPG built around hero collection (gacha), tactical combat, and long-term progression loops. It's explicitly modeled after games like **Epic Seven**, **Summoners War**, and **Genshin Impact** — the design borrows heavily from E7 in particular (Combat Readiness, Soul system, Hunts, Molagora, artifact system, and even the pity structure are near-identical).

The core fantasy: collect a roster of heroes, build them with equipment, and field teams of 4 to overcome increasingly complex combat puzzles.

---

## Combat — The Core Loop

### How Turns Work
The battle system uses a **Combat Readiness (CR) gauge** rather than a fixed turn order. Every unit fills their gauge based on Speed, and whoever hits 100% CR goes next. This means:
- Speed is arguably the most important stat in the game
- Skills can push/pull CR, making turn manipulation a key strategic axis
- A fast team can "lap" a slow one, acting multiple times before the opponent gets a turn

This is directly lifted from Epic Seven's system.

### The 4 Skills per Hero
Each hero has:
- **S1** (Basic Attack) — no cooldown, generates Soul resource
- **S2** — moderate cooldown, stronger/utility effect
- **S3/Ultimate** — long cooldown or requires Soul, high impact
- **S4 (Passive)** — always active or conditional trigger

### Soul System
Soul is a shared team resource built through S1 usage. Spending it activates **Soul Burn** — an enhanced version of a skill (e.g., ignore effect resistance, gain extra turn, higher multiplier). This adds a resource management layer on top of turn management.

### Dual Attack
5% base chance that any basic attack triggers a random ally to follow up. Certain heroes/artifacts can increase this or guarantee it, enabling "dual attack team" archetypes. This is a direct parallel to E7's chain attack system.

### Elements
5 elements in a triangle + two neutral:
- Fire > Earth > Water > Fire
- Light and Dark counter each other
- Advantage = +15% damage + higher chance to proc elemental bonus effects

**Debuff Elemental Typing (Resolved):**
Debuffs carry elemental typing that follows the affinity system:
- **Fire debuffs** (Burn, Scorch, DoT effects) land with increased chance/effectiveness against **Earth** enemies, reduced against **Water**
- **Dark debuffs** (Curse, Blind, stat reduction effects) land with increased chance/effectiveness against **Light** enemies, and vice versa

Two layers to this system:
1. **Elemental typing** — debuff landing/effectiveness is affected by the caster's element vs. the target's element, same as damage affinity
2. **Mechanical category** — Fire debuffs = damage over time; Dark debuffs = stat reduction / weakening effects

---

## Heroes — Collection & Progression

### Rarity Tiers
- **3★ Common** — story/friendship summon fodder
- **4★ Rare** — standard pool, good units
- **5★ Epic** — the main gacha targets, full 4-skill kits
- **5★ Legendary (ML)** — special Light/Dark pool, unique scaling, most powerful

### 6 Roles
| Role | What They Do |
|------|-------------|
| Knight | Tank, provoke, shields |
| Warrior | Sustained/burst DPS |
| Ranger | Flexible attacker + CR manipulation |
| Mage | AoE damage + crowd control |
| Soul Weaver | Healer/cleanser/buffer |
| Thief | Single-target burst, evasion, assassin |

### Progression Path (Per Hero)
1. Level to 60 (EXP from battles or Penguin items)
2. Awaken 0→6 stars (elemental runes unlock stat boosts + skill enhancements)
3. Promote (sacrifice fodder to raise star rating beyond base)
4. Skill Enhance with Mola currency (reduce cooldowns, increase multipliers)
5. Equip Exclusive Equipment at max awakening (modifies one specific skill)
6. Imprint via duplicates (stat bonus to self or team)

This is a deep progression system — a single hero can take weeks or months to fully build.

---

## Equipment — The Gear Grind

### 6 Slots
Left side (fixed main stats): Weapon (ATK), Helmet (HP), Armor (DEF)
Right side (flexible main stats): Necklace, Ring, Boots — these can roll ATK%, HP%, DEF%, and offensive/defensive secondaries

### Set Bonuses (2 or 4 pieces)
Key sets and their meta implications:
- **Velocity (4pc)** — +25% Speed, dominant on support/control units
- **Assault (4pc)** — +35% ATK, standard DPS set
- **Destruction (4pc)** — +40% Crit Damage, glass cannon builds
- **Counter (4pc)** — +20% counter chance, enables counter-attack archetypes
- **Lifesteal (4pc)** — sustain for solo carries
- **Immunity (2pc)** — 1-turn debuff immunity, critical in debuff-heavy content

### Substats & RNG
Each piece rolls 1–4 substats. Enhancement at +3/+6/+9/+12/+15 upgrades a **random** substat. This is the primary grind driver — getting the right substats on the right piece is intentionally rare. Epic (red) gear starts with 4 substats, reducing some RNG.

### Gear Tiers
Gray → Green → Blue → Purple → Red (Epic), dropping from progressively harder Hunt stages.

### Artifacts
Separate from gear — one per hero, passive combat effect. Also obtained through gacha pulls. Another collection axis on top of heroes.

---

## Gacha & Economy

### Summon Types
- **Covenant (Standard)** — bookmarks, all non-limited heroes
- **Featured Banner** — rate-up for 1 Epic hero
- **Moonlight (ML)** — Galaxy Bookmarks, Light/Dark exclusive heroes
- **Friendship** — free, low-tier rewards
- **Limited** — time-gated heroes, never re-added to standard pool

### Rates
- 5★ Hero: 1.25% (Covenant) / 1.00% featured + 0.25% off-banner (Featured)
- Soft pity at 100 pulls, hard pity at 121 (guaranteed featured)
- Moonlight hard pity at 200, with Spark system for hero selection

These are low rates. 121 pulls at ~$1/pull = ~$121 per guaranteed featured hero. This is intentional — the business model depends on it.

### Anti-Frustration Features
- Selective Summon (30 rerolls after early chapters) for new players
- Daily free Covenant summon
- Visible pity counters
- Galaxy Coins from login/Abyss Tower

---

## Game Modes — Content Pillars

### Story Campaign
- 10+ Chapters, each with 10 stages + boss
- Normal → Hard → Hell difficulty progression
- Animated cutscenes, voice acting
- Unlocks new systems as chapters progress (Hunts unlock at Ch1–3, Moonlight at Ch7–9, etc.)

### Hunts (Gear Farming)
- Repeatable PvE boss stages, each tied to specific gear sets
- Scale 1–13 in difficulty
- Auto-battle + multi-run (up to 20 runs) for efficiency
- **This is where the majority of playtime goes** for endgame players

### Abyss Tower (120 Floors)
- Permanent, progressive PvE puzzle challenge
- No hero reuse within 20-floor blocks — forces roster breadth
- Puzzle mechanics per floor (gimmicks that require specific solutions)
- Major milestones: Floor 80 = Moonlight hero selector, Floor 100 = Legendary artifact selector
- Floors 101–120 reset seasonally

### Boss Rush
- Endgame gauntlet, 10 rounds of escalating bosses
- Persistent HP/cooldowns between rounds
- 3-team roster (12 heroes), swap between rounds
- Weekly rotating modifiers shift the meta
- Scoring system → leaderboard rewards

### PvP Arena
- **Async Arena**: Attack AI defense teams, weekly rankings
- **Real-Time Arena (RTA)**: Live 5-hero draft with 1-ban, seasonal skins as rewards

### Guild Content
- **Guild Wars**: Hex-grid territory battles, earns Moonlight currency
- **Guild Boss**: Shared HP cooperative boss, scales with guild damage

### World Boss / Expedition
- Server-wide 48-hour raid, rewards by contribution %
- 3-player cooperative Expeditions with role requirements

### Labyrinth
- Grid-based dungeon exploration
- Morale resource that decays per encounter
- Branching paths, hidden bosses, unique rewards

---

## Economy & Monetization

### Currencies (8 total)
Gold, Skystones (premium), Bookmarks, Galaxy Bookmarks, Mystic Medals, Stamina, Mola, Runes — each serves a specific purpose and creates separate progression bottlenecks.

### Stamina System
- 1 per 5 minutes, soft cap 160
- Story: 8–12 per stage; Hunts: 20–25 per run
- 3 refills/day with Skystones
- **Boss Rush and Abyss Tower are stamina-free** — smart design keeping high-engagement content freely accessible
---

## Live Service Cadence

| Cycle | Content |
|-------|---------|
| Weekly | Arena reset, Guild War, Boss Rush rotation |
| Bi-weekly | New/rerun hero banner |
| Monthly | Side story, balance patch, QoL |
| Quarterly | New chapter, RTA season, Abyss 101–120 reset, collab event |
| Yearly | Major expansion, free summons, selectors |

---

## Technical Considerations

- Auto-battle with adjustable AI priorities
- Multi-run farming, skip tickets
- Gear lock/filter/auto-equip optimizer
- Controller support on PC
- 1x/2x/3x animation speed
- Colorblind mode, text scaling

---

## Resolved Decisions

| Decision | Answer |
|----------|--------|
| Engine | Unity |
| Art Style | Hybrid (2D + 3D) |
| Game Title | Placeholder — "Gacha Slop" is not the final name |
| Monetization | Too early to define — not in scope yet |
| Next Priority | Combat prototype first, then hero roster |

## Open Questions & Gaps in the GDD

1. ~~**Fire vs. Dark Debuff distinction**~~ — **Resolved:** See below.
2. **PvP model** — async-only at launch vs. RTA at launch
3. **Regional launch strategy** — global simultaneous vs. soft launch in SEA/KR
4. **Hero roster** — archetypes defined, actual heroes designed after combat prototype is stable

---

## My Read on This Design

**What's strong:**
- The CR-based turn system is proven and creates genuine strategic depth
- The no-hero-reuse Abyss Tower forces roster investment beyond a single "best team"
- Stamina-free endgame modes (Boss Rush, Tower) keep hardcore players engaged without pay pressure
- The monetization stack has both a F2P-friendly path and multiple spending tiers

**What's borrowed (a lot):**
This GDD is essentially a specification for a game that plays almost identically to Epic Seven. The CR system, Soul system, Mola, Hunts, Artifact system, Abyss Tower structure, Guild Wars, and even the pity numbers are near-copies. The differentiation would need to come from art, characters, and story — which aren't designed in this document yet.

**What needs design work:**
- The actual hero roster (personalities, skill kits, synergies)
- The narrative and world — currently placeholder chapter themes
- The Fire vs. Dark debuff distinction (hinted as a design intention but unresolved)
- PvP balance philosophy (RTA in gacha games is notoriously hard to balance)
- The Labyrinth mode is underdeveloped compared to other modes
