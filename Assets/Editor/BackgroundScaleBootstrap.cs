using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ChickenShooter.EditorTools
{
    public static class BackgroundScaleBootstrap
    {
        const string BackgroundDir = "Assets/Sprites/Backgrounds/";
        const string EggSpritePath = "Assets/Sprites/Lasers/eggLaser.png";

        public static void Run()
        {
            foreach (var name in new[] { "parallaxFarLevel1.png", "parallaxFarLevel2.png", "parallaxFarLevel3.png", "parallaxNear.png" })
            {
                SetupTiledBackground(BackgroundDir + name);
            }

            var eggImporter = RequireImporter(EggSpritePath);
            eggImporter.spritePixelsPerUnit = 220f;
            eggImporter.SaveAndReimport();

            var eggSprite = AssetDatabase.LoadAssetAtPath<Sprite>(EggSpritePath);
            if (eggSprite == null) throw new InvalidOperationException("BackgroundScaleBootstrap: egg sprite missing");
            using (var scope = new PrefabUtility.EditPrefabContentsScope("Assets/Prefabs/EnemyEgg.prefab"))
            {
                var root = scope.prefabContentsRoot;
                root.GetComponent<SpriteRenderer>().sprite = eggSprite;
                root.GetComponent<BoxCollider2D>().size = eggSprite.bounds.size;
            }

            for (int level = 1; level <= 3; level++)
            {
                var scene = EditorSceneManager.OpenScene("Assets/Scenes/Level" + level + ".unity", OpenSceneMode.Single);
                ApplyTiled("ParallaxBackground", BackgroundDir + "parallaxFarLevel" + level + ".png");
                ApplyTiled("ParallaxForeground", BackgroundDir + "parallaxNear.png");
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("BackgroundScaleBootstrap: done.");
        }

        static TextureImporter RequireImporter(string path)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new InvalidOperationException("BackgroundScaleBootstrap: no importer for " + path);
            return importer;
        }

        static void SetupTiledBackground(string path)
        {
            var importer = RequireImporter(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 4096;
            importer.wrapMode = TextureWrapMode.Repeat;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }

        static void ApplyTiled(string objectName, string spritePath)
        {
            var go = GameObject.Find(objectName);
            if (go == null) throw new InvalidOperationException("BackgroundScaleBootstrap: missing " + objectName);
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null) throw new InvalidOperationException("BackgroundScaleBootstrap: missing sprite " + spritePath);

            var renderer = go.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.tileMode = SpriteTileMode.Continuous;

            go.transform.localScale = Vector3.one;
            var position = go.transform.position;
            go.transform.position = new Vector3(0f, 0f, position.z);
            renderer.size = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y * 2f);

            if (go.transform.localScale != Vector3.one) throw new InvalidOperationException("BackgroundScaleBootstrap: scale did not stick on " + objectName);
        }
    }
}
