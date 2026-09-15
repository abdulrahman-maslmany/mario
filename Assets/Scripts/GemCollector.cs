using UnityEngine;

namespace MyGame
{
    public class GemCollector : MonoBehaviour
    {
        private int collectedGems = 0;
        [SerializeField] private TMPro.TextMeshProUGUI gemsCounterUI; // اربطه من Inspector

        private void Start()
        {
            UpdateGemsUI();
        }

        public void CollectGem(GameObject gem)
        {
            collectedGems++;
            Destroy(gem);
            
            UpdateGemsUI();
            Debug.Log($"جُمعت جوهرة! العدد الحالي: {collectedGems}");
            
            // ✅ الإشعار لـ LevelManager
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnGemCollected(collectedGems);
            }
        }

        public void AddGems(int amount)
        {
            collectedGems += amount;
            UpdateGemsUI();
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnGemCollected(collectedGems);
            }
        }

        public int GetGemsCollected()
        {
            return collectedGems;
        }

        private void UpdateGemsUI()
        {
            if (gemsCounterUI != null)
            {
                int required = LevelManager.Instance?.GetRequiredGemsForCurrentLevel() ?? 1;
                gemsCounterUI.text = $"{collectedGems}/{required}";
            }
        }
    }
}