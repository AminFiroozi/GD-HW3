using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ChickenShooter.EditorTools
{
    public static class UiDedupeBootstrap
    {
        static readonly string[] AllScenes =
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };

        static readonly string[] CanvasNames =
        {
            "MainMenuCanvas",
            "HudCanvas",
            "PauseCanvas",
            "GameOverCanvas",
            "VictoryCanvas",
            "BossHealthBarCanvas"
        };

        [MenuItem("ChickenShooter/Bootstrap/Dedupe UI")]
        public static void DedupeUi()
        {
            foreach (var scenePath in AllScenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                int removed = 0;

                foreach (var canvasName in CanvasNames)
                {
                    var canvasGo = GameObject.Find(canvasName);
                    if (canvasGo == null) continue;

                    removed += DedupeChildrenRecursive(canvasGo.transform);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"UiDedupeBootstrap: removed {removed} duplicate UI objects in {scenePath}");
            }
        }

        static int DedupeChildrenRecursive(Transform parent)
        {
            int removed = 0;
            var seen = new Dictionary<string, Transform>();
            var toDestroy = new List<GameObject>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (seen.ContainsKey(child.name))
                {
                    toDestroy.Add(child.gameObject);
                }
                else
                {
                    seen[child.name] = child;
                }
            }

            foreach (var go in toDestroy)
            {
                Object.DestroyImmediate(go);
                removed++;
            }

            foreach (var kv in seen)
            {
                removed += DedupeChildrenRecursive(kv.Value);
            }

            return removed;
        }
    }
}
