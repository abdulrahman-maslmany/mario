using UnityEngine;
using MyGame;

public class Attack : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private Vector2 knockbackForce = new Vector2(5f, 2f);
    [SerializeField] private LayerMask targetLayer;

    private void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1f, 1f), 0f, targetLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Damageable target))
            {
                target.TakeDamage(damage);

                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (hit.transform.position - transform.position).normalized;
                    rb.AddForce(new Vector2(direction.x * knockbackForce.x, direction.y * knockbackForce.y), ForceMode2D.Impulse);
                }
            }
        }
    }
}
