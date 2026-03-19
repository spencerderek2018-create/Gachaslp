using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GachaRPG
{
    /// <summary>
    /// Victory/Defeat result screen shown at the end of a battle.
    /// Displays outcome, rewards summary, and navigation options.
    ///
    /// Attach to a full-screen Canvas panel (starts hidden).
    /// BattleManager.OnBattleEnded triggers Show().
    /// </summary>
    public class BattleResultScreen : MonoBehaviour
    {
        public static BattleResultScreen Instance { get; private set; }

        [Header("Panel")]
        public GameObject panel;

        [Header("Outcome")]
        public TextMeshProUGUI outcomeLabel;    // "VICTORY" / "DEFEAT"
        public Image           outcomeBacking;
        public Color           victoryColor = new(0.2f, 0.8f, 0.2f);
        public Color           defeatColor  = new(0.8f, 0.2f, 0.2f);

        [Header("Rewards")]
        public TextMeshProUGUI goldLabel;
        public TextMeshProUGUI skystonesLabel;
        public TextMeshProUGUI bookmarksLabel;
        public TextMeshProUGUI gearDropsLabel;
        public TextMeshProUGUI bonusMessageLabel;

        [Header("Buttons")]
        public Button retryButton;
        public Button continueButton;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            panel?.SetActive(false); // null-safe: BattleUIBuilder assigns panel after AddComponent
        }

        private void Start()
        {
            var battleManager = FindFirstObjectByType<BattleManager>();
            if (battleManager != null)
                battleManager.OnBattleEnded += (won) => Show(won, null);

            retryButton.onClick.AddListener(OnRetry);
            continueButton.onClick.AddListener(OnContinue);
        }

        /// <summary>Shows the result screen with the battle outcome and rewards.</summary>
        public void Show(bool playerWon, ModeRewards rewards)
        {
            panel.SetActive(true);

            outcomeLabel.text   = playerWon ? "VICTORY!" : "DEFEAT";
            outcomeBacking.color = playerWon ? victoryColor : defeatColor;

            if (rewards != null)
            {
                goldLabel.text      = $"Gold: {rewards.Gold:N0}";
                skystonesLabel.text = $"Skystones: {rewards.Skystones}";
                bookmarksLabel.text = $"Bookmarks: {rewards.Bookmarks}";
                gearDropsLabel.text = rewards.GearDrops.Count > 0
                    ? $"Gear Drops: {rewards.GearDrops.Count}"
                    : "No gear drops";
                bonusMessageLabel.text = rewards.BonusMessage ?? "";
            }
            else
            {
                goldLabel.text      = "";
                skystonesLabel.text = "";
                bookmarksLabel.text = "";
                gearDropsLabel.text = "";
                bonusMessageLabel.text = "";
            }

            retryButton.gameObject.SetActive(!playerWon);
            continueButton.gameObject.SetActive(playerWon);
        }

        private void OnRetry()
        {
            panel.SetActive(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void OnContinue()
        {
            panel.SetActive(false);
            // TODO: Return to map/stage select screen
            Debug.Log("[Result] Continue pressed — return to stage select.");
        }
    }
}
