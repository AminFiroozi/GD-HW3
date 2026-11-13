using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ChickenShooter.EditorTools
{
    public static class UiContrastFixBootstrap
    {
        static readonly string[] AllScenes =
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };

        static readonly Color ButtonLabelColor = new Color(0.1f, 0.12f, 0.2f);

        [MenuItem("ChickenShooter/Bootstrap/Fix Button Text Contrast")]
        public static void FixButtonTextContrast()
        {
            foreach (var scenePath in AllScenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                int fixedCount = 0;

                foreach (var button in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    var label = button.GetComponentInChildren<Text>();
                    if (label != null)
                    {
                        label.color = ButtonLabelColor;
                        EditorUtility.SetDirty(label);
                        fixedCount++;
                    }
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"UiContrastFixBootstrap: fixed {fixedCount} button labels in {scenePath}");
            }
        }

        [MenuItem("ChickenShooter/Bootstrap/Add Victory Main Menu Button")]
        public static void AddVictoryMainMenuButton()
        {
            foreach (var scenePath in new[] { "Assets/Scenes/Level1.unity", "Assets/Scenes/Level2.unity", "Assets/Scenes/Level3.unity" })
            {
                UIBootstrap.BuildVictoryScreen(scenePath);
            }
        }
    }
}
