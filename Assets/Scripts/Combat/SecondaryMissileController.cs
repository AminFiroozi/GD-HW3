using UnityEngine;
using UnityEngine.InputSystem;
using ChickenShooter.Core;
using ChickenShooter.Core.Pooling;
using ChickenShooter.Enemies;
using ChickenShooter.Input;

namespace ChickenShooter.Combat
{
    public class SecondaryMissileController : MonoBehaviour
    {
        public float bossDamageFraction = 0.18f;
        public AudioClip explosionSound;

        PlayerControls controls;

        void Awake()
        {
            controls = new PlayerControls();
        }

        void OnEnable()
        {
            controls.Player.Enable();
            controls.Player.Missile.performed += OnMissileFired;
        }

        void OnDisable()
        {
            controls.Player.Missile.performed -= OnMissileFired;
            controls.Player.Disable();
        }

        void OnMissileFired(InputAction.CallbackContext ctx)
        {
            if (GameManager.Instance == null || !GameManager.Instance.TryUseSecondaryMissile()) return;

            AudioManager.Instance?.PlaySfx(explosionSound);

            foreach (var enemy in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                enemy.TakeDamage(int.MaxValue);
            }

            foreach (var meteor in Object.FindObjectsByType<Meteor>(FindObjectsSortMode.None))
            {
                meteor.TakeDamage(int.MaxValue);
            }

            foreach (var egg in GameObject.FindGameObjectsWithTag("EnemyBullet"))
            {
                PoolManager.Instance.Despawn(egg);
            }

            var boss = Object.FindFirstObjectByType<Boss>();
            if (boss != null)
            {
                int damage = Mathf.CeilToInt(boss.maxHealth * bossDamageFraction);
                boss.TakeDamage(damage);
            }
        }
    }
}
