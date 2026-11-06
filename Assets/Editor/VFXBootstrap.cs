using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ChickenShooter.VFX;
using ChickenShooter.Enemies;
using ChickenShooter.Core.Pooling;

namespace ChickenShooter.EditorTools
{
    public static class VFXBootstrap
    {
        const string ExplosionPrefabPath = "Assets/Prefabs/ExplosionBurst.prefab";

        [MenuItem("ChickenShooter/Bootstrap/VFX/Run All")]
        public static void RunAll()
        {
            BuildExplosionPrefab();
            WireExplosionsIntoPrefabs();

            foreach (var levelScene in new[] { "Assets/Scenes/Level1.unity", "Assets/Scenes/Level2.unity", "Assets/Scenes/Level3.unity" })
            {
                BuildSceneVfx(levelScene);
            }
        }

        [MenuItem("ChickenShooter/Bootstrap/VFX/Build Explosion Prefab")]
        public static void BuildExplosionPrefab()
        {
            var go = new GameObject("ExplosionBurst", typeof(ParticleSystem), typeof(DespawnAfterSeconds));
            var ps = go.GetComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startLifetime = 0.4f;
            main.startSpeed = 4f;
            main.startSize = 0.3f;
            main.startColor = new Color(1f, 0.6f, 0.1f, 1f);
            main.stopAction = ParticleSystemStopAction.Disable;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 20) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.1f;

            go.GetComponent<DespawnAfterSeconds>().seconds = 0.6f;

            PrefabUtility.SaveAsPrefabAsset(go, ExplosionPrefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            Debug.Log("VFXBootstrap: explosion prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/VFX/Wire Explosions Into Prefabs")]
        public static void WireExplosionsIntoPrefabs()
        {
            var explosionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ExplosionPrefabPath);

            foreach (var path in new[]
            {
                "Assets/Prefabs/EnemyBasic.prefab",
                "Assets/Prefabs/EnemyArmored.prefab",
                "Assets/Prefabs/EnemyAlien.prefab"
            })
            {
                using var scope = new PrefabUtility.EditPrefabContentsScope(path);
                scope.prefabContentsRoot.GetComponent<Enemy>().explosionPrefab = explosionPrefab;
                scope.prefabContentsRoot.AddComponent<DamageFlash>();
            }

            foreach (var path in new[]
            {
                "Assets/Prefabs/BossLevel1.prefab",
                "Assets/Prefabs/BossLevel2.prefab",
                "Assets/Prefabs/BossLevel3.prefab"
            })
            {
                using var scope = new PrefabUtility.EditPrefabContentsScope(path);
                scope.prefabContentsRoot.GetComponent<Boss>().explosionPrefab = explosionPrefab;
                scope.prefabContentsRoot.AddComponent<DamageFlash>();
            }

            using (var scope = new PrefabUtility.EditPrefabContentsScope("Assets/Prefabs/MeteorBig.prefab"))
            {
                scope.prefabContentsRoot.GetComponent<Meteor>().explosionPrefab = explosionPrefab;
                scope.prefabContentsRoot.AddComponent<DamageFlash>();
            }

            AssetDatabase.SaveAssets();
            Debug.Log("VFXBootstrap: explosions wired into enemy/boss/meteor prefabs.");
        }

        static void BuildSceneVfx(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            var cameraGo = GameObject.Find("Main Camera");
            if (cameraGo != null && cameraGo.GetComponent<ScreenShake>() == null)
            {
                cameraGo.AddComponent<ScreenShake>();

                var camData = cameraGo.GetComponent<UniversalAdditionalCameraData>();
                if (camData == null) camData = cameraGo.AddComponent<UniversalAdditionalCameraData>();
                camData.renderPostProcessing = true;
            }

            BuildParallaxLayer(scenePath);
            BuildBloomVolume();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        static void BuildParallaxLayer(string scenePath)
        {
            if (GameObject.Find("ParallaxBackground") != null) return;

            string farSpritePath = scenePath.Contains("Level1")
                ? "Assets/Sprites/Backgrounds/parallaxFarLevel1.png"
                : scenePath.Contains("Level2")
                    ? "Assets/Sprites/Backgrounds/parallaxFarLevel2.png"
                    : "Assets/Sprites/Backgrounds/parallaxFarLevel3.png";

            var farSprite = AssetDatabase.LoadAssetAtPath<Sprite>(farSpritePath);
            var nearSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Backgrounds/parallaxNear.png");

            var farGo = new GameObject("ParallaxBackground", typeof(SpriteRenderer), typeof(ParallaxLayer));
            farGo.transform.position = new Vector3(0f, 0f, 10f);
            farGo.transform.localScale = new Vector3(20f, 30f, 1f);
            var farRenderer = farGo.GetComponent<SpriteRenderer>();
            farRenderer.sprite = farSprite;
            farRenderer.sortingOrder = -100;
            farGo.GetComponent<ParallaxLayer>().scrollSpeed = 0.5f;

            var nearGo = new GameObject("ParallaxForeground", typeof(SpriteRenderer), typeof(ParallaxLayer));
            nearGo.transform.position = new Vector3(0f, 0f, 9f);
            nearGo.transform.localScale = new Vector3(20f, 30f, 1f);
            var nearRenderer = nearGo.GetComponent<SpriteRenderer>();
            nearRenderer.sprite = nearSprite;
            nearRenderer.color = new Color(1f, 1f, 1f, 0.15f);
            nearRenderer.sortingOrder = -99;
            nearGo.GetComponent<ParallaxLayer>().scrollSpeed = 1.2f;
        }

        static void BuildBloomVolume()
        {
            if (GameObject.Find("GlobalVolume") != null) return;

            const string profilePath = "Assets/Settings/BloomVolumeProfile.asset";
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, profilePath);
            }

            if (!profile.TryGet(out Bloom bloom))
            {
                bloom = profile.Add<Bloom>(true);
            }
            bloom.intensity.overrideState = true;
            bloom.intensity.value = 1.2f;
            bloom.threshold.overrideState = true;
            bloom.threshold.value = 1.0f;

            var volumeGo = new GameObject("GlobalVolume", typeof(Volume));
            var volume = volumeGo.GetComponent<Volume>();
            volume.isGlobal = true;
            volume.sharedProfile = profile;

            AssetDatabase.SaveAssets();
        }
    }
}
