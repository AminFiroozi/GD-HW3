using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ChickenShooter.EditorTools
{
    public static class CanvasFixBootstrap
    {
        static readonly string[] AllScenes =
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };

        [MenuItem("ChickenShooter/Bootstrap/Fix Canvas Reference Resolution")]
        public static void FixCanvasReferenceResolution()
        {
            foreach (var scenePath in AllScenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                int fixedCount = 0;

                foreach (var scaler in Object.FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None))
                {
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.matchWidthOrHeight = 0.5f;
                    EditorUtility.SetDirty(scaler);
                    fixedCount++;
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"CanvasFixBootstrap: fixed {fixedCount} canvas scalers in {scenePath}");
            }
        }
    }
}
