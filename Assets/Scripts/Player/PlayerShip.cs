using UnityEngine;
using ChickenShooter.Core;
using ChickenShooter.Input;

namespace ChickenShooter.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerShip : MonoBehaviour
    {
        public float speed = 7f;
        public float invincibilityDuration = 1.5f;
        public float invincibilityFlashInterval = 0.1f;

        PlayerControls controls;
        Vector2 moveInput;
        Camera cam;
        SpriteRenderer spriteRenderer;
        float invincibilityTimer;

        void Awake()
        {
            controls = new PlayerControls();
            cam = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnEnable()
        {
            controls.Player.Enable();
            controls.Player.Move.performed += OnMove;
            controls.Player.Move.canceled += OnMove;
        }

        void OnDisable()
        {
            controls.Player.Move.performed -= OnMove;
            controls.Player.Move.canceled -= OnMove;
            controls.Player.Disable();
        }

        void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        void Update()
        {
            Move();
            TickInvincibility();
        }

        void Move()
        {
            var delta = (Vector3)(moveInput.normalized * speed * Time.deltaTime);
            var newPos = transform.position + delta;

            if (cam != null)
            {
                var viewportHalfWidth = cam.orthographicSize * cam.aspect;
                var viewportHalfHeight = cam.orthographicSize;
                var extentsX = spriteRenderer.bounds.extents.x;
                var extentsY = spriteRenderer.bounds.extents.y;

                newPos.x = Mathf.Clamp(newPos.x, -viewportHalfWidth + extentsX, viewportHalfWidth - extentsX);
                newPos.y = Mathf.Clamp(newPos.y, -viewportHalfHeight + extentsY, viewportHalfHeight - extentsY);
            }

            transform.position = newPos;
        }

        public void TakeDamage()
        {
            if (invincibilityTimer > 0f) return;

            bool dead = GameManager.Instance != null && GameManager.Instance.LoseLife();
            invincibilityTimer = invincibilityDuration;

            if (dead)
            {
                gameObject.SetActive(false);
                GameManager.Instance?.TriggerGameOver();
            }
        }

        void TickInvincibility()
        {
            if (invincibilityTimer <= 0f)
            {
                spriteRenderer.enabled = true;
                return;
            }

            invincibilityTimer -= Time.deltaTime;
            var flashOn = Mathf.FloorToInt(invincibilityTimer / invincibilityFlashInterval) % 2 == 0;
            spriteRenderer.enabled = flashOn;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Hazard"))
            {
                TakeDamage();
            }
        }
    }
}
