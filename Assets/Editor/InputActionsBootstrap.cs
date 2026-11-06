using UnityEditor;
using UnityEngine;

namespace ChickenShooter.EditorTools
{
    public static class InputActionsBootstrap
    {
        const string AssetPath = "Assets/InputActions/PlayerControls.inputactions";

        [MenuItem("ChickenShooter/Bootstrap/Generate Input Wrapper")]
        public static void GenerateWrapper()
        {
            var importer = AssetImporter.GetAtPath(AssetPath);
            if (importer == null)
            {
                Debug.LogError($"InputActionsBootstrap: no importer found at {AssetPath}");
                return;
            }

            var so = new SerializedObject(importer);
            so.FindProperty("m_GenerateWrapperCode").boolValue = true;
            so.FindProperty("m_WrapperCodePath").stringValue = "Assets/InputActions/PlayerControls.cs";
            so.FindProperty("m_WrapperClassName").stringValue = "PlayerControls";
            so.FindProperty("m_WrapperCodeNamespace").stringValue = "ChickenShooter.Input";
            so.ApplyModifiedProperties();

            importer.SaveAndReimport();
            AssetDatabase.Refresh();

            Debug.Log("InputActionsBootstrap: wrapper generated.");
        }
    }
}
