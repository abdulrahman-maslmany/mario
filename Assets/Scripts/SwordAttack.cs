using UnityEngine;
using MyGame;  // ضروري لكي يعرف السكربت Damageable

public class SwordAttack : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private Vector2 knockbackForce = new Vector2(5f, 2f);
    [SerializeField] private LayerMask targetLayer;

    public void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1f,1f), 0f, targetLayer);
        foreach (var hit in hits)
        {
            Damageable target = hit.GetComponent<Damageable>();
            if (target != null)
            {
                // استدعاء TakeDamage بدلاً من Hit
                target.TakeDamage(damage);

                // إضافة Knockback إذا كان لدى الهدف Rigidbody2D
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (hit.transform.position - transform.position).normalized;
                    rb.AddForce(new Vector2(direction.x * knockbackForce.x, knockbackForce.y), ForceMode2D.Impulse);
                }

                Debug.Log($"Dealt {damage} damage to {hit.name}");
            }
        }
    }
}
