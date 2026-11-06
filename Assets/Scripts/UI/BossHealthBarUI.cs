using UnityEngine;
using UnityEngine.UI;
using ChickenShooter.Enemies;

namespace ChickenShooter.UI
{
    public class BossHealthBarUI : MonoBehaviour
    {
        public Image fillImage;
        public GameObject root;

        public void Bind(Boss boss)
        {
            if (root != null) root.SetActive(true);
            boss.OnHealthChanged += HandleHealthChanged;
            boss.OnBossDied += HandleBossDied;
        }

        void HandleHealthChanged(int current, int max)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = max > 0 ? (float)current / max : 0f;
            }
        }

        void HandleBossDied()
        {
            if (root != null) root.SetActive(false);
        }
    }
}
