using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ChickenShooter.EditorTools
{
    public static class UiRestyleBootstrap
    {
        const string UiDir = "Assets/Sprites/UI/";
        const float HeaderHeight = 68f;

        static Font titleFont;
        static Font bodyFont;

        public static void Run()
        {
            SetupSprite(UiDir + "windowBlue.png", new Vector4(36f, 36f, 36f, HeaderHeight));
            SetupSprite(UiDir + "buttonBlue.png", new Vector4(14f, 14f, 14f, 14f));
            SetupSprite(UiDir + "buttonGreen.png", new Vector4(14f, 14f, 14f, 14f));
            SetupSprite(UiDir + "buttonRed.png", new Vector4(14f, 14f, 14f, 14f));
            SetupSprite(UiDir + "barBack.png", new Vector4(12f, 12f, 12f, 12f));
            SetupSprite(UiDir + "barYellow.png", new Vector4(10f, 10f, 10f, 10f));
            SetupSprite(UiDir + "barBlue.png", new Vector4(10f, 10f, 10f, 10f));
            SetupSprite(UiDir + "barRed.png", new Vector4(10f, 10f, 10f, 10f));

            AssetDatabase.ImportAsset("Assets/Fonts/KenneyFuture.ttf", ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset("Assets/Fonts/KenneyFutureNarrow.ttf", ImportAssetOptions.ForceUpdate);
            titleFont = Require(AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/KenneyFuture.ttf"), "title font");
            bodyFont = Require(AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/KenneyFutureNarrow.ttf"), "body font");

            RestyleMainMenu();
            foreach (var path in new[] { "Assets/Scenes/Level1.unity", "Assets/Scenes/Level2.unity", "Assets/Scenes/Level3.unity" })
            {
                RestyleLevel(path);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("UiRestyleBootstrap: done.");
        }

        static T Require<T>(T value, string what) where T : class
        {
            if (value == null || value.Equals(null)) throw new InvalidOperationException("UiRestyleBootstrap: missing " + what);
            return value;
        }

        static void SetupSprite(string path, Vector4 border)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = Require(AssetImporter.GetAtPath(path) as TextureImporter, "importer for " + path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.spriteBorder = border;
            importer.SaveAndReimport();
        }

        static Sprite LoadSprite(string name)
        {
            return Require(AssetDatabase.LoadAssetAtPath<Sprite>(UiDir + name), "sprite " + name);
        }

        static Transform Child(Transform parent, string name)
        {
            return Require(parent.Find(name), "child " + name + " under " + parent.name);
        }

        static Transform CanvasRoot(string name)
        {
            return Require(GameObject.Find(name), "canvas " + name).transform;
        }

        static void RestyleMainMenu()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);
            var canvas = CanvasRoot("MainMenuCanvas");
            var panel = Child(canvas, "Panel");

            var backdrop = canvas.Find("Backdrop");
            if (backdrop == null)
            {
                var go = new GameObject("Backdrop", typeof(Image));
                go.transform.SetParent(canvas, false);
                backdrop = go.transform;
            }
            backdrop.SetAsFirstSibling();
            Stretch((RectTransform)backdrop);
            var backdropImage = backdrop.GetComponent<Image>();
            backdropImage.sprite = Require(AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Backgrounds/parallaxFarLevel1.png"), "menu backdrop sprite");
            backdropImage.color = Color.white;
            backdropImage.raycastTarget = false;

            panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.25f);

            StyleWindow(panel, 520f, 520f, 50f);
            StyleTitle(Child(panel, "Title"), 276f, 480f, 34);

            StyleButton(Child(panel, "NewGameButton"), "buttonGreen.png", 170f);
            StyleButton(Child(panel, "ContinueButton"), "buttonBlue.png", 90f);
            StyleButton(Child(panel, "QuitButton"), "buttonRed.png", -110f);

            var volumeLabel = EnsureText(panel, "VolumeLabel", "Volume");
            var volumeRect = (RectTransform)volumeLabel.transform;
            CenterAnchor(volumeRect, new Vector2(0f, 26f), new Vector2(300f, 36f));
            volumeLabel.font = bodyFont;
            volumeLabel.fontSize = 24;
            volumeLabel.alignment = TextAnchor.MiddleCenter;
            volumeLabel.color = new Color(0.16f, 0.2f, 0.32f);

            var slider = Child(panel, "VolumeSlider");
            CenterAnchor((RectTransform)slider, new Vector2(0f, -14f), new Vector2(300f, 32f));
            StyleBar(Child(slider, "Background").GetComponent<Image>(), "barBack.png");
            var fillArea = (RectTransform)Child(slider, "Fill Area");
            fillArea.anchorMin = Vector2.zero;
            fillArea.anchorMax = Vector2.one;
            fillArea.offsetMin = new Vector2(4f, 4f);
            fillArea.offsetMax = new Vector2(-4f, -4f);
            StyleBar(Child(fillArea, "Fill").GetComponent<Image>(), "barBlue.png");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        static void RestyleLevel(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            var hud = CanvasRoot("HudCanvas");
            StyleHudText(Child(hud, "RoundText"), true, -30f);
            StyleHudText(Child(hud, "LivesText"), true, -78f);
            StyleHudText(Child(hud, "ScoreText"), false, -30f);
            StyleHudText(Child(hud, "MissilesText"), false, -78f);

            var foodBar = Child(hud, "FoodBar");
            var foodRect = (RectTransform)foodBar;
            foodRect.anchoredPosition = new Vector2(0f, 50f);
            foodRect.sizeDelta = new Vector2(420f, 32f);
            StyleBar(foodBar.GetComponent<Image>(), "barBack.png");
            StyleFill(Child(foodBar, "Fill").GetComponent<Image>(), "barYellow.png");

            var foodLabel = EnsureText(hud, "FoodLabel", "Food");
            var foodLabelRect = (RectTransform)foodLabel.transform;
            foodLabelRect.anchorMin = new Vector2(0.5f, 0f);
            foodLabelRect.anchorMax = new Vector2(0.5f, 0f);
            foodLabelRect.pivot = new Vector2(0.5f, 0.5f);
            foodLabelRect.anchoredPosition = new Vector2(0f, 88f);
            foodLabelRect.sizeDelta = new Vector2(300f, 30f);
            foodLabel.font = bodyFont;
            foodLabel.fontSize = 22;
            foodLabel.alignment = TextAnchor.MiddleCenter;
            foodLabel.color = Color.white;
            foodLabel.raycastTarget = false;
            EnsureOutline(foodLabel.gameObject);

            var bossRoot = Child(CanvasRoot("BossHealthBarCanvas"), "BossHealthBarRoot");
            StyleBar(Child(bossRoot, "Background").GetComponent<Image>(), "barBack.png");
            StyleFill(Child(bossRoot, "Fill").GetComponent<Image>(), "barRed.png");

            var pausePanel = Child(CanvasRoot("PauseCanvas"), "PausePanel");
            StyleWindow(pausePanel, 460f, 340f, 10f);
            StyleTitle(Child(pausePanel, "Title"), 146f, 420f, 34);
            StyleButton(Child(pausePanel, "ResumeButton"), "buttonBlue.png", 40f);
            StyleButton(Child(pausePanel, "QuitToMenuButton"), "buttonRed.png", -50f);

            var gameOverPanel = Child(CanvasRoot("GameOverCanvas"), "GameOverPanel");
            StyleWindow(gameOverPanel, 460f, 340f, 10f);
            StyleTitle(Child(gameOverPanel, "Title"), 146f, 420f, 34);
            StyleButton(Child(gameOverPanel, "RetryButton"), "buttonGreen.png", 40f);
            StyleButton(Child(gameOverPanel, "QuitToMenuButton"), "buttonRed.png", -50f);

            var victoryPanel = Child(CanvasRoot("VictoryCanvas"), "VictoryPanel");
            StyleWindow(victoryPanel, 460f, 260f, 0f);
            StyleTitle(Child(victoryPanel, "Title"), 96f, 420f, 34);
            StyleButton(Child(victoryPanel, "MainMenuButton"), "buttonBlue.png", -34f);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static void CenterAnchor(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        static Text EnsureText(Transform parent, string name, string content)
        {
            var existing = parent.Find(name);
            if (existing == null)
            {
                var go = new GameObject(name, typeof(Text));
                go.transform.SetParent(parent, false);
                existing = go.transform;
            }
            var text = existing.GetComponent<Text>();
            text.text = content;
            return text;
        }

        static void EnsureOutline(GameObject go)
        {
            var outline = go.GetComponent<Outline>();
            if (outline == null) outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.9f);
            outline.effectDistance = new Vector2(2f, -2f);
        }

        static void StyleWindow(Transform panel, float width, float height, float centerY)
        {
            var window = panel.Find("Window");
            if (window == null)
            {
                var go = new GameObject("Window", typeof(Image));
                go.transform.SetParent(panel, false);
                window = go.transform;
            }
            window.SetAsFirstSibling();
            CenterAnchor((RectTransform)window, new Vector2(0f, centerY), new Vector2(width, height));
            var image = window.GetComponent<Image>();
            image.sprite = LoadSprite("windowBlue.png");
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;
        }

        static void StyleTitle(Transform title, float y, float width, int fontSize)
        {
            CenterAnchor((RectTransform)title, new Vector2(0f, y), new Vector2(width, HeaderHeight - 8f));
            var text = title.GetComponent<Text>();
            text.font = titleFont;
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.raycastTarget = false;
        }

        static void StyleButton(Transform button, string spriteName, float y)
        {
            var size = new Vector2(300f, 64f);
            CenterAnchor((RectTransform)button, new Vector2(0f, y), size);
            var image = button.GetComponent<Image>();
            image.sprite = LoadSprite(spriteName);
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            var label = Child(button, "Label");
            CenterAnchor((RectTransform)label, Vector2.zero, size);
            var text = label.GetComponent<Text>();
            text.font = bodyFont;
            text.fontSize = 26;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.raycastTarget = false;

            var shadow = label.GetComponent<Shadow>();
            if (shadow == null) shadow = label.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);
        }

        static void StyleBar(Image image, string spriteName)
        {
            image.sprite = LoadSprite(spriteName);
            image.type = Image.Type.Sliced;
            image.color = Color.white;
        }

        static void StyleFill(Image image, string spriteName)
        {
            var amount = image.fillAmount;
            image.sprite = LoadSprite(spriteName);
            image.color = Color.white;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            image.fillAmount = amount;
            var rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(5f, 5f);
            rect.offsetMax = new Vector2(-5f, -5f);
        }

        static void StyleHudText(Transform t, bool left, float y)
        {
            var rect = (RectTransform)t;
            var corner = new Vector2(left ? 0f : 1f, 1f);
            rect.anchorMin = corner;
            rect.anchorMax = corner;
            rect.pivot = corner;
            rect.anchoredPosition = new Vector2(left ? 40f : -40f, y);
            rect.sizeDelta = new Vector2(520f, 44f);

            var text = t.GetComponent<Text>();
            text.font = bodyFont;
            text.fontSize = 30;
            text.alignment = left ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.raycastTarget = false;
            EnsureOutline(t.gameObject);
        }
    }
}
