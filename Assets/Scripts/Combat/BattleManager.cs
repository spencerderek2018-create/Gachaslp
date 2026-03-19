using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GachaRPG
{
    /// <summary>
    /// Master orchestrator for a single battle.
    ///
    /// Responsibilities:
    ///   - Initialise all BattleUnits from HeroData arrays
    ///   - Drive the main battle loop (coroutine)
    ///   - Route player input to SkillResolver
    ///   - Run basic enemy AI
    ///   - Trigger dual attack checks after S1 usage
    ///   - Call IGameMode hooks at appropriate moments
    ///   - Detect and handle win/loss conditions
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("Team Setup")]
        public HeroData[] playerHeroData;  // Assign up to 4 heroes in Inspector
        public HeroData[] enemyHeroData;   // Assign up to 4 enemies in Inspector

        // --- Runtime unit lists ---
        private List<BattleUnit> playerUnits = new();
        private List<BattleUnit> enemyUnits  = new();
        private List<BattleUnit> allUnits    = new();

        // --- Core systems ---
        private TurnManager       turnManager;
        private SoulManager       soulManager;
        private DualAttackResolver dualAttack;

        // --- Active turn state ---
        private BattleUnit activeUnit;
        private bool       playerActionSubmitted;
        private bool       lastSkillWasS1;
        private BattleUnit lastTargetHit;

        // --- Game mode (optional, set before battle starts) ---
        private IGameMode currentMode;

        // --- Events for UI ---
        public event System.Action<BattleUnit>          OnTurnStarted;
        public event System.Action<bool>                OnBattleEnded;    // true = player won
        public event System.Action<List<BattleUnit>>    OnTurnOrderUpdated;

        // Exposed so BattleUIBuilder can bind the SoulGaugeUI after init
        public SoulManager PlayerSoulManager => soulManager;

        // ----------------------------------------------------------------
        // Public API
        // ----------------------------------------------------------------

        /// <summary>Call before StartBattle to attach a game mode (Hunt, Boss Rush, etc.).</summary>
        public void SetGameMode(IGameMode mode) => currentMode = mode;

        public List<BattleUnit> GetPlayerUnits() => new(playerUnits);
        public List<BattleUnit> GetEnemyUnits()  => new(enemyUnits);

        /// <summary>Restores player unit states between Boss Rush rounds.</summary>
        public void RestoreUnits(List<BattleUnit> preserved)
        {
            // Replace player units with the preserved snapshot (HP, cooldowns intact)
            playerUnits = preserved;
            allUnits = playerUnits.Concat(enemyUnits).ToList();
        }

        // ----------------------------------------------------------------
        // Unity lifecycle
        // ----------------------------------------------------------------

        private void Start()
        {
            InitBattle();
            StartCoroutine(BattleLoop());
        }

        // ----------------------------------------------------------------
        // Initialisation
        // ----------------------------------------------------------------

        private void InitBattle()
        {
            playerUnits.Clear();
            enemyUnits.Clear();
            allUnits.Clear();

            foreach (var data in playerHeroData)
            {
                if (data == null) continue;
                var unit = new BattleUnit(data, isPlayerTeam: true);
                playerUnits.Add(unit);
                allUnits.Add(unit);
            }

            foreach (var data in enemyHeroData)
            {
                if (data == null) continue;
                var unit = new BattleUnit(data, isPlayerTeam: false);
                enemyUnits.Add(unit);
                allUnits.Add(unit);
            }

            turnManager = new TurnManager(allUnits);
            soulManager = new SoulManager();
            dualAttack  = new DualAttackResolver();

            currentMode?.OnBattleStart(this);

            Debug.Log($"[Battle] Started — {playerUnits.Count} heroes vs {enemyUnits.Count} enemies.");
        }

        // ----------------------------------------------------------------
        // Main battle loop
        // ----------------------------------------------------------------

        private IEnumerator BattleLoop()
        {
            while (!IsBattleOver())
            {
                // Advance CR until the next unit is ready
                activeUnit = turnManager.AdvanceToNextTurn();
                if (activeUnit == null || activeUnit.IsDead) continue;

                // Notify UI of new turn order
                OnTurnOrderUpdated?.Invoke(turnManager.PreviewTurnOrder(8));
                OnTurnStarted?.Invoke(activeUnit);

                // Tick debuffs, decrement cooldowns
                activeUnit.OnTurnStart();

                // Immunity set 2pc: grant immunity buff at start of turn
                // (SetBonusManager writes a flag; BattleUnit checks it on TryApplyDebuff)

                if (activeUnit.IsDead)
                {
                    CleanUpDead();
                    continue;
                }

                // Skip turn for stunned/frozen units
                if (activeUnit.IsStunned)
                {
                    Debug.Log($"[Turn] {activeUnit.Data.heroName} is stunned — skipping turn.");
                    yield return new WaitForSeconds(0.4f);
                    continue;
                }

                lastSkillWasS1  = false;
                lastTargetHit   = null;

                if (activeUnit.IsPlayerTeam)
                    yield return StartCoroutine(HandlePlayerTurn(activeUnit));
                else
                    yield return StartCoroutine(HandleEnemyTurn(activeUnit));

                // Dual attack check — only triggers after S1
                if (lastSkillWasS1)
                {
                    var teamAllies  = activeUnit.IsPlayerTeam ? playerUnits : enemyUnits;
                    var teamEnemies = activeUnit.IsPlayerTeam ? enemyUnits  : playerUnits;

                    var dualAttacker = dualAttack.TryTriggerDualAttack(
                        activeUnit, teamAllies, teamEnemies, lastTargetHit);

                    if (dualAttacker != null)
                        yield return new WaitForSeconds(0.3f);
                }

                CleanUpDead();
                yield return null; // breathe for one frame
            }

            bool playerWon = playerUnits.Any(u => !u.IsDead);
            currentMode?.OnRoundEnd(this, playerWon);
            EndBattle(playerWon);
        }

        // ----------------------------------------------------------------
        // Player turn
        // ----------------------------------------------------------------

        private IEnumerator HandlePlayerTurn(BattleUnit unit)
        {
            playerActionSubmitted = false;

            // Signal HUD to show skill buttons for this unit
            BattleHUD.Instance?.ShowSkillButtons(unit, soulManager);

            // Auto-select the first living enemy so the player can press a skill immediately
            var firstEnemy = enemyUnits.Find(u => !u.IsDead);
            if (firstEnemy != null)
                BattleHUD.Instance?.SetSelectedTarget(firstEnemy);

            Debug.Log($"[Turn] {unit.Data.heroName}'s turn. Waiting for player input...");

            // Wait until player submits an action via SubmitPlayerAction()
            yield return new WaitUntil(() => playerActionSubmitted);

            BattleHUD.Instance?.HideSkillButtons();
        }

        /// <summary>
        /// Called by UI when the player selects a skill and a target.
        /// Validates the action and resolves it.
        /// </summary>
        public void SubmitPlayerAction(SkillSlot slot, BattleUnit target, bool soulBurn = false)
        {
            if (activeUnit == null || !activeUnit.IsPlayerTeam) return;

            SkillData skill = slot switch
            {
                SkillSlot.S1 => activeUnit.Data.s1_BasicAttack,
                SkillSlot.S2 => activeUnit.Data.s2,
                SkillSlot.S3 => activeUnit.Data.s3_Ultimate,
                _            => null
            };

            if (skill == null)
            {
                Debug.LogWarning($"[Input] {activeUnit.Data.heroName} has no skill in slot {slot}.");
                return;
            }

            if (!activeUnit.IsSkillReady(slot))
            {
                Debug.LogWarning($"[Input] {skill.skillName} is on cooldown ({activeUnit.GetCooldown(slot)} turns).");
                return;
            }

            if (activeUnit.IsSilenced && slot != SkillSlot.S1)
            {
                Debug.LogWarning($"[Input] {activeUnit.Data.heroName} is silenced — only S1 allowed.");
                return;
            }

            if (soulBurn && !soulManager.TrySpendSoul(skill.soulBurnCost))
                return;

            lastSkillWasS1 = slot == SkillSlot.S1;
            lastTargetHit  = target;

            SkillResolver.Execute(
                activeUnit,
                skill,
                new List<BattleUnit> { target },
                allUnits,
                soulManager,
                turnManager,
                soulBurn);

            playerActionSubmitted = true;
        }

        // ----------------------------------------------------------------
        // Enemy AI turn
        // ----------------------------------------------------------------

        private IEnumerator HandleEnemyTurn(BattleUnit unit)
        {
            yield return new WaitForSeconds(0.7f); // brief "thinking" pause

            SkillData skill = PickEnemySkill(unit);

            // Target selection: pick the lowest-HP player unit (focus fire)
            BattleUnit target = playerUnits
                .Where(u => !u.IsDead)
                .OrderBy(u => u.CurrentHP)
                .FirstOrDefault();

            if (target == null) yield break;

            lastSkillWasS1 = skill.slot == SkillSlot.S1;
            lastTargetHit  = target;

            SkillResolver.Execute(
                unit,
                skill,
                new List<BattleUnit> { target },
                allUnits,
                soulManager,
                turnManager);

            yield return new WaitForSeconds(0.4f);
        }

        /// <summary>Simple AI priority: S3 if ready, else S2 if ready, else S1.</summary>
        private SkillData PickEnemySkill(BattleUnit unit)
        {
            if (unit.Data.s3_Ultimate != null && unit.IsSkillReady(SkillSlot.S3))
                return unit.Data.s3_Ultimate;
            if (unit.Data.s2 != null && unit.IsSkillReady(SkillSlot.S2))
                return unit.Data.s2;
            return unit.Data.s1_BasicAttack;
        }

        // ----------------------------------------------------------------
        // Cleanup & end condition
        // ----------------------------------------------------------------

        private void CleanUpDead()
        {
            playerUnits.RemoveAll(u => u.IsDead);
            enemyUnits.RemoveAll(u => u.IsDead);
            allUnits.RemoveAll(u => u.IsDead);
        }

        private bool IsBattleOver() =>
            playerUnits.All(u => u.IsDead) || enemyUnits.All(u => u.IsDead);

        private void EndBattle(bool playerWon)
        {
            Debug.Log($"[Battle] {(playerWon ? "VICTORY!" : "DEFEAT.")}");
            OnBattleEnded?.Invoke(playerWon);
            BattleResultScreen.Instance?.Show(playerWon, currentMode?.CalculateRewards());
        }
    }
}
