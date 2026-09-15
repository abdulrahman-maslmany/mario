using UnityEngine;

namespace MyGame
{
    public class Gem : MonoBehaviour
    {
        private bool collected = false;
        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collected) return;

            if (collision.CompareTag("Player"))
            {
                collected = true;
                Debug.Log("[Gem] Gem collected by player!");

                // ✅ أضف الجوهرة إلى العداد
                if (GemCounterUI.Instance != null)
                {
                    GemCounterUI.Instance.AddGems(1);
                    // ✅ Trigger level complete check in LevelManager
                    LevelManager.Instance.OnGemCollected(GemCounterUI.Instance.CurrentGems);  // Updated to use public getter
                }
                else
                {
                    Debug.LogWarning("[Gem] GemCounterUI.Instance is NULL!");
                }

                // تأثيرات أو صوت إن أردت
                // AudioSource.PlayClipAtPoint(collectSound, transform.position);

                gameObject.SetActive(false); // إخفاء الجوهرة بعد جمعها
            }
        }

        public void ResetGem()
        {
            collected = false;
            transform.position = startPos;
            gameObject.SetActive(true);
        }
    }
}