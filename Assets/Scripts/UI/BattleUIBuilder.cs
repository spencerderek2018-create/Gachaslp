using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;  // still needed for EventSystem / StandaloneInputModule
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using TMPro;


namespace GachaRPG
{
    /// <summary>
    /// Programmatically builds the entire Battle UI at runtime.
    ///
    /// Add this script to any GameObject in the TestBattle scene.
    /// It will auto-construct a Canvas with all panels, wire every
    /// component together, and bind health bars / debuff icons to
    /// the BattleUnits after BattleManager.Start() has run.
    ///
    /// Requires TextMeshPro Essentials imported in the project.
    /// </summary>
    [DefaultExecutionOrder(10)] // runs after BattleManager (default 0)
    public class BattleUIBuilder : MonoBehaviour
    {
        // ── runtime bindings (filled by BuildUI, read by Start) ───────
        private UnitHealthBar[]     _playerHPs     = new UnitHealthBar[4];
        private UnitHealthBar[]     _enemyHPs      = new UnitHealthBar[4];
        private DebuffIconDisplay[] _playerDebuffs = new DebuffIconDisplay[4];
        private DebuffIconDisplay[] _enemyDebuffs  = new DebuffIconDisplay[4];
        private SoulGaugeUI         _soulGauge;

        // ── lifecycle ─────────────────────────────────────────────────

        private void Awake() => BuildUI();

        private void Start()
        {
            var bm = FindFirstObjectByType<BattleManager>();
            if (bm == null)
            {
                Debug.LogError("[UIBuilder] BattleManager not found in scene.");
                return;
            }

            BindUnits(bm.GetPlayerUnits(), _playerHPs, _playerDebuffs);
            BindUnits(bm.GetEnemyUnits(),  _enemyHPs,  _enemyDebuffs);

            if (bm.PlayerSoulManager != null)
                _soulGauge.Bind(bm.PlayerSoulManager);
            else
                Debug.LogWarning("[UIBuilder] SoulManager not ready — soul gauge will be empty.");
        }

        // ── unit binding ──────────────────────────────────────────────

        private static void BindUnits(
            List<BattleUnit>    units,
            UnitHealthBar[]     hps,
            DebuffIconDisplay[] debuffs)
        {
            for (int i = 0; i < units.Count && i < hps.Length; i++)
            {
                hps[i]?.Bind(units[i]);
                debuffs[i]?.Bind(units[i]);
            }
        }

        // ── top-level builder ─────────────────────────────────────────

        private void BuildUI()
        {
            EnsureEventSystem();

            var canvas = MakeCanvas();
            var root   = canvas.gameObject;

            // Shared icon prefabs (disabled GameObjects used as templates)
            var turnIconPrefab  = MakeIconPrefab(root, "TurnIconTemplate",  52, 52, 12);
            var debuffIconPrefab = MakeIconPrefab(root, "DebuffIconTemplate", 38, 38, 9);

            MakeTurnOrderBar(root, turnIconPrefab);

            (_enemyHPs,  _enemyDebuffs)  = MakeUnitPanels(root, debuffIconPrefab, isPlayer: false, count: 2);
            (_playerHPs, _playerDebuffs) = MakeUnitPanels(root, debuffIconPrefab, isPlayer: true,  count: 2);

            _soulGauge = MakeSoulGauge(root);

            MakeSkillPanel(root);
            MakeResultScreen(root);
        }

        // ── EventSystem ───────────────────────────────────────────────

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }

        // ── canvas ────────────────────────────────────────────────────

        private static Canvas MakeCanvas()
        {
            var go     = new GameObject("BattleCanvas");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution  = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight   = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        // ── icon prefab template ──────────────────────────────────────

        private static GameObject MakeIconPrefab(
            GameObject parent, string name, float w, float h, int fontSize)
        {
            var go  = new GameObject(name);
            go.transform.SetParent(parent.transform, false);

            var img = go.AddComponent<Image>();
            img.color = Color.grey;

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(w, h);

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);

            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.fontSize  = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color     = Color.white;
            tmp.text      = "";

            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;

            go.SetActive(false); // inactive = acts as prefab template
            return go;
        }

        // ── turn order bar ────────────────────────────────────────────

