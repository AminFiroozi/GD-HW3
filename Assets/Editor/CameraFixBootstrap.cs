using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ChickenShooter.EditorTools
{
    public static class CameraFixBootstrap
    {
        static readonly string[] AllScenes =
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity"
        };

        [MenuItem("ChickenShooter/Bootstrap/Fix Camera Depth")]
        public static void FixCameraDepth()
        {
            foreach (var scenePath in AllScenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                var cameraGo = GameObject.Find("Main Camera");
                if (cameraGo != null)
                {
                    cameraGo.transform.position = new Vector3(0f, 0f, -10f);

                    if (cameraGo.GetComponent<UniversalAdditionalCameraData>() == null)
                    {
                        cameraGo.AddComponent<UniversalAdditionalCameraData>();
                    }
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"CameraFixBootstrap: fixed camera depth in {scenePath}");
            }
        }
    }
}
