using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using ChickenShooter.Core;
using ChickenShooter.Data;
using ChickenShooter.Enemies;
using ChickenShooter.UI;

namespace ChickenShooter.EditorTools
{
    public static class BossBootstrap
    {
        const string BossPrefabPath = "Assets/Prefabs/BossLevel1.prefab";

        [MenuItem("ChickenShooter/Bootstrap/Run All Boss Setup")]
        public static void RunAll()
        {
            BuildBossPrefab();
            BuildBossHealthBarUi();
            WireLevel1Boss();
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Boss Prefab")]
        public static void BuildBossPrefab()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Enemies/bossUfoRed.png");
            var eggPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnemyEgg.prefab");
            var minionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnemyBasic.prefab");

            var go = new GameObject("BossLevel1", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Boss));
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            go.transform.localScale = Vector3.one * 2.2f;

            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (sprite != null) col.size = sprite.bounds.size;

            var boss = go.GetComponent<Boss>();
            boss.eggPrefab = eggPrefab;
            boss.minionPrefab = minionPrefab;
            boss.maxHealth = 60;
            boss.scoreValue = 500;

            PrefabUtility.SaveAsPrefabAsset(go, BossPrefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            Debug.Log("BossBootstrap: boss prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Boss Health Bar UI")]
        public static void BuildBossHealthBarUi()
        {
            BuildBossHealthBarUi("Assets/Scenes/Level1.unity");
        }

        public static void BuildBossHealthBarUi(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            if (GameObject.Find("BossHealthBarCanvas") != null)
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log("BossBootstrap: boss health bar UI already present.");
                return;
            }

            var canvasGo = new GameObject("BossHealthBarCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            var barRoot = new GameObject("BossHealthBarRoot", typeof(RectTransform));
            barRoot.transform.SetParent(canvasGo.transform, false);
            var rootRect = barRoot.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 1f);
            rootRect.anchorMax = new Vector2(0.5f, 1f);
            rootRect.anchoredPosition = new Vector2(0f, -80f);
            rootRect.sizeDelta = new Vector2(700f, 40f);

            var bgGo = new GameObject("Background", typeof(Image));
            bgGo.transform.SetParent(barRoot.transform, false);
            var bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgGo.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);

            var fillGo = new GameObject("Fill", typeof(Image));
            fillGo.transform.SetParent(barRoot.transform, false);
            var fillRect = fillGo.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(4f, 4f);
            fillRect.offsetMax = new Vector2(-4f, -4f);
            var fillImage = fillGo.GetComponent<Image>();
            fillImage.color = new Color(0.8f, 0.1f, 0.1f, 1f);
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillAmount = 1f;

            var hud = barRoot.AddComponent<BossHealthBarUI>();
            hud.fillImage = fillImage;
            hud.root = barRoot;

            barRoot.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("BossBootstrap: boss health bar UI built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Wire Level1 Boss")]
        public static void WireLevel1Boss()
        {
            var levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>("Assets/ScriptableObjects/Levels/Level1.asset");
            var bossPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
            levelData.bossPrefab = bossPrefab;
            EditorUtility.SetDirty(levelData);
            AssetDatabase.SaveAssets();

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Level1.unity", OpenSceneMode.Single);

            var controllerGo = GameObject.Find("RoundController");
            var player = GameObject.Find("PlayerShip");
            var healthBarRoot = GameObject.Find("BossHealthBarRoot");

            if (controllerGo != null)
            {
                var roundController = controllerGo.GetComponent<RoundController>();
                roundController.playerTransform = player != null ? player.transform : null;
                roundController.bossHealthBar = healthBarRoot != null ? healthBarRoot.GetComponent<BossHealthBarUI>() : null;
                EditorUtility.SetDirty(controllerGo);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("BossBootstrap: level 1 boss wired.");
        }
    }
}
