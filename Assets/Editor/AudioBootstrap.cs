using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ChickenShooter.Combat;
using ChickenShooter.Core;
using ChickenShooter.Data;
using ChickenShooter.Enemies;
using ChickenShooter.UI;

namespace ChickenShooter.EditorTools
{
    public static class AudioBootstrap
    {
        [MenuItem("ChickenShooter/Bootstrap/Audio/Run All")]
        public static void RunAll()
        {
            BuildAudioManager();
            WireWeaponSounds();
            WireLevelMusic();
            WireMenuAndVictoryMusic();
            WirePrefabSounds();
        }

        [MenuItem("ChickenShooter/Bootstrap/Audio/Build Audio Manager")]
        public static void BuildAudioManager()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);

            var existing = GameObject.Find("AudioManager");
            if (existing == null)
            {
                var go = new GameObject("AudioManager", typeof(AudioManager));
                var music = new GameObject("MusicSource", typeof(AudioSource));
                music.transform.SetParent(go.transform);
                var musicSource = music.GetComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;

                var sfx = new GameObject("SfxSource", typeof(AudioSource));
                sfx.transform.SetParent(go.transform);
                var sfxSource = sfx.GetComponent<AudioSource>();
                sfxSource.playOnAwake = false;

                var manager = go.GetComponent<AudioManager>();
                manager.musicSource = musicSource;
                manager.sfxSource = sfxSource;

                EditorUtility.SetDirty(go);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("AudioBootstrap: audio manager built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Audio/Wire Weapon Sounds")]
        public static void WireWeaponSounds()
        {
            SetFireSound("Assets/ScriptableObjects/Weapons/Weapon_Level1.asset", "Assets/Audio/SFX/weaponFireLevel1.ogg");
            SetFireSound("Assets/ScriptableObjects/Weapons/Weapon_Level2.asset", "Assets/Audio/SFX/weaponFireLevel2.ogg");
            SetFireSound("Assets/ScriptableObjects/Weapons/Weapon_Level3.asset", "Assets/Audio/SFX/weaponFireLevel3.ogg");
            AssetDatabase.SaveAssets();
            Debug.Log("AudioBootstrap: weapon sounds wired.");
        }

        static void SetFireSound(string weaponPath, string clipPath)
        {
            var weapon = AssetDatabase.LoadAssetAtPath<WeaponDataSO>(weaponPath);
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);
            weapon.fireSound = clip;
            EditorUtility.SetDirty(weapon);
        }

        [MenuItem("ChickenShooter/Bootstrap/Audio/Wire Level Music")]
        public static void WireLevelMusic()
        {
            SetLevelMusic("Assets/ScriptableObjects/Levels/Level1.asset", "Assets/Audio/Music/Level1Music.ogg");
            SetLevelMusic("Assets/ScriptableObjects/Levels/Level2.asset", "Assets/Audio/Music/Level2Music.ogg");
            SetLevelMusic("Assets/ScriptableObjects/Levels/Level3.asset", "Assets/Audio/Music/Level3Music.ogg");
            AssetDatabase.SaveAssets();
            Debug.Log("AudioBootstrap: level music wired.");
        }

        static void SetLevelMusic(string levelPath, string clipPath)
        {
            var level = AssetDatabase.LoadAssetAtPath<LevelDataSO>(levelPath);
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);
            level.backgroundMusic = clip;
            EditorUtility.SetDirty(level);
        }

        [MenuItem("ChickenShooter/Bootstrap/Audio/Wire Menu And Victory Music")]
        public static void WireMenuAndVictoryMusic()
        {
            var menuClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music/MainMenuMusic.ogg");
            var victoryClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music/VictoryMusic.ogg");

            var menuScene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);
            var menuController = GameObject.Find("MainMenuController");
            if (menuController != null)
            {
                menuController.GetComponent<MainMenuUI>().menuMusic = menuClip;
                EditorUtility.SetDirty(menuController);
            }
            EditorSceneManager.MarkSceneDirty(menuScene);
            EditorSceneManager.SaveScene(menuScene);

            foreach (var scenePath in new[] { "Assets/Scenes/Level1.unity", "Assets/Scenes/Level2.unity", "Assets/Scenes/Level3.unity" })
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var victoryController = GameObject.Find("VictoryController");
                if (victoryController != null)
                {
                    victoryController.GetComponent<VictoryUI>().victoryMusic = victoryClip;
                    EditorUtility.SetDirty(victoryController);
                }
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            Debug.Log("AudioBootstrap: menu and victory music wired.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Audio/Wire Prefab Sounds")]
        public static void WirePrefabSounds()
        {
            var eggHatch = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/eggHatch.ogg");
            var explosion = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/explosion.ogg");
            var bossEnrage = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/bossEnrage.ogg");
            var playerHit = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/playerHit.ogg");
            var pickupFood = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/pickupFood.ogg");
            var pickupGift = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/pickupGift.ogg");
            var missileExplosion = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/missileExplosion.ogg");

            foreach (var path in new[] { "Assets/Prefabs/EnemyBasic.prefab", "Assets/Prefabs/EnemyArmored.prefab", "Assets/Prefabs/EnemyAlien.prefab" })
            {
                using var scope = new PrefabUtility.EditPrefabContentsScope(path);
                var enemy = scope.prefabContentsRoot.GetComponent<Enemy>();
                enemy.eggDropSound = eggHatch;
                enemy.deathSound = explosion;
            }

            foreach (var path in new[] { "Assets/Prefabs/BossLevel1.prefab", "Assets/Prefabs/BossLevel2.prefab", "Assets/Prefabs/BossLevel3.prefab" })
            {
                using var scope = new PrefabUtility.EditPrefabContentsScope(path);
                var boss = scope.prefabContentsRoot.GetComponent<Boss>();
                boss.enrageSound = bossEnrage;
                boss.deathSound = explosion;
            }

            using (var scope = new PrefabUtility.EditPrefabContentsScope("Assets/Prefabs/FoodDrop.prefab"))
            {
                scope.prefabContentsRoot.GetComponent<FoodDrop>().pickupSound = pickupFood;
            }

            using (var scope = new PrefabUtility.EditPrefabContentsScope("Assets/Prefabs/WeaponGiftBox.prefab"))
            {
                scope.prefabContentsRoot.GetComponent<WeaponGiftBox>().pickupSound = pickupGift;
            }

            using (var scope = new PrefabUtility.EditPrefabContentsScope("Assets/Prefabs/PlayerShip.prefab"))
            {
                var root = scope.prefabContentsRoot;
                root.GetComponent<Player.PlayerShip>().hitSound = playerHit;

                var missile = root.GetComponent<SecondaryMissileController>();
                if (missile == null) missile = root.AddComponent<SecondaryMissileController>();
                missile.explosionSound = missileExplosion;
            }

            AssetDatabase.SaveAssets();
            Debug.Log("AudioBootstrap: prefab sounds wired.");
        }
    }
}
