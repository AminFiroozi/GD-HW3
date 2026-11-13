using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;
using ChickenShooter.Data;
using ChickenShooter.Enemies;
using ChickenShooter.UI;

namespace ChickenShooter.EditorTools
{
    public static class Level3Bootstrap
    {
        const string LevelAssetPath = "Assets/ScriptableObjects/Levels/Level3.asset";
        const string AlienEnemyPrefabPath = "Assets/Prefabs/EnemyAlien.prefab";
        const string BossPrefabPath = "Assets/Prefabs/BossLevel3.prefab";

        [MenuItem("ChickenShooter/Bootstrap/Level3/Run All")]
        public static void RunAll()
        {
            BuildData();
            BuildPrefabs();
            WireScene();
        }

        [MenuItem("ChickenShooter/Bootstrap/Level3/Build Data Assets")]
        public static void BuildData()
        {
            const string folder = "Assets/ScriptableObjects";
            EnsureFolder($"{folder}/Waves");
            EnsureFolder($"{folder}/Levels");

            var tightWave = CreateOrLoad<WaveDataSO>($"{folder}/Waves/Wave_Level3_Tight.asset");
            tightWave.waveType = WaveType.VFormation;
            tightWave.enemyCount = 15;
            tightWave.enemySpeedMultiplier = 1.6f;
            tightWave.enemyFireRateMultiplier = 1.8f;
            tightWave.entrySpeed = 6.5f;
            tightWave.diveBombChance = 0.22f;
            tightWave.diveBombIntervalSeconds = 2f;
            EditorUtility.SetDirty(tightWave);

            var gridWave = CreateOrLoad<WaveDataSO>($"{folder}/Waves/Wave_Level3_Grid.asset");
            gridWave.waveType = WaveType.GridFormation;
            gridWave.enemyCount = 16;
            gridWave.enemySpeedMultiplier = 1.5f;
            gridWave.enemyFireRateMultiplier = 1.7f;
            gridWave.entrySpeed = 6f;
            gridWave.diveBombChance = 0.2f;
            gridWave.diveBombIntervalSeconds = 2.2f;
            EditorUtility.SetDirty(gridWave);

            var asteroidWave = CreateOrLoad<WaveDataSO>($"{folder}/Waves/Wave_Level3_Asteroids.asset");
            asteroidWave.waveType = WaveType.AsteroidField;
            asteroidWave.enemyCount = 14;
            asteroidWave.enemySpeedMultiplier = 1.4f;
            asteroidWave.entrySpeed = 4.5f;
            EditorUtility.SetDirty(asteroidWave);

            var level3 = CreateOrLoad<LevelDataSO>(LevelAssetPath);
            level3.levelIndex = 3;
            level3.sceneName = "Level3";
            level3.nextLevelSceneName = "";
            level3.rounds = new[]
            {
                tightWave, gridWave, tightWave, asteroidWave, gridWave,
                tightWave, asteroidWave, gridWave, tightWave, gridWave
            };
            EditorUtility.SetDirty(level3);

            AssetDatabase.SaveAssets();
            Debug.Log("Level3Bootstrap: data assets built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Level3/Build Prefabs")]
        public static void BuildPrefabs()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Enemies/enemyRed1.png");
            var eggPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnemyEgg.prefab");
            var foodPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FoodDrop.prefab");
            var giftBoxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WeaponGiftBox.prefab");

            var enemyGo = new GameObject("EnemyAlien", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Enemy));
            enemyGo.GetComponent<SpriteRenderer>().sprite = sprite;

            var enemyCol = enemyGo.GetComponent<BoxCollider2D>();
            enemyCol.isTrigger = true;
            if (sprite != null) enemyCol.size = sprite.bounds.size;

            var enemy = enemyGo.GetComponent<Enemy>();
            enemy.maxHealth = 3;
            enemy.scoreValue = 35;
            enemy.eggDropIntervalMin = 1f;
            enemy.eggDropIntervalMax = 2.5f;
            enemy.eggPrefab = eggPrefab;
            enemy.foodPrefab = foodPrefab;
            enemy.giftBoxPrefab = giftBoxPrefab;

            PrefabUtility.SaveAsPrefabAsset(enemyGo, AlienEnemyPrefabPath);
            Object.DestroyImmediate(enemyGo);

            var bossSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Enemies/bossUfoGreen.png");
            var minionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AlienEnemyPrefabPath);

            var bossGo = new GameObject("BossLevel3", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Boss));
            bossGo.GetComponent<SpriteRenderer>().sprite = bossSprite;
            bossGo.transform.localScale = Vector3.one * 2.7f;

            var bossCol = bossGo.GetComponent<BoxCollider2D>();
            bossCol.isTrigger = true;
            if (bossSprite != null) bossCol.size = bossSprite.bounds.size;

            var boss = bossGo.GetComponent<Boss>();
            boss.eggPrefab = eggPrefab;
            boss.minionPrefab = minionPrefab;
            boss.maxHealth = 140;
            boss.scoreValue = 1500;
            boss.clusterEggInterval = 1.5f;
            boss.clusterEggCount = 10;
            boss.diveStrikeInterval = 3f;
            boss.diveSpeed = 7.5f;
            boss.enragedFireMultiplier = 2.5f;
            boss.minionSummonInterval = 4f;

            PrefabUtility.SaveAsPrefabAsset(bossGo, BossPrefabPath);
            Object.DestroyImmediate(bossGo);

            var levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>(LevelAssetPath);
            levelData.bossPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
            EditorUtility.SetDirty(levelData);

            AssetDatabase.SaveAssets();
            Debug.Log("Level3Bootstrap: prefabs built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Level3/Wire Scene")]
        public static void WireScene()
        {
            UIBootstrap.BuildLevelUi("Assets/Scenes/Level3.unity");
            BossBootstrap.BuildBossHealthBarUi("Assets/Scenes/Level3.unity");

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Level3.unity", OpenSceneMode.Single);

            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PlayerShip.prefab");
            if (GameObject.Find("PlayerShip") == null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, scene);
                instance.transform.position = new Vector3(0f, -4f, 0f);
            }

            var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AlienEnemyPrefabPath);
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
            Debug.Log("Level3Bootstrap: scene wired.");
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
