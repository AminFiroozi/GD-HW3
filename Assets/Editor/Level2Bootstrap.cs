using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ChickenShooter.Combat;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;
using ChickenShooter.Data;
using ChickenShooter.Enemies;
using ChickenShooter.UI;

namespace ChickenShooter.EditorTools
{
    public static class Level2Bootstrap
    {
        const string LevelAssetPath = "Assets/ScriptableObjects/Levels/Level2.asset";
        const string ArmoredEnemyPrefabPath = "Assets/Prefabs/EnemyArmored.prefab";
        const string BossPrefabPath = "Assets/Prefabs/BossLevel2.prefab";

        [MenuItem("ChickenShooter/Bootstrap/Level2/Run All")]
        public static void RunAll()
        {
            BuildData();
            BuildPrefabs();
            WireScene();
        }

        [MenuItem("ChickenShooter/Bootstrap/Level2/Build Data Assets")]
        public static void BuildData()
        {
            const string folder = "Assets/ScriptableObjects";
            EnsureFolder($"{folder}/Waves");
            EnsureFolder($"{folder}/Levels");

            var vWave = CreateOrLoad<WaveDataSO>($"{folder}/Waves/Wave_Level2_V.asset");
            vWave.waveType = WaveType.VFormation;
            vWave.enemyCount = 12;
            vWave.enemySpeedMultiplier = 1.3f;
            vWave.enemyFireRateMultiplier = 1.4f;
            vWave.entrySpeed = 5f;
            vWave.diveBombChance = 0.15f;
            vWave.diveBombIntervalSeconds = 2.5f;
            EditorUtility.SetDirty(vWave);

            var asteroidWave = CreateOrLoad<WaveDataSO>($"{folder}/Waves/Wave_Level2_Asteroids.asset");
            asteroidWave.waveType = WaveType.AsteroidField;
            asteroidWave.enemyCount = 10;
            asteroidWave.enemySpeedMultiplier = 1.1f;
            asteroidWave.entrySpeed = 3.5f;
            EditorUtility.SetDirty(asteroidWave);

            var level2 = CreateOrLoad<LevelDataSO>(LevelAssetPath);
            level2.levelIndex = 2;
            level2.sceneName = "Level2";
            level2.nextLevelSceneName = "Level3";
            level2.rounds = new[]
            {
                vWave, vWave, asteroidWave, vWave, vWave, asteroidWave, vWave
            };
            EditorUtility.SetDirty(level2);

            AssetDatabase.SaveAssets();
            Debug.Log("Level2Bootstrap: data assets built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Level2/Build Prefabs")]
        public static void BuildPrefabs()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Enemies/enemyGreen1.png");
            var eggPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnemyEgg.prefab");
            var foodPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FoodDrop.prefab");
            var giftBoxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WeaponGiftBox.prefab");

            var enemyGo = new GameObject("EnemyArmored", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Enemy));
            enemyGo.GetComponent<SpriteRenderer>().sprite = sprite;

            var enemyCol = enemyGo.GetComponent<BoxCollider2D>();
            enemyCol.isTrigger = true;
            if (sprite != null) enemyCol.size = sprite.bounds.size;

            var enemy = enemyGo.GetComponent<Enemy>();
            enemy.maxHealth = 2;
            enemy.scoreValue = 20;
            enemy.eggDropIntervalMin = 1.5f;
            enemy.eggDropIntervalMax = 3.5f;
            enemy.eggPrefab = eggPrefab;
            enemy.foodPrefab = foodPrefab;
            enemy.giftBoxPrefab = giftBoxPrefab;

            PrefabUtility.SaveAsPrefabAsset(enemyGo, ArmoredEnemyPrefabPath);
            Object.DestroyImmediate(enemyGo);

            var bossSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Enemies/bossUfoYellow.png");
            var minionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArmoredEnemyPrefabPath);

            var bossGo = new GameObject("BossLevel2", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Boss));
            bossGo.GetComponent<SpriteRenderer>().sprite = bossSprite;
            bossGo.transform.localScale = Vector3.one * 2.4f;

            var bossCol = bossGo.GetComponent<BoxCollider2D>();
            bossCol.isTrigger = true;
            if (bossSprite != null) bossCol.size = bossSprite.bounds.size;

            var boss = bossGo.GetComponent<Boss>();
            boss.eggPrefab = eggPrefab;
            boss.minionPrefab = minionPrefab;
            boss.maxHealth = 90;
            boss.scoreValue = 800;
            boss.clusterEggInterval = 2f;
            boss.clusterEggCount = 8;
            boss.diveStrikeInterval = 4f;
            boss.diveSpeed = 6.5f;

            PrefabUtility.SaveAsPrefabAsset(bossGo, BossPrefabPath);
            Object.DestroyImmediate(bossGo);

            var levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(LevelAssetPath);
            levelData.bossPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
            EditorUtility.SetDirty(levelData);

            AssetDatabase.SaveAssets();
            Debug.Log("Level2Bootstrap: prefabs built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Level2/Wire Scene")]
        public static void WireScene()
        {
            UIBootstrap.BuildLevelUi("Assets/Scenes/Level2.unity");
            BossBootstrap.BuildBossHealthBarUi("Assets/Scenes/Level2.unity");

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Level2.unity", OpenSceneMode.Single);

            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PlayerShip.prefab");
            if (GameObject.Find("PlayerShip") == null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, scene);
                instance.transform.position = new Vector3(0f, -4f, 0f);
            }

            var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArmoredEnemyPrefabPath);
            var meteorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MeteorBig.prefab");
            var levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(LevelAssetPath);

            var controllerGo = GameObject.Find("RoundController");
            if (controllerGo == null)
            {
                controllerGo = new GameObject("RoundController", typeof(EnemySpawner), typeof(RoundController));
            }

            var player = GameObject.Find("PlayerShip");

            var spawner = controllerGo.GetComponent<EnemySpawner>();
            spawner.defaultEnemyPrefab = enemyPrefab;
            spawner.defaultMeteorPrefab = meteorPrefab;
            spawner.playerTransform = player != null ? player.transform : null;

            var bossHealthRoot = GameObject.Find("BossHealthBarRoot");

            var roundController = controllerGo.GetComponent<RoundController>();
            roundController.levelData = levelData;
            roundController.spawner = spawner;
            roundController.playerTransform = player != null ? player.transform : null;
            roundController.bossHealthBar = bossHealthRoot != null ? bossHealthRoot.GetComponent<BossHealthBarUI>() : null;

            EditorUtility.SetDirty(controllerGo);

            if (GameObject.Find("PoolManager") == null && GameObject.Find("GameManager") == null)
            {
                var gm = new GameObject("GameManager", typeof(GameManager));
                var pm = new GameObject("PoolManager", typeof(PoolManager));
                EditorUtility.SetDirty(gm);
                EditorUtility.SetDirty(pm);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Level2Bootstrap: scene wired.");
        }

        static T CreateOrLoad<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = path.Substring(0, path.LastIndexOf('/'));
            var leaf = path.Substring(path.LastIndexOf('/') + 1);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
