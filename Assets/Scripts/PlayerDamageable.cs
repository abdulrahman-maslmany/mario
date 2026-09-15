using System;
using UnityEngine;
using MyGame.Animation;

namespace MyGame
{
    public class PlayerDamageable : Damageable
    {
        public static PlayerDamageable Instance { get; private set; }

        public event Action OnRevive;

        private new void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("✅ PlayerDamageable Instance set correctly!");

                currentHealth = maxHealth;
                Debug.Log($"[INIT] Player health set to {currentHealth}/{maxHealth} in Awake");
            }
            else
            {
                Debug.LogWarning("⚠️ Duplicate PlayerDamageable found! Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        protected virtual void Start()
        {
            Debug.Log($"[START] Player health at scene start: {currentHealth}/{maxHealth}");
        }

        public override void TakeDamage(int damage)
        {
            Debug.LogWarning($"[TAKE-DAMAGE] Someone called TakeDamage with {damage} | current = {currentHealth}/{maxHealth}");

            Debug.Log($"[TakeDamage] Called | damage = {damage} | currentHealth BEFORE = {currentHealth}/{maxHealth} | IsAlive = {IsAlive}");

            if (currentHealth <= 0)
            {
                Debug.Log("[TakeDamage] Player already dead → ignoring additional damage");
                return;
            }

            currentHealth -= damage;
            if (currentHealth < 0)
                currentHealth = 0;

            Debug.Log($"[TakeDamage] Health AFTER = {currentHealth}/{maxHealth}");
            InvokeOnHealthChanged(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Debug.Log("[TakeDamage] Health <= 0 → calling Die()");
                Die();
            }
            else
            {
                Debug.Log("[TakeDamage] Still alive after damage");
            }
        }

        protected override void Die()
        {
            Debug.Log("💀💀💀 [Die] PLAYER DIED – starting death sequence 💀💀💀");

            // 1. تعطيل التحكم والفيزياء + أنيميشن الموت فورًا
            var controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                Debug.Log("[Die] Found PlayerController → calling HandleDeath()");
                controller.HandleDeath();
            }
            else
            {
                Debug.LogError("[Die] CRITICAL: No PlayerController component found!");
            }

            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log("[Die] Found Animator → setting death trigger");
                animator.SetTrigger(AnimationStrings.deathTrigger);
            }
            else
            {
                Debug.LogError("[Die] No Animator found on player!");
            }

            // 2. أظهر شاشة الخسارة فورًا (مع خيار مشاهدة إعلان Rewarded)
            ShowGameOverUI();

            // لا تعرض Interstitial هنا — الـ Rewarded Ad يتم تشغيله من زر الشاشة فقط

            base.Die();
        }

        private void ShowGameOverUI()
        {
            Debug.Log("[ShowGameOverUI] Attempting to show lose screen");

            if (GameOverManager.Instance != null)
            {
                Debug.Log("[ShowGameOverUI] GameOverManager found → calling ShowLoseScreen()");
                GameOverManager.Instance.ShowLoseScreen();
            }
            else
            {
                Debug.LogError("❌ [ShowGameOverUI] GameOverManager.Instance is null! Cannot show lose screen.");
            }
        }

        public void ReviveAfterAd()
        {
            Debug.Log("[ReviveAfterAd] Starting revive sequence");
            RestoreHealth();
            gameObject.SetActive(true);

            var controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                Debug.Log("[ReviveAfterAd] Calling HandleRevive on controller");
                controller.HandleRevive();
            }

            LevelManager.Instance?.RestoreAfterAd();
            LevelManager.Instance?.ReLinkMovementButtons();

            // إخفاء شاشة الخسارة بعد الإحياء (مهم جدًا)
            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.HideLoseScreen();
            }

            OnRevive?.Invoke();
            Debug.Log("✅ [ReviveAfterAd] Player revived after rewarded ad!");
        }

        public void RespawnAtLevelStart()
        {
            Debug.Log("[RespawnAtLevelStart] Respawning player at level start");
            RestoreHealth();

            if (LevelManager.Instance != null)
            {
                transform.position = LevelManager.Instance.playerStartPos;
            }

            gameObject.SetActive(true);

            var controller = GetComponent<PlayerController>();
            if (controller != null)
                controller.HandleRevive();

            LevelManager.Instance?.ReLinkMovementButtons();

            // إخفاء شاشة الخسارة بعد الـ Respawn
            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.HideLoseScreen();
            }

            Debug.Log("✅ Player respawned at level start!");
        }

        public void RestoreHealth()
        {
            currentHealth = maxHealth;
            InvokeOnHealthChanged(currentHealth, maxHealth);
            Debug.Log("❤️ Player health restored!");
        }
    }
}