using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ChickenShooter.Combat;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;
using ChickenShooter.Data;
using ChickenShooter.Enemies;

namespace ChickenShooter.EditorTools
{
    public static class EnemyBootstrap
    {
        const string EggPrefabPath = "Assets/Prefabs/EnemyEgg.prefab";
        const string EnemyPrefabPath = "Assets/Prefabs/EnemyBasic.prefab";

        [MenuItem("ChickenShooter/Bootstrap/Run All Enemy Setup")]
        public static void RunAll()
        {
            BuildEggPrefab();
            BuildFoodPrefab();
            BuildEnemyPrefab();
            BuildMeteorPrefab();
            WireLevel1RoundController();
            WireAsteroidWave();
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Egg Prefab")]
        public static void BuildEggPrefab()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Lasers/eggLaser.png");
            var go = new GameObject("EnemyEgg", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Bullet), typeof(DespawnAfterSeconds));
            go.tag = "EnemyBullet";
            go.GetComponent<SpriteRenderer>().sprite = sprite;

            var rb = go.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (sprite != null) col.size = sprite.bounds.size;

            var bullet = go.GetComponent<Bullet>();
            bullet.movesUp = false;
            bullet.speed = 5f;

            go.GetComponent<DespawnAfterSeconds>().seconds = 4f;

            PrefabUtility.SaveAsPrefabAsset(go, EggPrefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            Debug.Log("EnemyBootstrap: egg prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Enemy Prefab")]
        public static void BuildEnemyPrefab()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Enemies/enemyBlack1.png");
            var eggPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EggPrefabPath);
            var foodPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FoodDrop.prefab");
            var giftBoxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WeaponGiftBox.prefab");

            var go = new GameObject("EnemyBasic", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Enemy));
            go.GetComponent<SpriteRenderer>().sprite = sprite;

            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (sprite != null) col.size = sprite.bounds.size;

            var enemy = go.GetComponent<Enemy>();
            enemy.maxHealth = 1;
            enemy.scoreValue = 10;
            enemy.eggPrefab = eggPrefab;
            enemy.foodPrefab = foodPrefab;
            enemy.giftBoxPrefab = giftBoxPrefab;

            PrefabUtility.SaveAsPrefabAsset(go, EnemyPrefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            Debug.Log("EnemyBootstrap: enemy prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Food Drop Prefab")]
        public static void BuildFoodPrefab()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Food/foodDrop.png");
            var go = new GameObject("FoodDrop", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(FoodDrop), typeof(DespawnAfterSeconds));
            go.tag = "Pickup";
            go.GetComponent<SpriteRenderer>().sprite = sprite;

            var rb = go.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (sprite != null) col.size = sprite.bounds.size;

            go.GetComponent<DespawnAfterSeconds>().seconds = 6f;

            PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/FoodDrop.prefab");
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            Debug.Log("EnemyBootstrap: food drop prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Meteor Prefab")]
        public static void BuildMeteorPrefab()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Meteors/meteorBig.png");
            var go = new GameObject("MeteorBig", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Meteor));
            go.GetComponent<SpriteRenderer>().sprite = sprite;

            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (sprite != null) col.size = sprite.bounds.size;

            PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/MeteorBig.prefab");
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            Debug.Log("EnemyBootstrap: meteor prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Wire Asteroid Wave")]
        public static void WireAsteroidWave()
        {
            var meteorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MeteorBig.prefab");
            var asteroidWave = AssetDatabase.LoadAssetAtPath<WaveDataSO>("Assets/ScriptableObjects/Waves/Wave_Level1_Asteroids.asset");
            asteroidWave.enemyPrefab = meteorPrefab;
            EditorUtility.SetDirty(asteroidWave);
            AssetDatabase.SaveAssets();

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Level1.unity", OpenSceneMode.Single);
            var controllerGo = GameObject.Find("RoundController");
            if (controllerGo != null)
            {
                var spawner = controllerGo.GetComponent<EnemySpawner>();
                spawner.defaultMeteorPrefab = meteorPrefab;
                EditorUtility.SetDirty(controllerGo);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("EnemyBootstrap: asteroid wave wired to meteor prefab.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Wire Level1 Round Controller")]
        public static void WireLevel1RoundController()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Level1.unity", OpenSceneMode.Single);

            var player = GameObject.Find("PlayerShip");
            var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefabPath);
            var levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>("Assets/ScriptableObjects/Levels/Level1.asset");

            var controllerGo = GameObject.Find("RoundController");
            if (controllerGo == null)
            {
                controllerGo = new GameObject("RoundController", typeof(EnemySpawner), typeof(RoundController));
            }

            var spawner = controllerGo.GetComponent<EnemySpawner>();
            spawner.defaultEnemyPrefab = enemyPrefab;
            spawner.playerTransform = player != null ? player.transform : null;

            var roundController = controllerGo.GetComponent<RoundController>();
            roundController.levelData = levelData;
            roundController.spawner = spawner;

            EditorUtility.SetDirty(controllerGo);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("EnemyBootstrap: round controller wired into Level1.");
        }
    }
}
