using UnityEditor;
using UnityEngine;

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

            AssetDatabase.SaveAssets();
            Debug.Log("EnemyChickenReskinBootstrap: done.");
        }
    }
}
