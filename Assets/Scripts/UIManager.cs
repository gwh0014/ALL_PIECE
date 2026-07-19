using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Canvas")]
    public Canvas canvas;

    [Header("HUD Elements")]
    public Text objectiveText;
    public Text treasureText;
    public Text hullText;
    public Slider hullSlider;
    public Text promptText;

    [Header("Screens")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public Button gameOverRestartButton;
    public Button victoryRestartButton;

    private InteractionDetector playerDetector;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Ensure canvas and UI elements exist, generate them programmatically if missing!
        EnsureUIStructure();
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerShip != null)
        {
            playerDetector = GameManager.Instance.playerShip.GetComponent<InteractionDetector>();
        }

        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.AddListener(OnRestartClicked);
        }

        if (victoryRestartButton != null)
        {
            victoryRestartButton.onClick.AddListener(OnRestartClicked);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        // Update HUD
        if (treasureText != null)
        {
            treasureText.text = $"🪙 Treasure: {GameManager.Instance.collectedTreasure} / {GameManager.Instance.targetTreasure}";
        }

        if (objectiveText != null)
        {
            objectiveText.text = $"⚓ Level {GameManager.Instance.currentLevel}: Sail and collect the treasure!";
        }

        if (hullText != null)
        {
            hullText.text = $"Hull Integrity: {Mathf.CeilToInt(GameManager.Instance.currentHullHealth)}%";
        }

        if (hullSlider != null)
        {
            hullSlider.value = GameManager.Instance.currentHullHealth / GameManager.Instance.maxHullHealth;
        }

        // Update interaction prompt
        if (promptText != null)
        {
            if (playerDetector == null && GameManager.Instance.playerShip != null)
            {
                playerDetector = GameManager.Instance.playerShip.GetComponent<InteractionDetector>();
            }

            if (playerDetector != null)
            {
                string prompt = playerDetector.GetCurrentPrompt();
                if (!string.IsNullOrEmpty(prompt))
                {
                    promptText.text = $"Press [Space] to {prompt}";
                    promptText.gameObject.SetActive(true);
                }
                else
                {
                    promptText.gameObject.SetActive(false);
                }
            }
            else
            {
                promptText.gameObject.SetActive(false);
            }
        }

        // Update Screen states
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(GameManager.Instance.currentState == GameManager.GameState.GameOver);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(GameManager.Instance.currentState == GameManager.GameState.Victory);
        }
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    private void EnsureUIStructure()
    {
        // 1. Root Canvas
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("HUD_Canvas");
            canvasObj.transform.SetParent(transform);
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Font selection (Arial fallback or similar standard UI font)
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 2. HUD Panel for text alignments
        GameObject hudPanel = new GameObject("HUD_Panel");
        hudPanel.transform.SetParent(canvas.transform, false);
        RectTransform hudRect = hudPanel.AddComponent<RectTransform>();
        hudRect.anchoredPosition = Vector2.zero;
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.sizeDelta = Vector2.zero;

        // 3. Objective Text (Top Left)
        if (objectiveText == null)
        {
            GameObject objTextObj = new GameObject("ObjectiveText");
            objTextObj.transform.SetParent(hudPanel.transform, false);
            objectiveText = objTextObj.AddComponent<Text>();
            if (defaultFont != null) objectiveText.font = defaultFont;
            objectiveText.fontSize = 24;
            objectiveText.fontStyle = FontStyle.Bold;
            objectiveText.color = Color.white;
            objectiveText.alignment = TextAnchor.UpperLeft;

            RectTransform r = objTextObj.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0, 1);
            r.anchorMax = new Vector2(0, 1);
            r.pivot = new Vector2(0, 1);
            r.anchoredPosition = new Vector2(25f, -25f);
            r.sizeDelta = new Vector2(600f, 40f);

            // Add background drop shadow for readability
            Shadow shadow = objTextObj.AddComponent<Shadow>();
            shadow.effectColor = Color.black;
            shadow.effectDistance = new Vector2(2f, -2f);
        }

        // 4. Treasure Text (Top Right)
        if (treasureText == null)
        {
            GameObject treasureTextObj = new GameObject("TreasureText");
            treasureTextObj.transform.SetParent(hudPanel.transform, false);
            treasureText = treasureTextObj.AddComponent<Text>();
            if (defaultFont != null) treasureText.font = defaultFont;
            treasureText.fontSize = 24;
            treasureText.fontStyle = FontStyle.Bold;
            treasureText.color = Color.yellow;
            treasureText.alignment = TextAnchor.UpperRight;

            RectTransform r = treasureTextObj.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(1, 1);
            r.anchorMax = new Vector2(1, 1);
            r.pivot = new Vector2(1, 1);
            r.anchoredPosition = new Vector2(-50f, -250f);
            r.sizeDelta = new Vector2(400f, 40f);

            Shadow shadow = treasureTextObj.AddComponent<Shadow>();
            shadow.effectColor = Color.black;
            shadow.effectDistance = new Vector2(2f, -2f);
        }

        // 5. Hull Health display (Bottom Left)
        if (hullSlider == null)
        {
            // Slider background
            GameObject sliderObj = new GameObject("Hull_Slider");
            sliderObj.transform.SetParent(hudPanel.transform, false);
            hullSlider = sliderObj.AddComponent<Slider>();

            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0, 0);
            sliderRect.anchorMax = new Vector2(0, 0);
            sliderRect.pivot = new Vector2(0, 0);
            sliderRect.anchoredPosition = new Vector2(25f, 25f);
            sliderRect.sizeDelta = new Vector2(250f, 25f);

            // Slider background image
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.1f, 0.1f, 0.7f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Slider fill area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.9f, 0.2f, 0.2f, 0.9f);
            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;

            hullSlider.fillRect = fillRect;
            hullSlider.minValue = 0f;
            hullSlider.maxValue = 1f;
            hullSlider.value = 1f;

            // Text on top of health bar
            GameObject hullTextObj = new GameObject("HullText");
            hullTextObj.transform.SetParent(sliderObj.transform, false);
            hullText = hullTextObj.AddComponent<Text>();
            if (defaultFont != null) hullText.font = defaultFont;
            hullText.fontSize = 14;
            hullText.fontStyle = FontStyle.Bold;
            hullText.color = Color.white;
            hullText.alignment = TextAnchor.MiddleCenter;

            RectTransform textRect = hullTextObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            Shadow shadow = hullTextObj.AddComponent<Shadow>();
            shadow.effectColor = Color.black;
            shadow.effectDistance = new Vector2(1f, -1f);
        }

        // 6. Interaction Prompt Text (Center Bottom)
        if (promptText == null)
        {
            GameObject pTextObj = new GameObject("PromptText");
            pTextObj.transform.SetParent(hudPanel.transform, false);
            promptText = pTextObj.AddComponent<Text>();
            if (defaultFont != null) promptText.font = defaultFont;
            promptText.fontSize = 20;
            promptText.fontStyle = FontStyle.Bold;
            promptText.color = Color.green;
            promptText.alignment = TextAnchor.MiddleCenter;

            RectTransform r = pTextObj.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0);
            r.anchorMax = new Vector2(0.5f, 0);
            r.pivot = new Vector2(0.5f, 0);
            r.anchoredPosition = new Vector2(0f, 100f);
            r.sizeDelta = new Vector2(500f, 40f);

            Shadow shadow = pTextObj.AddComponent<Shadow>();
            shadow.effectColor = Color.black;
            shadow.effectDistance = new Vector2(1.5f, -1.5f);
        }

        // 7. Game Over Panel
        if (gameOverPanel == null)
        {
            gameOverPanel = CreateOverlaysScreen(canvas.transform, defaultFont, "GAME OVER", "Your ship has sunk!", out gameOverRestartButton);
            gameOverPanel.SetActive(false);
        }

        // 8. Victory Panel
        if (victoryPanel == null)
        {
            victoryPanel = CreateOverlaysScreen(canvas.transform, defaultFont, "VICTORY", "You collected all treasure and became Pirate King!", out victoryRestartButton);
            victoryPanel.SetActive(false);
        }
    }

    private GameObject CreateOverlaysScreen(Transform parent, Font defaultFont, string title, string subtitle, out Button restartBtn)
    {
        GameObject screenPanel = new GameObject(title + "_Panel");
        screenPanel.transform.SetParent(parent, false);
        RectTransform screenRect = screenPanel.AddComponent<RectTransform>();
        screenRect.anchorMin = Vector2.zero;
        screenRect.anchorMax = Vector2.one;
        screenRect.sizeDelta = Vector2.zero;

        Image panelImg = screenPanel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        // Title Text
        GameObject tObj = new GameObject("Title");
        tObj.transform.SetParent(screenPanel.transform, false);
        Text t = tObj.AddComponent<Text>();
        if (defaultFont != null) t.font = defaultFont;
        t.fontSize = 60;
        t.fontStyle = FontStyle.Bold;
        t.color = (title == "VICTORY") ? Color.yellow : Color.red;
        t.alignment = TextAnchor.MiddleCenter;

        RectTransform tr = tObj.GetComponent<RectTransform>();
        tr.anchorMin = new Vector2(0.5f, 0.6f);
        tr.anchorMax = new Vector2(0.5f, 0.6f);
        tr.pivot = new Vector2(0.5f, 0.5f);
        tr.sizeDelta = new Vector2(600f, 80f);

        // Subtitle Text
        GameObject sObj = new GameObject("Subtitle");
        sObj.transform.SetParent(screenPanel.transform, false);
        Text s = sObj.AddComponent<Text>();
        if (defaultFont != null) s.font = defaultFont;
        s.fontSize = 20;
        s.color = Color.white;
        s.alignment = TextAnchor.MiddleCenter;

        RectTransform sr = sObj.GetComponent<RectTransform>();
        sr.anchorMin = new Vector2(0.5f, 0.5f);
        sr.anchorMax = new Vector2(0.5f, 0.5f);
        sr.pivot = new Vector2(0.5f, 0.5f);
        sr.sizeDelta = new Vector2(600f, 40f);

        // Button Container
        GameObject btnObj = new GameObject("Restart_Button");
        btnObj.transform.SetParent(screenPanel.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        restartBtn = btnObj.AddComponent<Button>();

        RectTransform br = btnObj.GetComponent<RectTransform>();
        br.anchorMin = new Vector2(0.5f, 0.35f);
        br.anchorMax = new Vector2(0.5f, 0.35f);
        br.pivot = new Vector2(0.5f, 0.5f);
        br.sizeDelta = new Vector2(200f, 50f);

        // Button Text
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        Text btnText = btnTextObj.AddComponent<Text>();
        if (defaultFont != null) btnText.font = defaultFont;
        btnText.fontSize = 18;
        btnText.fontStyle = FontStyle.Bold;
        btnText.color = Color.white;
        btnText.text = "SAIL AGAIN";
        btnText.alignment = TextAnchor.MiddleCenter;

        RectTransform btr = btnTextObj.GetComponent<RectTransform>();
        btr.anchorMin = Vector2.zero;
        btr.anchorMax = Vector2.one;
        btr.sizeDelta = Vector2.zero;

        return screenPanel;
    }
}

