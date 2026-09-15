
using UnityEngine;
using MyGame;               // للوصول إلى Damageable و EnemyAttack
using MyGame.Animation;    // ✅ مهم جداً حتى يتعرف على AnimationStrings
using System.Collections;  // 🔥 إضافة لـ Coroutine

namespace MyGame
{
    [RequireComponent(typeof(Rigidbody2D), typeof(EnemyAttack))]
    public class GroundEnemyController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 2f;
        public float detectionRange = 5f;
        public float attackRange = 1f;  // تأكد من مطابقتها في Inspector إذا غيرتها

        [Header("Attack")]
        public float attackCooldown = 1f;
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
            // 🔥 ابدأ البحث الآمن عن اللاعب عبر Coroutine لتجنب مشاكل التحميل
            StartCoroutine(FindPlayerSafely());
        }

        private void OnEnable()
        {
            ResetEnemy();
        }

        // 🔥 دالة آمنة للبحث عن اللاعب (تُستدعى في Coroutine)
        private IEnumerator FindPlayerSafely()
        {
            // انتظر إطارًا واحدًا لضمان تنفيذ Awake للـ Singleton أو PlayerController
            yield return null;

            // جرب الـ Singleton أولاً (إذا كنت تستخدمه)
            if (player == null && PlayerSingleton.Instance != null)
            {
                player = PlayerSingleton.Instance.PlayerTransform;
                Debug.Log("[GroundEnemy] Player found via Singleton.");
            }
            else
            {
                // إذا لم يُعثر عليه، ابحث يدويًا في الـ Scene باستخداف PlayerController (أو غيره إذا غيرت الاسم)
                PlayerController playerController = FindFirstObjectByType<PlayerController>();  // 🔥 استخدم FindFirstObjectByType للحداثة
                if (playerController != null)
                {
                    player = playerController.transform;
                    Debug.Log("[GroundEnemy] Player found via FindFirstObjectByType.");
                }
                else
                {
                    Debug.LogWarning("[GroundEnemy] Player not found yet. Retrying in 0.5s...");
                    // أعد المحاولة بعد نصف ثانية إذا لم يُعثر عليه
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(FindPlayerSafely());  // تكرار آمن
                    yield break;  // توقف هنا ولا تستمر
                }
            }

            // إذا وُجِد اللاعب، ابدأ التحقق الدوري كل ثانية (بديل لـ InvokeRepeating للتعامل مع Respawn)
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
                Debug.Log("[GroundEnemy] Player lost. Re-finding...");
                CancelInvoke(nameof(CheckPlayerExists));  // أوقف التحقق الدوري
                StartCoroutine(FindPlayerSafely());
            }
        }

        private void FixedUpdate()
        {
            if (player == null) 
            { 
                // 🔥 لا تستدعِ FindPlayer هنا؛ دع الـ Coroutine تتعامل معه لتجنب الأخطاء
                return; 
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            hasTarget = distanceToPlayer <= detectionRange;
            bool isInAttackRange = distanceToPlayer <= attackRange;

            // استخدم AnimationStrings للتوافق مع باقي الأعداء (غيرها إذا كانت أسماء الباراميترز مختلفة)
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
                        // لا تستدعي الضرر هنا - دع الحدث في الـ Animation يتعامل معه للتزامن
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
            // فقط الاتجاه الأفقي (X) للأعداء الأرضية - يحافظ على الـ Y الحالي (مثل القفز أو المنصات إذا أردت)
            Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;

            // حرك العدو في المحور X فقط، مع الحفاظ على الـ velocity Y إذا كان هناك فيزياء (مثل الجاذبية)
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

            animator.SetBool(AnimationStrings.canMove, true);
            animator.SetFloat(AnimationStrings.speed, Mathf.Abs(direction.x));  // سرعة أفقية فقط

            // اتجاه الوجه بناءً على حركة X
            if (direction.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }

        private void StopMoving()
        {
            // وقف الحركة الأفقية فقط، مع الحفاظ على Y إذا لزم الأمر (لكن في معظم الحالات وقف كامل)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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

            // 🔥 أعد البحث عن اللاعب بعد الـ Reset للتعامل مع إعادة التحميل
            player = null;
            CancelInvoke(nameof(CheckPlayerExists));  // أوقف أي تحقق دوري قديم
            StartCoroutine(FindPlayerSafely());
        }
    }
}