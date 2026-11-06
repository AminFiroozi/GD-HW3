using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using ChickenShooter.UI;

namespace ChickenShooter.EditorTools
{
    public static class UIBootstrap
    {
        [MenuItem("ChickenShooter/Bootstrap/Run All UI Setup")]
        public static void RunAll()
        {
            BuildMainMenu();
            BuildLevelUi();
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Main Menu UI")]
        public static void BuildMainMenu()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);

            EnsureEventSystem();
            var canvas = EnsureCanvas("MainMenuCanvas");

            var panel = CreatePanel(canvas.transform, "Panel", new Color(0f, 0f, 0f, 0.4f));

            var title = CreateText(panel.transform, "Title", "Chicken Shooter", 48, new Vector2(0f, 260f), new Vector2(500f, 80f));

            var newGameBtn = CreateButton(panel.transform, "NewGameButton", "New Game", new Vector2(0f, 100f));
            var continueBtn = CreateButton(panel.transform, "ContinueButton", "Continue", new Vector2(0f, 20f));
            var quitBtn = CreateButton(panel.transform, "QuitButton", "Quit", new Vector2(0f, -140f));

            var sliderGo = CreateSlider(panel.transform, "VolumeSlider", new Vector2(0f, -60f));

            var menuGo = GameObject.Find("MainMenuController");
            if (menuGo == null) menuGo = new GameObject("MainMenuController");
            var menu = menuGo.GetComponent<MainMenuUI>();
            if (menu == null) menu = menuGo.AddComponent<MainMenuUI>();

            menu.newGameButton = newGameBtn.GetComponent<Button>();
            menu.continueButton = continueBtn.GetComponent<Button>();
            menu.quitButton = quitBtn.GetComponent<Button>();
            menu.volumeSlider = sliderGo.GetComponent<Slider>();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("UIBootstrap: main menu UI built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Level UI")]
        public static void BuildLevelUi()
        {
            BuildLevelUi("Assets/Scenes/Level1.unity");
        }

        public static void BuildLevelUi(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            EnsureEventSystem();
            BuildHud();
            BuildPauseMenu();
            BuildGameOverScreen();
            BuildVictoryScreen();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("UIBootstrap: level UI built.");
        }

        static void BuildHud()
        {
            var canvas = EnsureCanvas("HudCanvas");

            var roundText = CreateText(canvas.transform, "RoundText", "Level 1 - Round 1", 28, new Vector2(-350f, -50f), new Vector2(400f, 50f));
            var scoreText = CreateText(canvas.transform, "ScoreText", "Score: 0", 28, new Vector2(350f, -50f), new Vector2(300f, 50f));
            var livesText = CreateText(canvas.transform, "LivesText", "Lives: 3", 28, new Vector2(-350f, -100f), new Vector2(300f, 50f));
            var missilesText = CreateText(canvas.transform, "MissilesText", "Missiles: 2", 28, new Vector2(350f, -100f), new Vector2(300f, 50f));

            var foodBarGo = CreatePanel(canvas.transform, "FoodBar", new Color(0.1f, 0.1f, 0.1f, 0.6f));
            var foodBarRect = foodBarGo.GetComponent<RectTransform>();
            foodBarRect.anchorMin = new Vector2(0.5f, 0f);
            foodBarRect.anchorMax = new Vector2(0.5f, 0f);
            foodBarRect.anchoredPosition = new Vector2(0f, 60f);
            foodBarRect.sizeDelta = new Vector2(400f, 24f);

            var fillGo = new GameObject("Fill", typeof(Image));
            fillGo.transform.SetParent(foodBarGo.transform, false);
            var fillRect = fillGo.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2f, 2f);
            fillRect.offsetMax = new Vector2(-2f, -2f);
            var fillImage = fillGo.GetComponent<Image>();
            fillImage.color = new Color(1f, 0.7f, 0f, 1f);
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillAmount = 0f;

            var hudGo = GameObject.Find("HudController");
            if (hudGo == null) hudGo = new GameObject("HudController");
            var hud = hudGo.GetComponent<HudUI>();
            if (hud == null) hud = hudGo.AddComponent<HudUI>();

