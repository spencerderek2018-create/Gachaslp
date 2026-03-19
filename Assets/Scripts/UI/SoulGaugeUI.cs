using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GachaRPG
{
    /// <summary>
    /// Displays the team's Soul gauge as a row of pip images.
    /// Subscribes to SoulManager.OnSoulChanged and updates filled/empty pips.
    ///
    /// Attach to a Canvas element.
    /// Assign pipImages (array of 10 Image components) in the Inspector.
    /// </summary>
    public class SoulGaugeUI : MonoBehaviour
    {
        [Header("Pip Images (10 total)")]
        public Image[] pipImages;

        [Header("Colors")]
        public Color filledColor = new(0.9f, 0.7f, 0.1f); // gold
        public Color emptyColor  = new(0.3f, 0.3f, 0.3f); // dark grey

        [Header("Soul Count Label")]
        public TextMeshProUGUI soulCountLabel;

        private SoulManager soulManager;

        private void Start()
        {
            // Find the SoulManager via BattleManager
            // In a real implementation this would be injected or found through a service locator
            var battle = FindObjectOfType<BattleManager>();
            if (battle == null) return;

            // SoulManager is private in BattleManager — expose via event subscription
            // For prototype: BattleManager raises the OnSoulChanged event through SoulManager
            // We subscribe once the manager is accessible
            // TODO: expose SoulManager from BattleManager or use a singleton for prototype
        }

        /// <summary>
        /// Call this to bind to a SoulManager instance (called by BattleManager after init).
        /// </summary>
        public void Bind(SoulManager manager)
        {
            soulManager = manager;
            soulManager.OnSoulChanged += Refresh;
            Refresh(soulManager.CurrentSoul, SoulManager.MaxSoul);
        }

        private void OnDestroy()
        {
            if (soulManager != null)
                soulManager.OnSoulChanged -= Refresh;
        }

        private void Refresh(int current, int max)
        {
            if (soulCountLabel != null)
                soulCountLabel.text = $"{current}/{max}";

            for (int i = 0; i < pipImages.Length; i++)
                pipImages[i].color = i < current ? filledColor : emptyColor;
        }
    }
}
