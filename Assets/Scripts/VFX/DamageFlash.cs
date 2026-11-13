using UnityEngine;

namespace ChickenShooter.VFX
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DamageFlash : MonoBehaviour
    {
        public float flashDuration = 0.1f;

        SpriteRenderer spriteRenderer;
        Color originalColor;
        float timer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        public void Flash()
        {
            timer = flashDuration;
            spriteRenderer.color = Color.white;
        }

        void Update()
        {
            if (timer <= 0f) return;

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }
}
