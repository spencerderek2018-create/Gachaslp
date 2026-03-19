using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GachaRPG
{
    /// <summary>
    /// Manages the main battle HUD.
    /// Shows skill buttons for the active player unit, handles Soul Burn toggle,
    /// and forwards player actions to BattleManager.
    ///
    /// Attach to a Canvas GameObject in the battle scene.
    /// Assign all Button/Text references in the Inspector.
    /// </summary>
    public class BattleHUD : MonoBehaviour
    {
        public static BattleHUD Instance { get; private set; }

        [Header("Skill Buttons")]
        public Button s1Button;
        public Button s2Button;
        public Button s3Button;

        [Header("Skill Labels")]
        public TextMeshProUGUI s1Label;
        public TextMeshProUGUI s2Label;
        public TextMeshProUGUI s3Label;

        [Header("Cooldown Overlays")]
        public TextMeshProUGUI s2CooldownText;
        public TextMeshProUGUI s3CooldownText;

        [Header("Soul Burn")]
        public Button soulBurnToggle;
        public TextMeshProUGUI soulBurnLabel;
        private bool soulBurnActive;

        [Header("Active Unit Info")]
        public TextMeshProUGUI activeUnitName;
        public TextMeshProUGUI activeUnitHP;

        // --- Internal state ---
        private BattleUnit    currentUnit;
        private SoulManager   soulManager;
        private BattleManager battleManager;

        // Player's selected target (set by clicking an enemy unit in the scene)
        private BattleUnit selectedTarget;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            battleManager = FindObjectOfType<BattleManager>();
        }

        /// <summary>
        /// Called by BattleUIBuilder immediately after button references are assigned.
        /// Must run before BattleManager.Start() triggers the first player turn.
        /// </summary>
        public void Init()
        {
            s1Button.onClick.AddListener(() => OnSkillButtonPressed(SkillSlot.S1));
            s2Button.onClick.AddListener(() => OnSkillButtonPressed(SkillSlot.S2));
            s3Button.onClick.AddListener(() => OnSkillButtonPressed(SkillSlot.S3));
            soulBurnToggle.onClick.AddListener(OnSoulBurnToggled);
        }

        /// <summary>Called by BattleManager at the start of a player turn.</summary>
        public void ShowSkillButtons(BattleUnit unit, SoulManager soul)
        {
            currentUnit  = unit;
            soulManager  = soul;
            soulBurnActive = false;

            gameObject.SetActive(true);

            activeUnitName.text = unit.Data.heroName;
            activeUnitHP.text   = $"{unit.CurrentHP} / {unit.FinalHP}";

            RefreshSkillButtons(unit, soul);
        }

        public void HideSkillButtons()
        {
            currentUnit = null;
            s1Button.interactable = false;
            s2Button.interactable = false;
            s3Button.interactable = false;
            soulBurnToggle.interactable = false;
        }

        /// <summary>Called by enemy unit click handlers in the scene.</summary>
        public void SetSelectedTarget(BattleUnit target)
        {
            selectedTarget = target;
            Debug.Log($"[HUD] Target selected: {target?.Data.heroName}");
        }

        // ----------------------------------------------------------------
        // Button handlers
        // ----------------------------------------------------------------

        private void OnSkillButtonPressed(SkillSlot slot)
        {
            if (currentUnit == null || selectedTarget == null) return;
            battleManager.SubmitPlayerAction(slot, selectedTarget, soulBurnActive);
            HideSkillButtons();
        }

        private void OnSoulBurnToggled()
        {
            soulBurnActive = !soulBurnActive;
            soulBurnLabel.text = soulBurnActive ? "Soul Burn: ON" : "Soul Burn: OFF";
            soulBurnLabel.color = soulBurnActive ? Color.yellow : Color.white;
            RefreshSkillButtons(currentUnit, soulManager);
        }

        // ----------------------------------------------------------------
        // Refresh
        // ----------------------------------------------------------------

        private void RefreshSkillButtons(BattleUnit unit, SoulManager soul)
        {
            if (unit == null) return;

            // S1 — always available, no cooldown
            s1Button.interactable = true;
            s1Label.text = unit.Data.s1_BasicAttack != null
                ? unit.Data.s1_BasicAttack.skillName
                : "Attack";

            // S2
            bool s2Ready = unit.IsSkillReady(SkillSlot.S2) && unit.Data.s2 != null && !unit.IsSilenced;
            s2Button.interactable = s2Ready;
            s2Label.text = unit.Data.s2 != null ? unit.Data.s2.skillName : "—";
            s2CooldownText.text = s2Ready ? "" : unit.GetCooldown(SkillSlot.S2).ToString();

            // S3
            bool s3Ready = unit.IsSkillReady(SkillSlot.S3) && unit.Data.s3_Ultimate != null && !unit.IsSilenced;
            s3Button.interactable = s3Ready;
            s3Label.text = unit.Data.s3_Ultimate != null ? unit.Data.s3_Ultimate.skillName : "—";
            s3CooldownText.text = s3Ready ? "" : unit.GetCooldown(SkillSlot.S3).ToString();

            // Soul Burn toggle — only show if the current skill has a soul burn variant
            bool canBurn = soul != null && unit.Data.s3_Ultimate != null &&
                           soul.CanSoulBurn(unit.Data.s3_Ultimate);
            soulBurnToggle.interactable = canBurn;
            soulBurnLabel.text = soulBurnActive ? "Soul Burn: ON" : "Soul Burn: OFF";
        }
    }
}
