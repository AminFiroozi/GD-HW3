using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using ChickenShooter.Core.Pooling;

namespace ChickenShooter.EditorTools
{
    public static class VfxUpgradeBootstrap
    {
        const string ExplosionPrefabPath = "Assets/Prefabs/ExplosionBurst.prefab";
        const string FlareTexPath = "Assets/Sprites/VFX/sparkFlare.png";
        const string RingTexPath = "Assets/Sprites/VFX/shockRing.png";
        const string EggSpritePath = "Assets/Sprites/Lasers/eggLaser.png";
        const string ExplosionMatPath = "Assets/Materials/ExplosionFlare.mat";
        const string RingMatPath = "Assets/Materials/ExplosionRing.mat";

        public static void Upgrade()
        {
            ForceReimportSprite(FlareTexPath);
            ForceReimportSprite(RingTexPath);
            ForceReimportSprite(EggSpritePath);
            ForceReimportSprite("Assets/Sprites/Backgrounds/parallaxFarLevel1.png");
            ForceReimportSprite("Assets/Sprites/Backgrounds/parallaxFarLevel2.png");
            ForceReimportSprite("Assets/Sprites/Backgrounds/parallaxFarLevel3.png");
            ForceReimportSprite("Assets/Sprites/Backgrounds/parallaxNear.png");

            RebuildEggPrefab();
            RebuildExplosionPrefab();
            RefreshSceneBackgrounds();

            AssetDatabase.SaveAssets();
            Debug.Log("VfxUpgradeBootstrap: done.");
        }

        static void ForceReimportSprite(string path)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        static Material MakeAdditiveParticleMaterial(string path, Texture2D tex)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.SetTexture("_BaseMap", tex);
            mat.SetTexture("_MainTex", tex);
            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);
            if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 1f);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.One);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        static void RebuildEggPrefab()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(EggSpritePath);
            if (sprite == null) return;

            const string path = "Assets/Prefabs/EnemyEgg.prefab";
            using var scope = new PrefabUtility.EditPrefabContentsScope(path);
            var root = scope.prefabContentsRoot;

            root.GetComponent<SpriteRenderer>().sprite = sprite;
            var col = root.GetComponent<BoxCollider2D>();
            if (col != null) col.size = sprite.bounds.size;
        }

        static void RebuildExplosionPrefab()
        {
            var flareTex = AssetDatabase.LoadAssetAtPath<Texture2D>(FlareTexPath);
            var ringTex = AssetDatabase.LoadAssetAtPath<Texture2D>(RingTexPath);
            var flareMat = MakeAdditiveParticleMaterial(ExplosionMatPath, flareTex);
            var ringMat = MakeAdditiveParticleMaterial(RingMatPath, ringTex);

            var go = new GameObject("ExplosionBurst", typeof(ParticleSystem), typeof(DespawnAfterSeconds));
            var ps = go.GetComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startLifetime = 0.45f;
            main.startSpeed = 5f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.85f, 0.4f, 1f), new Color(1f, 0.35f, 0.1f, 1f));
            main.stopAction = ParticleSystemStopAction.Disable;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 18) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.08f;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            var sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(1f, 0f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.material = flareMat;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            var ringGo = new GameObject("ShockRing", typeof(ParticleSystem));
            ringGo.transform.SetParent(go.transform, false);
            var ringPs = ringGo.GetComponent<ParticleSystem>();
            var ringMain = ringPs.main;
            ringMain.duration = 0.3f;
            ringMain.loop = false;
            ringMain.startLifetime = 0.3f;
            ringMain.startSpeed = 0f;
            ringMain.startSize = 0.2f;
            ringMain.startColor = new Color(1f, 0.9f, 0.6f, 0.8f);
            ringMain.stopAction = ParticleSystemStopAction.None;

            var ringEmission = ringPs.emission;
            ringEmission.rateOverTime = 0f;
            ringEmission.SetBursts(new[] { new ParticleSystem.Burst(0f, 1) });

            var ringShape = ringPs.shape;
            ringShape.enabled = false;

            var ringSize = ringPs.sizeOverLifetime;
            ringSize.enabled = true;
            var ringSizeCurve = new AnimationCurve();
            ringSizeCurve.AddKey(0f, 0.3f);
            ringSizeCurve.AddKey(1f, 3f);
            ringSize.size = new ParticleSystem.MinMaxCurve(1f, ringSizeCurve);

            var ringColor = ringPs.colorOverLifetime;
            ringColor.enabled = true;
            var ringGrad = new Gradient();
            ringGrad.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            ringColor.color = ringGrad;

            var ringRenderer = ringGo.GetComponent<ParticleSystemRenderer>();
            ringRenderer.material = ringMat;
            ringRenderer.renderMode = ParticleSystemRenderMode.Billboard;

            go.GetComponent<DespawnAfterSeconds>().seconds = 0.7f;

            PrefabUtility.SaveAsPrefabAsset(go, ExplosionPrefabPath);
            Object.DestroyImmediate(go);
            Debug.Log("VfxUpgradeBootstrap: explosion prefab rebuilt with flare material.");
        }

        static void RefreshSceneBackgrounds()
        {
            foreach (var scenePath in new[] { "Assets/Scenes/Level1.unity", "Assets/Scenes/Level2.unity", "Assets/Scenes/Level3.unity" })
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                string farSpritePath = scenePath.Contains("Level1")
                    ? "Assets/Sprites/Backgrounds/parallaxFarLevel1.png"
                    : scenePath.Contains("Level2")
                        ? "Assets/Sprites/Backgrounds/parallaxFarLevel2.png"
                        : "Assets/Sprites/Backgrounds/parallaxFarLevel3.png";

                var farSprite = AssetDatabase.LoadAssetAtPath<Sprite>(farSpritePath);
                var nearSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Backgrounds/parallaxNear.png");

                var farGo = GameObject.Find("ParallaxBackground");
                if (farGo != null)
                {
                    var farRenderer = farGo.GetComponent<SpriteRenderer>();
                    farRenderer.sprite = farSprite;
                    farRenderer.color = Color.white;
                }

                var nearGo = GameObject.Find("ParallaxForeground");
                if (nearGo != null)
                {
                    var nearRenderer = nearGo.GetComponent<SpriteRenderer>();
                    nearRenderer.sprite = nearSprite;
                    nearRenderer.color = new Color(1f, 1f, 1f, 0.8f);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }
    }
}
