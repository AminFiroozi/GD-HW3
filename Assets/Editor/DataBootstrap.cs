using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ChickenShooter.Core;
using ChickenShooter.Data;

namespace ChickenShooter.EditorTools
{
    public static class DataBootstrap
    {
        const string DataFolder = "Assets/ScriptableObjects";

        [MenuItem("ChickenShooter/Bootstrap/Create Data Assets")]
        public static void CreateDataAssets()
        {
            EnsureFolder(DataFolder);
            EnsureFolder($"{DataFolder}/Weapons");
            EnsureFolder($"{DataFolder}/Waves");
            EnsureFolder($"{DataFolder}/Levels");

            var weapon1 = CreateOrLoad<WeaponDataSO>($"{DataFolder}/Weapons/Weapon_Level1.asset");
            weapon1.weaponName = "Single Laser";
            weapon1.powerLevel = 1;
            weapon1.damage = 1;
            weapon1.bulletSpeed = 12f;
            weapon1.fireRate = 0.25f;
            weapon1.spawnOffsets = new[] { Vector2.zero };
            weapon1.spreadAngles = new[] { 0f };
            EditorUtility.SetDirty(weapon1);

            var weapon2 = CreateOrLoad<WeaponDataSO>($"{DataFolder}/Weapons/Weapon_Level2.asset");
            weapon2.weaponName = "Double Laser";
            weapon2.powerLevel = 2;
            weapon2.damage = 1;
            weapon2.bulletSpeed = 13f;
            weapon2.fireRate = 0.2f;
            weapon2.spawnOffsets = new[] { new Vector2(-0.2f, 0f), new Vector2(0.2f, 0f) };
            weapon2.spreadAngles = new[] { 0f, 0f };
            EditorUtility.SetDirty(weapon2);

            var weapon3 = CreateOrLoad<WeaponDataSO>($"{DataFolder}/Weapons/Weapon_Level3.asset");
            weapon3.weaponName = "Spread Laser";
            weapon3.powerLevel = 3;
            weapon3.damage = 1;
            weapon3.bulletSpeed = 13f;
            weapon3.fireRate = 0.18f;
            weapon3.spawnOffsets = new[] { Vector2.zero, Vector2.zero, Vector2.zero };
            weapon3.spreadAngles = new[] { -15f, 0f, 15f };
            EditorUtility.SetDirty(weapon3);

            var basicWave = CreateOrLoad<WaveDataSO>($"{DataFolder}/Waves/Wave_Level1_Grid.asset");
            basicWave.waveType = WaveType.GridFormation;
            basicWave.enemyCount = 10;
            basicWave.enemySpeedMultiplier = 1f;
            basicWave.enemyFireRateMultiplier = 1f;
            basicWave.entrySpeed = 4f;
            basicWave.diveBombChance = 0.1f;
            basicWave.diveBombIntervalSeconds = 3f;
            EditorUtility.SetDirty(basicWave);

            var asteroidWave = CreateOrLoad<WaveDataSO>($"{DataFolder}/Waves/Wave_Level1_Asteroids.asset");
            asteroidWave.waveType = WaveType.AsteroidField;
            asteroidWave.enemyCount = 8;
            asteroidWave.enemySpeedMultiplier = 0.8f;
            asteroidWave.entrySpeed = 3f;
            EditorUtility.SetDirty(asteroidWave);

            var level1 = CreateOrLoad<LevelDataSO>($"{DataFolder}/Levels/Level1.asset");
            level1.levelIndex = 1;
            level1.sceneName = "Level1";
            level1.nextLevelSceneName = "Level2";
            level1.rounds = new[]
            {
                basicWave, basicWave, basicWave, asteroidWave, basicWave
            };
            EditorUtility.SetDirty(level1);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("DataBootstrap: data assets created.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Wire GameManager In MainMenu")]
        public static void WireGameManager()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);

            var existing = GameObject.Find("GameManager");
            if (existing == null)
            {
                var go = new GameObject("GameManager", typeof(GameManager));
                EditorUtility.SetDirty(go);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("DataBootstrap: GameManager wired into MainMenu scene.");
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
