using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ChickenShooter.EditorTools
{
    public static class BuildBootstrap
    {
        [MenuItem("ChickenShooter/Bootstrap/Build Windows Standalone")]
        public static void BuildWindows()
        {
            var scenes = new System.Collections.Generic.List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled) scenes.Add(scene.path);
            }

            var options = new BuildPlayerOptions
            {
                scenes = scenes.ToArray(),
                locationPathName = "../GD-HW3-Build/ChickenShooter.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"BuildBootstrap: build succeeded, {report.summary.totalSize} bytes.");
            }
            else
            {
                Debug.LogError($"BuildBootstrap: build result {report.summary.result}");
            }
        }
    }
}
