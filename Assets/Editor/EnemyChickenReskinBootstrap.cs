using System;
using UnityEditor;
using UnityEngine;
using ChickenShooter.Enemies;
using ChickenShooter.VFX;

namespace ChickenShooter.EditorTools
{
    public static class EnemyChickenReskinBootstrap
    {
        public static void Run()
        {
            foreach (var path in new[]
            {
                "Assets/Sprites/Enemies/enemyBlack1.png",
                "Assets/Sprites/Enemies/enemyGreen1.png",
                "Assets/Sprites/Enemies/enemyRed1.png",
                "Assets/Sprites/Enemies/bossUfoRed.png",
                "Assets/Sprites/Enemies/bossUfoYellow.png",
                "Assets/Sprites/Enemies/bossUfoGreen.png",
            })
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            EnemyBootstrap.BuildEnemyPrefab();
            BossBootstrap.BuildBossPrefab();
            Level2Bootstrap.BuildPrefabs();
            Level3Bootstrap.BuildPrefabs();

            AudioBootstrap.WirePrefabSounds();
            RewireVfx();

            AssetDatabase.SaveAssets();
            Debug.Log("EnemyChickenReskinBootstrap: done.");
        }

        static void RewireVfx()
        {
            var explosionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ExplosionBurst.prefab");
            if (explosionPrefab == null) throw new InvalidOperationException("EnemyChickenReskinBootstrap: explosion prefab missing");

            foreach (var path in new[] { "Assets/Prefabs/EnemyBasic.prefab", "Assets/Prefabs/EnemyArmored.prefab", "Assets/Prefabs/EnemyAlien.prefab" })
            {
                using var scope = new PrefabUtility.EditPrefabContentsScope(path);
                var root = scope.prefabContentsRoot;
                root.GetComponent<Enemy>().explosionPrefab = explosionPrefab;
                if (root.GetComponent<DamageFlash>() == null) root.AddComponent<DamageFlash>();
            }

            foreach (var path in new[] { "Assets/Prefabs/BossLevel1.prefab", "Assets/Prefabs/BossLevel2.prefab", "Assets/Prefabs/BossLevel3.prefab" })
            {
                using var scope = new PrefabUtility.EditPrefabContentsScope(path);
                var root = scope.prefabContentsRoot;
                root.GetComponent<Boss>().explosionPrefab = explosionPrefab;
                if (root.GetComponent<DamageFlash>() == null) root.AddComponent<DamageFlash>();
            }
        }
    }
}
