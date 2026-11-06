using UnityEngine;

namespace ChickenShooter.VFX
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxLayer : MonoBehaviour
    {
        public float scrollSpeed = 1f;

        SpriteRenderer spriteRenderer;
        float textureHeight;
        Vector3 origin;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            textureHeight = spriteRenderer.bounds.size.y;
            origin = transform.position;
        }

        void Update()
        {
            float travelled = Mathf.Repeat(Time.time * scrollSpeed, textureHeight);
            transform.position = new Vector3(origin.x, origin.y - travelled, origin.z);
        }
    }
}
