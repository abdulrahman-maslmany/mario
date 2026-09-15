using UnityEngine;
using MyGame; // لـ PlayerDamageable

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDamageable playerDamageable = other.GetComponent<PlayerDamageable>();
            if (playerDamageable != null)
            {
                playerDamageable.Heal(healAmount); // 🔥 إصلاح: استخدم Heal
                Debug.Log("[HealthPickup] Player healed by " + healAmount);
            }
            Destroy(gameObject);
        }
    }
}