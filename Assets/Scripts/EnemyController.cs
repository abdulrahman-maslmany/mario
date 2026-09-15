using UnityEngine;

namespace MyGame
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 2f;
        public float detectionRange = 5f;
        public float attackRange = 1f;

        [Header("Attack")]
        public float attackCooldown = 1f;
        private float lastAttackTime = 0f;

        private Transform player;
        private Animator animator;
        private Rigidbody2D rb;
        private EnemyAttack enemyAttack;
        private bool hasTarget = false;

        private Vector3 startPosition;
        private Damageable damageable;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            enemyAttack = GetComponent<EnemyAttack>();
            damageable = GetComponent<Damageable>();
            startPosition = transform.position;
        }

        private void Start()
        {
            FindPlayer();
        }

        private void OnEnable()
        {
            ResetEnemy();
        }

        private void FindPlayer()
        {
            // 🔥 إصلاح: استخدم FindFirstObjectByType
            player = UnityEngine.Object.FindFirstObjectByType<PlayerController>().transform;
            Debug.Log("[EnemyController] Player found via Singleton.");
        }

        private void FixedUpdate()
        {
            if (player == null) 
            { 
                FindPlayer(); 
                return; 
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            hasTarget = distanceToPlayer <= detectionRange;
            bool isInAttackRange = distanceToPlayer <= attackRange;

            animator.SetBool("hasTarget", hasTarget);
            animator.SetBool("isInAttackRange", isInAttackRange);

            if (hasTarget)
            {
                if (isInAttackRange)
                {
                    StopMoving();
                    if (Time.time - lastAttackTime >= attackCooldown)
                    {
                        animator.SetTrigger("attackTrigger");
                        lastAttackTime = Time.time;
                    }
                }
                else
                {
                    FollowPlayer();
                }
            }
            else
            {
                StopMoving();
            }
        }

        private void FollowPlayer()
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            animator.SetBool("canMove", true);
            animator.SetFloat("speed", direction.magnitude);

            if (direction.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }

        private void StopMoving()
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("canMove", false);
            animator.SetFloat("speed", 0f);
        }

        public void ResetEnemy()
        {
            damageable?.ResetHealth();
            transform.position = startPosition;
            rb.linearVelocity = Vector2.zero;

            animator.Rebind();
            animator.Update(0f);

            lastAttackTime = 0f;
            hasTarget = false;
            animator.SetBool("hasTarget", false);
            animator.SetBool("isInAttackRange", false);
            animator.SetBool("canMove", false);
            animator.SetFloat("speed", 0f);

            gameObject.SetActive(true);
        }
    }
}