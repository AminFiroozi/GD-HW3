using UnityEngine;

namespace ChickenShooter.VFX
{
    public class ScreenShake : MonoBehaviour
    {
        public static ScreenShake Instance { get; private set; }

        Vector3 originalPos;
        float trauma;
        public float decaySpeed = 2f;
        public float maxOffset = 0.4f;

        void Awake()
        {
            Instance = this;
            originalPos = transform.localPosition;
        }

        public void Shake(float amount)
        {
            trauma = Mathf.Clamp01(trauma + amount);
        }

        void Update()
        {
            if (trauma <= 0f)
            {
                transform.localPosition = originalPos;
                return;
            }

            float shakeAmount = trauma * trauma;
            float offsetX = (Random.value * 2f - 1f) * maxOffset * shakeAmount;
            float offsetY = (Random.value * 2f - 1f) * maxOffset * shakeAmount;
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            trauma = Mathf.Max(0f, trauma - decaySpeed * Time.deltaTime);
        }
    }
}
