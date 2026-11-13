using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ChickenShooter.EditorTools
{
    public static class SceneBootstrap
    {
        const string ScenesFolder = "Assets/Scenes";
        const string SettingsFolder = "Assets/Settings";

        static readonly string[] SceneNames = { "MainMenu", "Level1", "Level2", "Level3" };

        [MenuItem("ChickenShooter/Bootstrap/Build Base Scenes")]
        public static void BuildScenes()
        {
            SetupUrp();

            if (!AssetDatabase.IsValidFolder(ScenesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            var scenePaths = new System.Collections.Generic.List<string>();

            foreach (var name in SceneNames)
            {
                var path = $"{ScenesFolder}/{name}.unity";
                if (!File.Exists(path))
                {
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    var cameraGO = new GameObject("Main Camera", typeof(Camera));
                    cameraGO.tag = "MainCamera";
                    var cam = cameraGO.GetComponent<Camera>();
                    cam.orthographic = true;
                    cam.orthographicSize = 5f;
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = Color.black;

                    EditorSceneManager.SaveScene(scene, path);
                }

                scenePaths.Add(path);
            }

            var buildScenes = new EditorBuildSettingsScene[scenePaths.Count];
            for (int i = 0; i < scenePaths.Count; i++)
            {
                buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("SceneBootstrap: base scenes created and registered in Build Settings.");
        }

        static void SetupUrp()
        {
            if (!AssetDatabase.IsValidFolder(SettingsFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Settings");
            }

            var urpAssetPath = $"{SettingsFolder}/URP-2D-Asset.asset";
            var existing = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(urpAssetPath);
            if (existing != null)
            {
                GraphicsSettings.defaultRenderPipeline = existing;
                QualitySettings.renderPipeline = existing;
                return;
            }

            var rendererDataPath = $"{SettingsFolder}/Renderer2DData.asset";
            var rendererData = ScriptableObject.CreateInstance<Renderer2DData>();
            AssetDatabase.CreateAsset(rendererData, rendererDataPath);

            var urpAsset = UniversalRenderPipelineAsset.Create(rendererData);
            AssetDatabase.CreateAsset(urpAsset, urpAssetPath);

            GraphicsSettings.defaultRenderPipeline = urpAsset;
            QualitySettings.renderPipeline = urpAsset;

            AssetDatabase.SaveAssets();
        }
    }
}
