using UnityEngine;

namespace MyGame
{
    public class FallDetector : MonoBehaviour
    {
        public static FallDetector Instance { get; private set; }

        [Header("Spawn Settings")]
        [SerializeField] private Transform spawnPoint;

        [Header("Fall Detection Settings")]
        [SerializeField] private float fallLimitY = -10f;

        private bool hasTriggeredFallDeath = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
                Debug.LogWarning("[FallDetector] Spawn point not set — using own position.");
            }
        }

        private void Update()
        {
            if (hasTriggeredFallDeath) return;

            if (PlayerDamageable.Instance == null || !PlayerDamageable.Instance.IsAlive)
                return;

            if (PlayerDamageable.Instance.transform.position.y < fallLimitY)
            {
                Debug.Log("☠️ Player fell below limit — triggering instant death!");
                PlayerDamageable.Instance.TakeDamage(9999);
                hasTriggeredFallDeath = true;
            }
        }

        // إعادة تهيئة الـ flag بعد revive أو بداية ليفل جديد
        public void ResetFallDetector()
        {
            hasTriggeredFallDeath = false;
            Debug.Log("[FallDetector] Fall death trigger reset");
        }

        public void SetSpawnPoint(Transform newSpawn)
        {
            spawnPoint = newSpawn;
            Debug.Log($"[FallDetector] Spawn point updated to: {newSpawn.position}");
        }

        public Vector3 GetStartPosition()
        {
            return spawnPoint != null ? spawnPoint.position : new Vector3(0, 1, 0);
        }
    }
}