        private static void MakeTurnOrderBar(GameObject canvas, GameObject iconPrefab)
        {
            var panel = MakePanel(canvas, "TurnOrderBar",
                new Vector2(0.1f, 0.92f), new Vector2(0.9f, 1.0f));
            panel.color = new Color(0f, 0f, 0f, 0.55f);

            var hlg = panel.gameObject.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing                = 4;
            hlg.childAlignment         = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth  = false;
            hlg.childForceExpandHeight = true;
            hlg.padding                = new RectOffset(8, 8, 4, 4);

            var display = panel.gameObject.AddComponent<TurnOrderDisplay>();
            display.iconPrefab    = iconPrefab;
            display.iconContainer = panel.transform;
            display.displayCount  = 8;
        }

        // ── unit panels ───────────────────────────────────────────────

        private (UnitHealthBar[], DebuffIconDisplay[]) MakeUnitPanels(
            GameObject canvas, GameObject debuffPrefab, bool isPlayer, int count)
        {
            var hps     = new UnitHealthBar[count];
            var debuffs = new DebuffIconDisplay[count];

            for (int i = 0; i < count; i++)
            {
                float xMin, xMax, yMin, yMax;

                if (isPlayer)
                {
                    xMin = 0.02f; xMax = 0.26f;
                    yMin = 0.10f + i * 0.17f;
                    yMax = 0.24f + i * 0.17f;
                }
                else
                {
                    xMin = 0.74f; xMax = 0.98f;
                    yMin = 0.70f - i * 0.17f;
                    yMax = 0.84f - i * 0.17f;
                }

                var panel = MakePanel(canvas, $"{(isPlayer ? "Player" : "Enemy")}Panel_{i}",
                    new Vector2(xMin, yMin), new Vector2(xMax, yMax));
                panel.color = new Color(0f, 0f, 0f, 0.65f);

                // --- Name label ---
                var nameGo  = MakeChild(panel.gameObject, "NameLabel");
                var nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
                nameTmp.fontSize      = 13;
                nameTmp.fontStyle     = FontStyles.Bold;
                nameTmp.alignment     = TextAlignmentOptions.Center;
                nameTmp.color         = Color.white;
                nameTmp.text          = isPlayer ? $"Player {i + 1}" : $"Enemy {i + 1}";
                nameTmp.raycastTarget = false;
                SetAnchors(nameGo, 0, 0.78f, 1, 1.0f);

                // --- HP bar background (no raycast — parent Button handles clicks) ---
                var bgGo  = MakeChild(panel.gameObject, "HPBarBG");
                var bgImg = bgGo.AddComponent<Image>();
                bgImg.color          = new Color(0.18f, 0.18f, 0.18f);
                bgImg.raycastTarget  = false;
                SetAnchors(bgGo, 0.02f, 0.52f, 0.98f, 0.77f);

                // --- HP bar fill ---
                var fillGo  = MakeChild(bgGo, "HPBarFill");
                var fillImg = fillGo.AddComponent<Image>();
                fillImg.type          = Image.Type.Filled;
                fillImg.fillMethod    = Image.FillMethod.Horizontal;
                fillImg.color         = new Color(0.2f, 0.85f, 0.2f);
                fillImg.raycastTarget = false;
                SetAnchors(fillGo, 0, 0, 1, 1);

                // --- HP text ---
                var hpTextGo  = MakeChild(panel.gameObject, "HPText");
                var hpTmp     = hpTextGo.AddComponent<TextMeshProUGUI>();
                hpTmp.fontSize      = 11;
                hpTmp.alignment     = TextAlignmentOptions.Center;
                hpTmp.color         = Color.white;
                hpTmp.raycastTarget = false;
                SetAnchors(hpTextGo, 0, 0.75f, 1, 0.92f);

                // --- Debuff container ---
                var debuffContainer = MakeChild(panel.gameObject, "DebuffContainer");
                SetAnchors(debuffContainer, 0, 0.01f, 1, 0.50f);
                var debuffHlg = debuffContainer.AddComponent<HorizontalLayoutGroup>();
                debuffHlg.spacing                = 2;
                debuffHlg.childForceExpandWidth  = false;
                debuffHlg.childForceExpandHeight = false;
                debuffHlg.childAlignment         = TextAnchor.MiddleLeft;

                // --- Enemy panel click → select target (Button is more reliable than EventTrigger) ---
                if (!isPlayer)
                {
                    int captured = i; // closure capture
                    var btn = panel.gameObject.AddComponent<Button>();
                    // Keep the existing Image as the Button's target graphic
                    btn.targetGraphic = panel;
                    var colors = btn.colors;
                    colors.normalColor      = new Color(1, 1, 1, 1);
                    colors.highlightedColor = new Color(1, 1, 0.4f, 1); // yellow tint on hover
                    colors.selectedColor    = new Color(1, 0.85f, 0.2f, 1);
                    colors.pressedColor     = new Color(0.8f, 0.6f, 0.1f, 1);
                    btn.colors = colors;
                    btn.onClick.AddListener(() =>
                    {
                        Debug.Log($"[UIBuilder] Enemy panel clicked: {captured}");
                        var bm = FindFirstObjectByType<BattleManager>();
                        if (bm == null) return;
                        var enemies = bm.GetEnemyUnits();
                        if (captured < enemies.Count)
                            BattleHUD.Instance?.SetSelectedTarget(enemies[captured]);
                    });
                }

                // --- Wire up components ---
                var healthBar = panel.gameObject.AddComponent<UnitHealthBar>();
                healthBar.fillImage = fillImg;
                healthBar.hpText    = hpTmp;

                var debuffDisplay = panel.gameObject.AddComponent<DebuffIconDisplay>();
                debuffDisplay.iconPrefab = debuffPrefab;
                debuffDisplay.container  = debuffContainer.transform;

                hps[i]     = healthBar;
                debuffs[i] = debuffDisplay;
            }

            return (hps, debuffs);
        }

