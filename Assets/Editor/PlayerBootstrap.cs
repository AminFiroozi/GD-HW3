using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ChickenShooter.Player;

namespace ChickenShooter.EditorTools
{
    public static class PlayerBootstrap
    {
        const string PrefabPath = "Assets/Prefabs/PlayerShip.prefab";
        const string SpritePath = "Assets/Sprites/Player/playerShip1_blue.png";

        [MenuItem("ChickenShooter/Bootstrap/Run All Player Setup")]
        public static void RunAll()
        {
            AddTags();
            BuildPlayerPrefab();
            PlacePlayerInLevel1();
        }

        [MenuItem("ChickenShooter/Bootstrap/Add Project Tags")]
        public static void AddTags()
        {
            AddTag("Hazard");
            AddTag("PlayerBullet");
            AddTag("EnemyBullet");
            AddTag("Pickup");
            Debug.Log("PlayerBootstrap: tags ensured.");
        }

        static void AddTag(string tag)
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset").FirstOrDefault();
            if (asset == null) return;

            var so = new SerializedObject(asset);
            var tagsProp = so.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag) return;
            }

            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
            so.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Player Prefab")]
        public static void BuildPlayerPrefab()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
            if (sprite == null)
            {
                Debug.LogError($"PlayerBootstrap: sprite not found at {SpritePath}");
                return;
            }

            var go = new GameObject("PlayerShip", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(PlayerShip));
            go.GetComponent<SpriteRenderer>().sprite = sprite;

            var rb = go.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = sprite.bounds.size;

            go.transform.localScale = Vector3.one * 0.5f;

            PrefabUtility.SaveAsPrefabAsset(go, PrefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("PlayerBootstrap: player prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Place Player In Level1")]
        public static void PlacePlayerInLevel1()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Debug.LogError("PlayerBootstrap: prefab not found, run Build Player Prefab first.");
                return;
            }

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Level1.unity", OpenSceneMode.Single);

            if (GameObject.Find("PlayerShip") == null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
                instance.transform.position = new Vector3(0f, -4f, 0f);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("PlayerBootstrap: player placed in Level1.");
        }
    }
}
