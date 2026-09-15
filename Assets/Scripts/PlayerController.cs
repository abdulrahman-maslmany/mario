using UnityEngine;
using MyGame.Animation;
using MyGame;
using GoogleMobileAds.Api;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Bow Attack")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float bowAttackCooldown = 0.5f;

    [Header("Fall Detection")]
    [SerializeField] private float fallThreshold = -10f;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInputSimulation input;
    private float lastBowAttackTime;
    private Vector3 respawnPoint;
    private bool isRespawning = false;
    
    // المتغير اللي هيوقف تكرار الضرر من السقوط
    private bool hasAppliedFallDamage = false;

    public bool IsFacingRight => transform.localScale.x > 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

#if UNITY_6000_0_OR_NEWER
        input = FindFirstObjectByType<PlayerInputSimulation>();
#else
        input = FindObjectOfType<PlayerInputSimulation>();
#endif

        respawnPoint = transform.position;

        if (PlayerDamageable.Instance != null)
        {
            PlayerDamageable.Instance.OnDeath += HandleDeath;
            PlayerDamageable.Instance.OnRevive += HandleRevive;
        }
    }

    private void OnDestroy()
    {
        if (PlayerDamageable.Instance != null)
        {
            PlayerDamageable.Instance.OnDeath -= HandleDeath;
            PlayerDamageable.Instance.OnRevive -= HandleRevive;
        }
    }

private void Update()
{
    // ────────────────────────────────────────────────────────
    // DEBUG – remove or comment out later
    // ────────────────────────────────────────────────────────
    bool lowEnoughForWarning = transform.position.y < fallThreshold + 5f;
    
    if (lowEnoughForWarning || Time.frameCount % 60 == 0) // print every second + when close to threshold
    {
        Debug.Log(
            $"[FallDebug] Y = {transform.position.y:F2} | Threshold = {fallThreshold:F1} | " +
            $"Respawning = {isRespawning} | Enabled = {enabled} | FallFlag = {hasAppliedFallDamage}"
        );
    }

    // Original early exit
    if (!enabled || isRespawning)
    {
        if (Time.frameCount % 120 == 0)
        {
            Debug.LogWarning("[FallDebug] Update blocked → !enabled or isRespawning");
        }
        return;
    }

    MobileDebug.Instance?.UpdateDebug(
        $"Y: {transform.position.y:F2} | Threshold: {fallThreshold} | Already fallen: {hasAppliedFallDamage}"
    );

    // كشف السقوط
    if (transform.position.y < fallThreshold)
    {
        Debug.LogWarning($"[FALL] Y = {transform.position.y:F2} → BELOW THRESHOLD! Attempting damage...");

        if (!hasAppliedFallDamage)
        {
            Debug.LogWarning("[FALL] FIRST TIME - Applying instant death");
            MobileDebug.Instance?.UpdateDebug("Fall detected! Applying damage ONCE...");

            if (PlayerDamageable.Instance != null)
            {
                if (PlayerDamageable.Instance.IsAlive)
                {
                    int damage = PlayerDamageable.Instance.MaxHealth + 1;
                    Debug.Log($"[FALL] Applying {damage} damage (maxHealth = {PlayerDamageable.Instance.MaxHealth})");

                    // ──── إضافة للتصحيح ────
                    Debug.LogWarning($"[DEBUG] Calling TakeDamage on {PlayerDamageable.Instance.gameObject.name} (instance valid)");

                    PlayerDamageable.Instance.TakeDamage(damage);

                    // ──── إضافة للتصحيح ────
                    Debug.LogWarning($"[DEBUG] TakeDamage call completed");

                    hasAppliedFallDamage = true;
                    Debug.Log("[FALL] Damage applied successfully - flag set to true");
                }
                else
                {
                    Debug.Log("[FALL] Player already dead - skipping damage");
                    hasAppliedFallDamage = true;
                }
            }
            else
            {
                Debug.LogError("[FALL] CRITICAL: PlayerDamageable.Instance is null!");
            }
        }
        else
        {
            if (Time.frameCount % 300 == 0)
            {
                Debug.Log("[FALL] Already handled - ignoring");
            }
        }
    }

    HandleMovement();
    HandleActions();
}
    private void HandleMovement()
    {
        if (input == null) return;

        float moveInput = 0f;
        if (input.MoveLeft) moveInput = -1f;
        else if (input.MoveRight) moveInput = 1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput != 0)
            transform.localScale = new Vector3(moveInput > 0 ? 1 : -1, 1, 1);

        animator.SetBool(AnimationStrings.canMove, moveInput != 0);
        animator.SetFloat(AnimationStrings.speed, Mathf.Abs(moveInput));
    }

    private void HandleActions()
    {
        if (input == null) return;

        if (input.JumpPressed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            input.ResetJump();
        }

        if (input.AttackPressed && Time.time - lastBowAttackTime >= bowAttackCooldown)
        {
            lastBowAttackTime = Time.time;
            animator.SetTrigger(AnimationStrings.bowAttackTrigger);
            FireArrow();
            input.ResetAttack();
        }
    }

    private void FireArrow()
    {
        if (!arrowPrefab || !arrowSpawnPoint) return;

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        if (arrow.TryGetComponent(out Projectile projectile))
        {
            Vector2 direction = IsFacingRight ? Vector2.right : Vector2.left;
            projectile.SetDirection(direction);
        }
    }

    public void HandleDeath()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;

        animator.SetBool(AnimationStrings.canMove, false);
        animator.SetTrigger(AnimationStrings.deathTrigger);

        enabled = false;
    }

    public void HandleRevive()
    {
        isRespawning = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        enabled = true;
        animator.Rebind();
        animator.Update(0f);

        hasAppliedFallDamage = false; // إعادة تفعيل كشف السقوط بعد الإحياء
    }

    public void ResetPlayer(Vector3 position)
    {
        isRespawning = true;
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = position;

        rb.simulated = true;
        HandleRevive();
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }

    public void RespawnPlayer()
    {
        ResetPlayer(respawnPoint);
    }

    public void ShowRewardedAd(RewardedAd rewardedAd)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((reward) =>
            {
                RespawnPlayer();
            });
        }
        else
        {
            RespawnPlayer();
        }
    }

    public void SetMoveDirection(float direction)
    {
        if (input == null) return;
        input.MoveLeft = direction < 0;
        input.MoveRight = direction > 0;
    }

    public void Jump()
    {
        if (input == null) return;
        input.JumpPressed = true;
    }
}