        // ── soul gauge ────────────────────────────────────────────────

        private static SoulGaugeUI MakeSoulGauge(GameObject canvas)
        {
            var panel = MakePanel(canvas, "SoulGauge",
                new Vector2(0.28f, 0.03f), new Vector2(0.72f, 0.10f));
            panel.color = new Color(0f, 0f, 0.12f, 0.75f);

            var pips = new Image[10];
            for (int i = 0; i < 10; i++)
            {
                var pipGo = MakeChild(panel.gameObject, $"Pip_{i}");
                var pip   = pipGo.AddComponent<Image>();
                pip.color = new Color(0.28f, 0.28f, 0.28f);

                var rt       = pipGo.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(i / 10f + 0.005f, 0.1f);
                rt.anchorMax = new Vector2((i + 1) / 10f - 0.005f, 0.9f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                pips[i] = pip;
            }

            var labelGo = MakeChild(panel.gameObject, "SoulLabel");
            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.fontSize  = 13;
            labelTmp.alignment = TextAlignmentOptions.Center;
            labelTmp.color     = new Color(0.9f, 0.7f, 0.1f);
            labelTmp.text      = "Soul 0/10";
            SetAnchors(labelGo, 0, 0, 1, 1);

            var soulGauge = panel.gameObject.AddComponent<SoulGaugeUI>();
            soulGauge.pipImages      = pips;
            soulGauge.soulCountLabel = labelTmp;
            return soulGauge;
        }

        // ── skill panel / HUD ─────────────────────────────────────────

        private static void MakeSkillPanel(GameObject canvas)
        {
            var panel = MakePanel(canvas, "SkillPanel",
                new Vector2(0.28f, 0.01f), new Vector2(0.98f, 0.20f));
            panel.color = new Color(0.05f, 0.05f, 0.12f, 0.88f);

            // Active unit info
            var unitNameGo  = MakeChild(panel.gameObject, "ActiveUnitName");
            var unitNameTmp = unitNameGo.AddComponent<TextMeshProUGUI>();
            unitNameTmp.fontSize  = 14;
            unitNameTmp.fontStyle = FontStyles.Bold;
            unitNameTmp.alignment = TextAlignmentOptions.Left;
            unitNameTmp.color     = Color.white;
            SetAnchors(unitNameGo, 0.01f, 0.75f, 0.28f, 1.0f);

            var unitHpGo  = MakeChild(panel.gameObject, "ActiveUnitHP");
            var unitHpTmp = unitHpGo.AddComponent<TextMeshProUGUI>();
            unitHpTmp.fontSize  = 11;
            unitHpTmp.alignment = TextAlignmentOptions.Left;
            unitHpTmp.color     = new Color(0.7f, 1f, 0.7f);
            SetAnchors(unitHpGo, 0.01f, 0.52f, 0.28f, 0.75f);

            // Skill buttons
            var (s1Go, s1Label, _)    = MakeSkillButton(panel.gameObject, "S1",       0.29f, 0.50f, "Attack",   new Color(0.2f, 0.35f, 0.65f));
            var (s2Go, s2Label, s2CD) = MakeSkillButton(panel.gameObject, "S2",       0.52f, 0.73f, "Skill 2",  new Color(0.25f, 0.45f, 0.25f));
            var (s3Go, s3Label, s3CD) = MakeSkillButton(panel.gameObject, "S3",       0.75f, 0.96f, "Ultimate", new Color(0.5f, 0.2f, 0.55f));
            var (sbGo, sbLabel, _)    = MakeSkillButton(panel.gameObject, "SoulBurn", 0.29f, 0.50f, "Soul Burn",new Color(0.55f, 0.35f, 0.0f));

            // Soul Burn button lives in the lower half of the panel
            var sbRt      = sbGo.GetComponent<RectTransform>();
            sbRt.anchorMin = new Vector2(0.29f, 0.02f);
            sbRt.anchorMax = new Vector2(0.50f, 0.48f);

            // Wire BattleHUD
            var hud = panel.gameObject.AddComponent<BattleHUD>();
            hud.s1Button        = s1Go.GetComponent<Button>();
            hud.s2Button        = s2Go.GetComponent<Button>();
            hud.s3Button        = s3Go.GetComponent<Button>();
            hud.soulBurnToggle  = sbGo.GetComponent<Button>();
            hud.s1Label         = s1Label;
            hud.s2Label         = s2Label;
            hud.s3Label         = s3Label;
            hud.s2CooldownText  = s2CD;
            hud.s3CooldownText  = s3CD;
            hud.soulBurnLabel   = sbLabel;
            hud.activeUnitName  = unitNameTmp;
            hud.activeUnitHP    = unitHpTmp;

            // Register listeners now — before BattleManager.Start() can reach a player turn
            hud.Init();
        }

        private static (GameObject btn, TextMeshProUGUI label, TextMeshProUGUI cooldown)
            MakeSkillButton(GameObject parent, string name, float xMin, float xMax,
                            string defaultText, Color tint)
        {
            var go  = MakeChild(parent, $"Btn_{name}");
            var img = go.AddComponent<Image>();
            img.color = tint;

            var btn    = go.AddComponent<Button>();
            btn.targetGraphic = img; // required when adding Button via code
            btn.interactable = false; // disabled until it's the player's turn
            var colors = btn.colors;
            colors.highlightedColor = tint * 1.4f;
            colors.disabledColor    = new Color(0.15f, 0.15f, 0.15f);
            btn.colors = colors;

            var rt       = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(xMin, 0.50f);
            rt.anchorMax = new Vector2(xMax, 0.98f);
            rt.offsetMin = new Vector2(3, 2);
            rt.offsetMax = new Vector2(-3, -2);

            // Label — raycastTarget=false so the button's Image receives clicks directly
            var labelGo  = MakeChild(go, "Label");
            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.fontSize      = 13;
            labelTmp.alignment     = TextAlignmentOptions.Center;
            labelTmp.color         = Color.white;
            labelTmp.text          = defaultText;
            labelTmp.raycastTarget = false;
            SetAnchors(labelGo, 0, 0.4f, 1, 1);

            // Cooldown overlay
            var cdGo  = MakeChild(go, "Cooldown");
            var cdTmp = cdGo.AddComponent<TextMeshProUGUI>();
            cdTmp.fontSize      = 20;
            cdTmp.fontStyle     = FontStyles.Bold;
            cdTmp.alignment     = TextAlignmentOptions.Center;
            cdTmp.color         = new Color(1f, 0.55f, 0.1f);
            cdTmp.text          = "";
            cdTmp.raycastTarget = false;
            SetAnchors(cdGo, 0, 0, 1, 0.45f);

            return (go, labelTmp, cdTmp);
        }

        // ── result screen ─────────────────────────────────────────────

        private static void MakeResultScreen(GameObject canvas)
        {
            // Full-screen dim overlay
            var overlayGo  = MakeChild(canvas, "ResultOverlay");
            var overlayImg = overlayGo.AddComponent<Image>();
            overlayImg.color = new Color(0f, 0f, 0f, 0.72f);
            SetAnchors(overlayGo, 0, 0, 1, 1);
            overlayGo.SetActive(false); // hidden until battle ends

            // Inner card
            var card = MakePanel(overlayGo, "ResultCard",
                new Vector2(0.25f, 0.18f), new Vector2(0.75f, 0.82f));
            card.color = new Color(0.09f, 0.09f, 0.14f);

            // Outcome header backing
            var bgGo  = MakeChild(card.gameObject, "OutcomeBacking");
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.75f, 0.2f);
            SetAnchors(bgGo, 0, 0.74f, 1, 1);
            bgGo.transform.SetAsFirstSibling();

            // Text rows
            var outcomeLabel   = MakeText(card.gameObject, "OutcomeLabel",   0.76f, 1.0f,  36, Color.white,              "RESULT");
            var goldLabel      = MakeText(card.gameObject, "GoldLabel",      0.58f, 0.72f, 16, Color.yellow,             "");
            var skystonesLabel = MakeText(card.gameObject, "SkyLabel",       0.46f, 0.59f, 16, new Color(0.4f,0.8f,1f), "");
            var bookmarkLabel  = MakeText(card.gameObject, "BookmarkLabel",  0.34f, 0.47f, 16, Color.white,              "");
            var gearLabel      = MakeText(card.gameObject, "GearLabel",      0.22f, 0.35f, 16, new Color(1f,0.8f,0.4f), "");
            var bonusLabel     = MakeText(card.gameObject, "BonusLabel",     0.11f, 0.23f, 13, Color.grey,               "");

            // Retry button
            var retryGo = MakeChild(card.gameObject, "RetryButton");
            retryGo.AddComponent<Image>().color = new Color(0.65f, 0.15f, 0.15f);
            var retryBtn = retryGo.AddComponent<Button>();
            SetAnchors(retryGo, 0.05f, 0.01f, 0.44f, 0.09f);
            MakeText(retryGo, "Label", 0, 1, 16, Color.white, "Retry");

            // Continue button
            var contGo = MakeChild(card.gameObject, "ContinueButton");
            contGo.AddComponent<Image>().color = new Color(0.15f, 0.55f, 0.15f);
            var contBtn = contGo.AddComponent<Button>();
            SetAnchors(contGo, 0.56f, 0.01f, 0.95f, 0.09f);
            MakeText(contGo, "Label", 0, 1, 16, Color.white, "Continue");

            // Wire BattleResultScreen component onto the overlay (not the card)
            var resultScreen = overlayGo.AddComponent<BattleResultScreen>();
            resultScreen.panel             = overlayGo;
            resultScreen.outcomeLabel      = outcomeLabel;
            resultScreen.outcomeBacking    = bgImg;
            resultScreen.goldLabel         = goldLabel;
            resultScreen.skystonesLabel    = skystonesLabel;
            resultScreen.bookmarksLabel    = bookmarkLabel;
            resultScreen.gearDropsLabel    = gearLabel;
            resultScreen.bonusMessageLabel = bonusLabel;
            resultScreen.retryButton       = retryBtn;
            resultScreen.continueButton    = contBtn;
        }

        // ── shared helpers ────────────────────────────────────────────

        private static Image MakePanel(GameObject parent, string name,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var go  = MakeChild(parent, name);
            var img = go.AddComponent<Image>();
            SetAnchors(go, anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y);
            return img;
        }

        private static TextMeshProUGUI MakeText(GameObject parent, string name,
            float yMin, float yMax, int size, Color col, string text,
            float xMin = 0.04f, float xMax = 0.96f)
        {
            var go  = MakeChild(parent, name);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.fontSize  = size;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color     = col;
            tmp.text      = text;
            SetAnchors(go, xMin, yMin, xMax, yMax);
            return tmp;
        }

        private static GameObject MakeChild(GameObject parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.AddComponent<RectTransform>();
            return go;
        }

        private static void SetAnchors(GameObject go,
            float xMin, float yMin, float xMax, float yMax)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(xMin, yMin);
            rt.anchorMax = new Vector2(xMax, yMax);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
