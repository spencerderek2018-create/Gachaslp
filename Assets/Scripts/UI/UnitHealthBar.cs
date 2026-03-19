using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GachaRPG
{
    /// <summary>
    /// Displays a single unit's HP as a filled Image bar and optional text.
    /// Bind to a BattleUnit via Bind() after the unit is created.
    ///
    /// Attach to each hero/enemy portrait prefab in the battle scene.
    /// Assign fillImage and optionally hpText in the Inspector.
    /// </summary>
    public class UnitHealthBar : MonoBehaviour
    {
        [Header("References")]
        public Image            fillImage;  // Image with Image Type = Filled
        public TextMeshProUGUI  hpText;     // Optional: "1200 / 8000"

        [Header("Colors")]
        public Color fullHealthColor = new(0.2f, 0.85f, 0.2f);  // green
        public Color midHealthColor  = new(0.95f, 0.8f, 0.1f);  // yellow
        public Color lowHealthColor  = new(0.9f, 0.2f, 0.2f);   // red

        private BattleUnit boundUnit;

        /// <summary>Binds this health bar to a BattleUnit and subscribes to HP change events.</summary>
        public void Bind(BattleUnit unit)
        {
            if (boundUnit != null)
                boundUnit.OnHPChanged -= OnHPChanged;

            boundUnit = unit;
            unit.OnHPChanged += OnHPChanged;

            // Initialise display
            OnHPChanged(unit.CurrentHP, unit.FinalHP);
        }

        private void OnDestroy()
        {
            if (boundUnit != null)
                boundUnit.OnHPChanged -= OnHPChanged;
        }

        private void OnHPChanged(int current, int max)
        {
            if (fillImage == null) return;

            float ratio = max > 0 ? (float)current / max : 0f;
            fillImage.fillAmount = ratio;
            fillImage.color = ratio > 0.5f ? fullHealthColor
                            : ratio > 0.25f ? midHealthColor
                            : lowHealthColor;

            if (hpText != null)
                hpText.text = $"{current:N0} / {max:N0}";
        }
    }
}