            hud.roundText = roundText.GetComponent<Text>();
            hud.scoreText = scoreText.GetComponent<Text>();
            hud.livesText = livesText.GetComponent<Text>();
            hud.missilesText = missilesText.GetComponent<Text>();
            hud.foodBarFill = fillImage;
        }

        public static void BuildPauseMenu(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            BuildPauseMenu();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        static void BuildPauseMenu()
        {
            var canvas = EnsureCanvas("PauseCanvas");
            var panel = CreatePanel(canvas.transform, "PausePanel", new Color(0f, 0f, 0f, 0.7f));

            CreateText(panel.transform, "Title", "Paused", 40, new Vector2(0f, 120f), new Vector2(400f, 60f));
            var resumeBtn = CreateButton(panel.transform, "ResumeButton", "Resume", new Vector2(0f, 20f));
            var quitBtn = CreateButton(panel.transform, "QuitToMenuButton", "Quit To Menu", new Vector2(0f, -60f));

            var controllerGo = GameObject.Find("PauseController");
            if (controllerGo == null) controllerGo = new GameObject("PauseController");
            var pause = controllerGo.GetComponent<PauseMenuUI>();
            if (pause == null) pause = controllerGo.AddComponent<PauseMenuUI>();

            pause.panel = panel;
            pause.resumeButton = resumeBtn.GetComponent<Button>();
            pause.quitToMenuButton = quitBtn.GetComponent<Button>();

            panel.SetActive(false);
        }

        public static void BuildGameOverScreen(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            BuildGameOverScreen();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        static void BuildGameOverScreen()
        {
            var canvas = EnsureCanvas("GameOverCanvas");
            var panel = CreatePanel(canvas.transform, "GameOverPanel", new Color(0.2f, 0f, 0f, 0.8f));

            CreateText(panel.transform, "Title", "Game Over", 44, new Vector2(0f, 120f), new Vector2(400f, 60f));
            var retryBtn = CreateButton(panel.transform, "RetryButton", "Retry", new Vector2(0f, 20f));
            var quitBtn = CreateButton(panel.transform, "QuitToMenuButton", "Main Menu", new Vector2(0f, -60f));

            var controllerGo = GameObject.Find("GameOverController");
            if (controllerGo == null) controllerGo = new GameObject("GameOverController");
            var gameOver = controllerGo.GetComponent<GameOverUI>();
            if (gameOver == null) gameOver = controllerGo.AddComponent<GameOverUI>();

            gameOver.panel = panel;
            gameOver.retryButton = retryBtn.GetComponent<Button>();
            gameOver.quitToMenuButton = quitBtn.GetComponent<Button>();

            panel.SetActive(false);
        }

        public static void BuildVictoryScreen(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            BuildVictoryScreen();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        static void BuildVictoryScreen()
        {
            var canvas = EnsureCanvas("VictoryCanvas");
            var panel = CreatePanel(canvas.transform, "VictoryPanel", new Color(0f, 0.2f, 0f, 0.85f));

            CreateText(panel.transform, "Title", "Victory!", 48, new Vector2(0f, 60f), new Vector2(400f, 80f));
            var mainMenuBtn = CreateButton(panel.transform, "MainMenuButton", "Main Menu", new Vector2(0f, -60f));

            var controllerGo = GameObject.Find("VictoryController");
            if (controllerGo == null) controllerGo = new GameObject("VictoryController");
            var victory = controllerGo.GetComponent<VictoryUI>();
            if (victory == null) victory = controllerGo.AddComponent<VictoryUI>();

            victory.panel = panel;
            victory.mainMenuButton = mainMenuBtn.GetComponent<Button>();
            panel.SetActive(false);
        }

        static Canvas EnsureCanvas(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing.GetComponent<Canvas>();

            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var existing = parent.Find(name);
            if (existing != null) return existing.gameObject;

            var go = new GameObject(name, typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            return go;
        }

        static GameObject CreateText(Transform parent, string name, string text, int fontSize, Vector2 anchoredPos, Vector2 size)
        {
            var existingText = parent.Find(name);
            if (existingText != null) return existingText.gameObject;

            var go = new GameObject(name, typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;

            var t = go.GetComponent<Text>();
            t.text = text;
            t.fontSize = fontSize;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return go;
        }

        static GameObject CreateButton(Transform parent, string name, string label, Vector2 anchoredPos)
        {
            var existingButton = parent.Find(name);
            if (existingButton != null) return existingButton.gameObject;

            var go = new GameObject(name, typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(260f, 60f);

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/buttonBlue.png");
            var image = go.GetComponent<Image>();
            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Sliced;
            }

            var labelGo = CreateText(go.transform, "Label", label, 26, Vector2.zero, rect.sizeDelta);
            labelGo.GetComponent<Text>().color = new Color(0.1f, 0.12f, 0.2f);

            return go;
        }

        static GameObject CreateSlider(Transform parent, string name, Vector2 anchoredPos)
        {
            var existing = parent.Find(name);
            if (existing != null) return existing.gameObject;

            var go = new GameObject(name, typeof(Slider));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(260f, 30f);

            var bgGo = new GameObject("Background", typeof(Image));
            bgGo.transform.SetParent(go.transform, false);
            var bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgGo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.2f);

            var fillAreaGo = new GameObject("Fill Area", typeof(RectTransform));
            fillAreaGo.transform.SetParent(go.transform, false);
            var fillAreaRect = fillAreaGo.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRect.offsetMin = new Vector2(5f, 0f);
            fillAreaRect.offsetMax = new Vector2(-5f, 0f);

            var fillGo = new GameObject("Fill", typeof(Image));
            fillGo.transform.SetParent(fillAreaGo.transform, false);
            var fillRect = fillGo.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            fillGo.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f, 1f);

            var slider = go.GetComponent<Slider>();
            slider.fillRect = fillRect;
            slider.targetGraphic = fillGo.GetComponent<Image>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            return go;
        }
    }
}
