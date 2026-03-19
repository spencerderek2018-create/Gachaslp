using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GachaRPG
{
    /// <summary>
    /// Displays active debuff icons with remaining turn counts for a single BattleUnit.
    ///
    /// Each debuff gets a small icon showing:
    ///   - A colored square (placeholder until art is available)
    ///   - The debuff type abbreviation
    ///   - Remaining turn count
    ///
    /// Attach to each unit portrait in the battle scene.
    /// Assign iconPrefab and container in the Inspector.
    /// </summary>
    public class DebuffIconDisplay : MonoBehaviour
    {
        [Header("References")]
        public GameObject iconPrefab;   // Small prefab: Image + TextMeshProUGUI
        public Transform  container;    // HorizontalLayoutGroup

        // Colors per debuff category (placeholder art)
        private static readonly Dictionary<DebuffType, Color> DebuffColors = new()
        {
            { DebuffType.Burn,    new Color(1.0f, 0.4f, 0.1f) }, // orange
            { DebuffType.Scorch,  new Color(1.0f, 0.2f, 0.0f) }, // deep orange
            { DebuffType.Ignite,  new Color(1.0f, 0.5f, 0.0f) }, // amber
            { DebuffType.Curse,   new Color(0.5f, 0.0f, 0.8f) }, // purple
            { DebuffType.Blind,   new Color(0.4f, 0.2f, 0.6f) }, // dark purple
            { DebuffType.Weaken,  new Color(0.6f, 0.0f, 0.6f) }, // violet
            { DebuffType.Silence, new Color(0.3f, 0.0f, 0.5f) }, // deep purple
            { DebuffType.Chill,   new Color(0.4f, 0.8f, 1.0f) }, // light blue
            { DebuffType.Freeze,  new Color(0.1f, 0.5f, 1.0f) }, // blue
            { DebuffType.Stun,    new Color(1.0f, 1.0f, 0.0f) }, // yellow
            { DebuffType.Provoke, new Color(1.0f, 0.0f, 0.3f) }, // red
        };

        private BattleUnit boundUnit;
        private readonly List<GameObject> spawnedIcons = new();

        public void Bind(BattleUnit unit)
        {
            if (boundUnit != null)
            {
                boundUnit.OnDebuffApplied  -= OnDebuffApplied;
                boundUnit.OnDebuffExpired  -= OnDebuffExpired;
            }

            boundUnit = unit;
            unit.OnDebuffApplied += OnDebuffApplied;
            unit.OnDebuffExpired += OnDebuffExpired;

            Refresh();
        }

        private void OnDestroy()
        {
            if (boundUnit != null)
            {
                boundUnit.OnDebuffApplied  -= OnDebuffApplied;
                boundUnit.OnDebuffExpired  -= OnDebuffExpired;
            }
        }

        private void OnDebuffApplied(DebuffInstance _) => Refresh();
        private void OnDebuffExpired(DebuffInstance _) => Refresh();

        private void Refresh()
        {
            foreach (var icon in spawnedIcons) Destroy(icon);
            spawnedIcons.Clear();

            if (boundUnit == null) return;

            foreach (var debuff in boundUnit.ActiveDebuffs)
            {
                var go  = Instantiate(iconPrefab, container);
                spawnedIcons.Add(go);

                // Background color by debuff type
                var img = go.GetComponent<Image>();
                if (img != null && DebuffColors.TryGetValue(debuff.Type, out var col))
                    img.color = col;

                // Label: abbreviation + turns remaining
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    string abbr = debuff.Type.ToString()[..Mathf.Min(3, debuff.Type.ToString().Length)];
                    label.text = $"{abbr}\n{debuff.RemainingTurns}";
                }
            }
        }
    }
}
