using UnityEngine;
using MyGame;               // للوصول إلى Damageable و EnemyAttack
using MyGame.Animation;    // ✅ مهم جداً حتى يتعرف على AnimationStrings
using System.Collections;  // 🔥 إضافة لـ Coroutine

namespace MyGame
{
    [RequireComponent(typeof(Rigidbody2D), typeof(EnemyAttack))]
    public class FlyingEyeController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 3f;
        public float detectionRange = 5f;
        public float attackRange = 1f;  // تأكد من مطابقتها في Inspector إذا غيرتها

        [Header("Attack")]
        public float attackCooldown = 0.5f;
        private float lastAttackTime = 0f;

        private Transform player;
        private Animator animator;
        private Rigidbody2D rb;
        private EnemyAttack enemyAttack;
        private bool hasTarget = false;

        private Vector3 startPosition; // لحفظ مكان البداية
        private Damageable damageable;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            enemyAttack = GetComponent<EnemyAttack>();
            damageable = GetComponent<Damageable>();
            startPosition = transform.position; // نخزن مكان البداية
        }

        private void Start()
        {
            // 🔥 ابدأ البحث الآمن عن اللاعب عبر Coroutine
            StartCoroutine(FindPlayerSafely());
        }

        private void OnEnable()
        {
            ResetEnemy();
        }

        // 🔥 دالة آمنة للبحث عن اللاعب (تُستدعى في Coroutine)
        private IEnumerator FindPlayerSafely()
        {
            // انتظر إطارًا واحدًا لضمان تنفيذ Awake للـ Singleton
            yield return null;

            // جرب الـ Singleton أولاً
            if (player == null && PlayerSingleton.Instance != null)
            {
                player = PlayerSingleton.Instance.PlayerTransform;
                Debug.Log("[FlyingEye] Player found via Singleton.");
            }
            else
            {
                // إذا لم يُعثر عليه، ابحث يدويًا في الـ Scene
                PlayerSingleton singleton = FindFirstObjectByType<PlayerSingleton>();  // 🔥 استخدم FindFirstObjectByType للحداثة (بدلاً من FindObjectOfType)
                if (singleton != null)
                {
                    player = singleton.PlayerTransform;
                    Debug.Log("[FlyingEye] Player found via FindFirstObjectByType.");
                }
                else
                {
                    Debug.LogWarning("[FlyingEye] PlayerSingleton not found yet. Retrying in 0.5s...");
                    // أعد المحاولة بعد نصف ثانية إذا لم يُعثر عليه
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(FindPlayerSafely());  // تكرار آمن
                    yield break;  // توقف هنا ولا تستمر
                }
            }

            // إذا وُجِد اللاعب، ابدأ التحقق الدوري كل ثانية (بديل لـ InvokeRepeating)
            if (player != null)
            {
                InvokeRepeating(nameof(CheckPlayerExists), 1f, 1f);
            }
        }

        // 🔥 دالة للتحقق الدوري من وجود اللاعب (للتعامل مع Respawn أو Destroy)
        private void CheckPlayerExists()
        {
            if (player == null || player.gameObject == null)
            {
                Debug.Log("[FlyingEye] Player lost. Re-finding...");
                StopCoroutine(FindPlayerSafely());  // أوقف إذا كانت تعمل
                StartCoroutine(FindPlayerSafely());
            }
        }

        private void FixedUpdate()
        {
            if (player == null) 
            { 
                // 🔥 لا تستدعِ FindPlayer هنا؛ دع الـ Coroutine تتعامل معه
                return; 
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
hasTarget = distanceToPlayer <= detectionRange;
bool isInAttackRange = distanceToPlayer <= attackRange;


            animator.SetBool(AnimationStrings.hasTarget, hasTarget);
            animator.SetBool(AnimationStrings.isInAttackRange, isInAttackRange);

            if (hasTarget)
            {
                if (isInAttackRange)
                {
                    StopMoving();
                    if (Time.time - lastAttackTime >= attackCooldown)
                    {
                        animator.SetTrigger(AnimationStrings.attackTrigger);
                        lastAttackTime = Time.time;
                        // لا تستدعي Attack() هنا - دع الحدث يتعامل معه
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

        // 🔥 إزالة الدالة القديمة FindPlayer() – لم تعد مستخدمة

        private void Attack(float distanceToPlayer)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
            // لا تستدعي Attack() هنا - دع الـ Event يتعامل مع الضرر للتزامن مع الرسوم
        }

        // يتم استدعاؤه من Animation Event في 'attack' clip
        public void DealDamage()
        {
            Debug.Log("[FlyingEye] DealDamage called from animation event!");  // للتصحيح: يجب أن يظهر في Console
            enemyAttack.PerformAttack();

        }

        private void FollowPlayer()
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;  // دعم كامل للحركة في X و Y (طيران)
            animator.SetBool(AnimationStrings.canMove, true);
            animator.SetFloat(AnimationStrings.speed, direction.magnitude);  // استخدم magnitude للسرعة الكلية

            if (direction.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }

        private void StopMoving()
        {
            rb.linearVelocity = Vector2.zero;  // وقف كامل للطائر
            animator.SetBool(AnimationStrings.canMove, false);
            animator.SetFloat(AnimationStrings.speed, 0f);
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
            animator.SetBool(AnimationStrings.hasTarget, false);
            animator.SetBool(AnimationStrings.isInAttackRange, false);
            animator.SetBool(AnimationStrings.canMove, false);
            animator.SetFloat(AnimationStrings.speed, 0f);

            gameObject.SetActive(true);

            // 🔥 أعد البحث عن اللاعب بعد الـ Reset
            player = null;
            StartCoroutine(FindPlayerSafely());
        }
    }
}