using UnityEngine;

namespace MyGame
{
    public class EnemyAttack : MonoBehaviour
    {
        [Header("Attack Settings")]
        public int damage = 20;
        public float attackRange = 1.2f;
        public LayerMask playerLayer;

        private void Awake()
        {
            // لو attackPoint منفصل، حدده هنا
            // attackPoint = transform.Find("AttackPoint") ?? transform;
        }

        public void PerformAttack()
{
    // ← أضف السطر ده في البداية
    if (PlayerDamageable.Instance == null || !PlayerDamageable.Instance.IsAlive)
    {
        return;  // ما نهاجمش لو اللاعب مش موجود أو ميت
    }

    Debug.Log("[EnemyAttack] PerformAttack called on " + gameObject.name);

    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
    Debug.Log($"[EnemyAttack] Found {hits.Length} colliders in range");

    foreach (Collider2D hit in hits)
    {
        GameObject hitObj = hit.gameObject;
        string layerName = LayerMask.LayerToName(hitObj.layer);

        Debug.Log($"[EnemyAttack] Found collider on object: {hitObj.name}, Layer: {layerName} (index {hitObj.layer}), Tag: {hitObj.tag}");

        PlayerDamageable player = hit.GetComponent<PlayerDamageable>();
        if (player != null)
        {
            Debug.Log($"[EnemyAttack] FOUND PLAYER! Hitting for {damage} damage!");
            player.TakeDamage(damage);
        }
        else
        {
            Debug.Log("[EnemyAttack] Collider found but no PlayerDamageable component!");
        }
    }
}

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}