using UnityEngine;
using UnityEngine.InputSystem;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;
using ChickenShooter.Data;
using ChickenShooter.Input;

namespace ChickenShooter.Combat
{
    public class WeaponController : MonoBehaviour
    {
        public WeaponDataSO[] weaponLevels;
        public GameObject bulletPrefab;
        public Transform firePoint;

        PlayerControls controls;
        float cooldown;
        bool firing;

        void Awake()
        {
            controls = new PlayerControls();
        }

        void OnEnable()
        {
            controls.Player.Enable();
            controls.Player.Fire.performed += OnFireStarted;
            controls.Player.Fire.canceled += OnFireStopped;
        }

        void OnDisable()
        {
            controls.Player.Fire.performed -= OnFireStarted;
            controls.Player.Fire.canceled -= OnFireStopped;
            controls.Player.Disable();
        }

        void OnFireStarted(InputAction.CallbackContext ctx) => firing = true;
        void OnFireStopped(InputAction.CallbackContext ctx) => firing = false;

        void Update()
        {
            cooldown -= Time.deltaTime;

            if (firing && cooldown <= 0f)
            {
                Fire();
            }
        }

        void Fire()
        {
            var weapon = CurrentWeapon();
            if (weapon == null || bulletPrefab == null) return;

            cooldown = weapon.fireRate;
            var origin = firePoint != null ? firePoint.position : transform.position;

            AudioManager.Instance?.PlaySfx(weapon.fireSound);

            for (int i = 0; i < weapon.spawnOffsets.Length; i++)
            {
                var offset = weapon.spawnOffsets[i];
                var angle = i < weapon.spreadAngles.Length ? weapon.spreadAngles[i] : 0f;
                var rotation = Quaternion.Euler(0f, 0f, angle);

                var spawnPos = origin + (Vector3)offset;
                var instance = PoolManager.Instance.Spawn(bulletPrefab, spawnPos, rotation);

                var bullet = instance.GetComponent<Bullet>();
                bullet.speed = weapon.bulletSpeed;
                bullet.damage = weapon.damage;
                bullet.movesUp = true;
                bullet.insta = false;
            }
        }

        WeaponDataSO CurrentWeapon()
        {
            if (weaponLevels == null || weaponLevels.Length == 0) return null;
            int level = GameManager.Instance != null ? GameManager.Instance.WeaponPowerLevel : 1;
            int index = Mathf.Clamp(level - 1, 0, weaponLevels.Length - 1);
            return weaponLevels[index];
        }
    }
}
