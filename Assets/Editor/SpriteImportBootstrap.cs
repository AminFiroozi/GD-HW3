using UnityEditor;
using UnityEngine;

namespace ChickenShooter.EditorTools
{
    public static class SpriteImportBootstrap
    {
        [MenuItem("ChickenShooter/Bootstrap/Fix Sprite Import Settings")]
        public static void FixSpriteImports()
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
            int changed = 0;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    dirty = true;
                }
                if (importer.spriteImportMode != SpriteImportMode.Single)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    dirty = true;
                }
                if (importer.filterMode != FilterMode.Bilinear)
                {
                    importer.filterMode = FilterMode.Bilinear;
                    dirty = true;
                }
                if (importer.alphaIsTransparency != true)
                {
                    importer.alphaIsTransparency = true;
                    dirty = true;
                }

                if (dirty)
                {
                    importer.SaveAndReimport();
                    changed++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"SpriteImportBootstrap: fixed import settings on {changed} textures.");
        }
    }
}
