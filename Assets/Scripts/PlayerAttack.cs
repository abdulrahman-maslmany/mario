using UnityEngine;

namespace MyGame
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Attack Settings")]
        public float attackRange = 1f;
        public int attackDamage = 20;
        public LayerMask enemyLayers;

        private PlayerController player;
        private Animator animator;
        private Transform attackPoint;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            animator = GetComponent<Animator>();
            attackPoint = transform.Find("AttackPoint");

            if (attackPoint == null)
            {
                Debug.LogWarning("[PlayerAttack] Missing AttackPoint transform. Please add an empty child named 'AttackPoint'");
            }
        }

        public void Attack()
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.TryGetComponent(out EnemyDamageable damageable))
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
