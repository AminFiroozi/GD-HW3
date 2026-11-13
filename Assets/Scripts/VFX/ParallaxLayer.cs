using UnityEngine;

namespace ChickenShooter.VFX
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxLayer : MonoBehaviour
    {
        public float scrollSpeed = 1f;

        float tileHeight;
        Vector3 origin;

        void Awake()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            tileHeight = spriteRenderer.sprite.bounds.size.y * transform.lossyScale.y;
            origin = transform.position;
        }

        void Update()
        {
            float travelled = Mathf.Repeat(Time.time * scrollSpeed, tileHeight);
            transform.position = new Vector3(origin.x, origin.y + tileHeight * 0.5f - travelled, origin.z);
        }
    }
}
