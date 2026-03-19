using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GachaRPG
{
    /// <summary>
    /// Displays the upcoming turn order as a horizontal row of unit icons.
    /// Subscribes to BattleManager.OnTurnOrderUpdated and redraws on each change.
    ///
    /// Each slot in the display shows:
    ///   - A colored icon (player = blue, enemy = red) as a placeholder
    ///   - The hero's name below it
    ///   - The active unit highlighted
    ///
    /// Attach to a horizontal layout group in the Canvas.
    /// Assign the iconPrefab and container in the Inspector.
    /// </summary>
    public class TurnOrderDisplay : MonoBehaviour
    {
        [Header("References")]
        public GameObject  iconPrefab;    // Prefab with Image + TextMeshProUGUI child
        public Transform   iconContainer; // HorizontalLayoutGroup
        public int         displayCount = 8;

        [Header("Colors")]
        public Color playerColor = new(0.3f, 0.6f, 1.0f);
        public Color enemyColor  = new(1.0f, 0.3f, 0.3f);
        public Color activeColor = Color.yellow;

        private BattleManager battleManager;
        private readonly List<GameObject> spawnedIcons = new();

        private void Start()
        {
            battleManager = FindFirstObjectByType<BattleManager>();
            if (battleManager != null)
                battleManager.OnTurnOrderUpdated += Refresh;
        }

        private void OnDestroy()
        {
            if (battleManager != null)
                battleManager.OnTurnOrderUpdated -= Refresh;
        }

        /// <summary>Redraws the turn order bar with the provided ordered unit list.</summary>
        public void Refresh(List<BattleUnit> order)
        {
            // Clear existing icons
            foreach (var icon in spawnedIcons)
                Destroy(icon);
            spawnedIcons.Clear();

            int count = Mathf.Min(order.Count, displayCount);
            for (int i = 0; i < count; i++)
            {
                var unit = order[i];
                var go   = Instantiate(iconPrefab, iconContainer);
                spawnedIcons.Add(go);

                // Background image color — first in order is active (yellow)
                var img = go.GetComponent<Image>();
                if (img != null)
                    img.color = i == 0
                        ? activeColor
                        : unit.IsPlayerTeam ? playerColor : enemyColor;

                // Name label
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = unit.Data.heroName.Length > 6
                        ? unit.Data.heroName[..6]
                        : unit.Data.heroName;
            }
        }
    }
}
