using UnityEngine;

namespace ChickenShooter.Data
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ChickenShooter/Weapon Data")]
    public class WeaponDataSO : ScriptableObject
    {
        public string weaponName;
        public int powerLevel = 1;
        public GameObject bulletPrefab;
        public int damage = 1;
        public float bulletSpeed = 12f;
        public float fireRate = 0.2f;
        public Vector2[] spawnOffsets = { Vector2.zero };
        public float[] spreadAngles = { 0f };
    }
}
