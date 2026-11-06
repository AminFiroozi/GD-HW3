using UnityEditor;
using UnityEngine;
using ChickenShooter.Combat;
using ChickenShooter.Core.Pooling;
using ChickenShooter.Data;

namespace ChickenShooter.EditorTools
{
    public static class WeaponBootstrap
    {
        const string BulletPrefabPath = "Assets/Prefabs/PlayerBullet.prefab";
        const string GiftBoxPrefabPath = "Assets/Prefabs/WeaponGiftBox.prefab";
        const string PlayerPrefabPath = "Assets/Prefabs/PlayerShip.prefab";

        [MenuItem("ChickenShooter/Bootstrap/Run All Weapon Setup")]
        public static void RunAll()
        {
            BuildBulletPrefab();
            BuildGiftBoxPrefab();
            WireWeaponOnPlayer();
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Bullet Prefab")]
        public static void BuildBulletPrefab()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Lasers/playerLaser.png");
            var go = new GameObject("PlayerBullet", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Bullet), typeof(DespawnAfterSeconds));
            go.tag = "PlayerBullet";
            go.GetComponent<SpriteRenderer>().sprite = sprite;

            var rb = go.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (sprite != null) col.size = sprite.bounds.size;

            go.GetComponent<DespawnAfterSeconds>().seconds = 2.5f;

            PrefabUtility.SaveAsPrefabAsset(go, BulletPrefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            Debug.Log("WeaponBootstrap: bullet prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Build Gift Box Prefab")]
        public static void BuildGiftBoxPrefab()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Powerups/weaponGiftBox.png");
            var go = new GameObject("WeaponGiftBox", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(WeaponGiftBox), typeof(DespawnAfterSeconds));
            go.tag = "Pickup";
            go.GetComponent<SpriteRenderer>().sprite = sprite;

            var rb = go.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (sprite != null) col.size = sprite.bounds.size;

            go.GetComponent<DespawnAfterSeconds>().seconds = 6f;

            PrefabUtility.SaveAsPrefabAsset(go, GiftBoxPrefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            Debug.Log("WeaponBootstrap: gift box prefab built.");
        }

        [MenuItem("ChickenShooter/Bootstrap/Wire Weapon On Player")]
        public static void WireWeaponOnPlayer()
        {
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            var bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BulletPrefabPath);

            if (playerPrefab == null || bulletPrefab == null)
            {
                Debug.LogError("WeaponBootstrap: missing player or bullet prefab.");
                return;
            }

            var weapon1 = AssetDatabase.LoadAssetAtPath<WeaponDataSO>("Assets/ScriptableObjects/Weapons/Weapon_Level1.asset");
            var weapon2 = AssetDatabase.LoadAssetAtPath<WeaponDataSO>("Assets/ScriptableObjects/Weapons/Weapon_Level2.asset");
            var weapon3 = AssetDatabase.LoadAssetAtPath<WeaponDataSO>("Assets/ScriptableObjects/Weapons/Weapon_Level3.asset");

            weapon1.bulletPrefab = bulletPrefab;
            weapon2.bulletPrefab = bulletPrefab;
            weapon3.bulletPrefab = bulletPrefab;
            EditorUtility.SetDirty(weapon1);
            EditorUtility.SetDirty(weapon2);
            EditorUtility.SetDirty(weapon3);

            using (var editScope = new PrefabUtility.EditPrefabContentsScope(PlayerPrefabPath))
            {
                var root = editScope.prefabContentsRoot;
                var weaponController = root.GetComponent<WeaponController>();
                if (weaponController == null) weaponController = root.AddComponent<WeaponController>();

                weaponController.weaponLevels = new[] { weapon1, weapon2, weapon3 };
                weaponController.bulletPrefab = bulletPrefab;
                weaponController.firePoint = root.transform;
            }

            AssetDatabase.SaveAssets();
            Debug.Log("WeaponBootstrap: weapon wired onto player prefab.");
        }
    }
}
