using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class GameOverManager : MonoBehaviour
    {
        public static GameOverManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject losePanel;     // شاشة الخسارة (Revive / Retry / Menu)
        [SerializeField] private GameObject winPanel;      // شاشة الفوز (Watch Ad to Next Level)

        [Header("Buttons in Lose Panel")]
        [SerializeField] private Button watchAdLoseButton; // Watch Ad للإحياء في الخسارة
        [SerializeField] private Button retryButton;       // Retry
        [SerializeField] private Button menuButtonLose;    // Menu من lose

        [Header("Buttons in Win Panel")]
        [SerializeField] private Button watchAdWinButton;  // Watch Ad to unlock next level
        [SerializeField] private Button noThanksButton;    // No Thanks / Menu من win

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // إخفاء الشاشات في البداية
            if (losePanel != null) losePanel.SetActive(false);
            if (winPanel != null) winPanel.SetActive(false);
        }

        // ────────────────────────────────────────────────────────
        //                  شاشة الخسارة (Lose Panel)
        // ────────────────────────────────────────────────────────
        public void ShowLoseScreen()
        {
            if (losePanel == null)
            {
                Debug.LogError("[GameOverManager] Lose Panel is not assigned in Inspector!");
                return;
            }

            losePanel.SetActive(true);
            if (winPanel != null) winPanel.SetActive(false);

            Time.timeScale = 0f;

            Debug.Log("[GameOverManager] Lose screen shown successfully!");

            // ربط الأزرار ديناميكيًا
            if (watchAdLoseButton != null)
            {
                watchAdLoseButton.onClick.RemoveAllListeners();
                watchAdLoseButton.onClick.AddListener(OnWatchAdLoseClicked);
            }

            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(OnRetryClicked);
            }

            if (menuButtonLose != null)
            {
                menuButtonLose.onClick.RemoveAllListeners();
                menuButtonLose.onClick.AddListener(OnMenuClicked);
            }
        }

        private void OnWatchAdLoseClicked()
        {
            HideLoseScreen();

            if (AdManager.Instance == null)
            {
                Debug.LogWarning("[Lose] AdManager null → re-show lose screen");
                ShowLoseScreen();
                return;
            }

            Debug.Log("[Lose] Showing Rewarded Ad for revive...");

            AdManager.Instance.ShowRewardedAd(
                onUserRewarded: () =>
                {
                    Debug.Log("[Rewarded] Success → Reviving player");
                    PlayerDamageable.Instance?.ReviveAfterAd();
                },
                onClosed: () =>
                {
                    Debug.Log("[Rewarded] Ad closed → re-showing lose screen");
                    ShowLoseScreen(); // ← هنا السبب الرئيسي في إعادة الظهور بعد الإعلان
                    // لو عايز الشاشة ما ترجعش بعد الإعلان، احذف أو علّق السطر السابق
                }
            );
        }

        private void OnRetryClicked()
        {
            HideLoseScreen();
            PlayerDamageable.Instance?.RespawnAtLevelStart();
            Time.timeScale = 1f;
        }

        private void OnMenuClicked()
        {
            HideLoseScreen();
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu"); // غيّر الاسم حسب اسم مشهد القائمة
        }

        public void HideLoseScreen()
        {
            if (losePanel != null) losePanel.SetActive(false);
            Time.timeScale = 1f;
        }

        // ────────────────────────────────────────────────────────
        //                  شاشة الفوز (Win Panel → Next Level)
        // ────────────────────────────────────────────────────────
        public void ShowWinScreenWithAdOption()
        {
            if (winPanel == null)
            {
                Debug.LogError("[GameOverManager] Win Panel is not assigned!");
                return;
            }

            winPanel.SetActive(true);
            if (losePanel != null) losePanel.SetActive(false);

            Time.timeScale = 0f;

            Debug.Log("[GameOverManager] Win screen with Ad option shown!");

            // ربط أزرار شاشة الفوز
            if (watchAdWinButton != null)
            {
                watchAdWinButton.onClick.RemoveAllListeners();
                watchAdWinButton.onClick.AddListener(OnWatchAdForNextLevel);
            }

            if (noThanksButton != null)
            {
                noThanksButton.onClick.RemoveAllListeners();
                noThanksButton.onClick.AddListener(OnNoThanksWin);
            }
        }

       private void OnWatchAdForNextLevel()
{
    HideWinScreen();

    Debug.Log("[Win] بدء عرض إعلان للانتقال للمستوى التالي");

    AdManager.Instance.ShowRewardedAd(
        onUserRewarded: () =>
        {
            Debug.Log("[Win → Reward] نجاح! → فتح المستوى التالي الآن");
            LevelManager.Instance.UnlockNextLevelAfterAd();
        },
        onClosed: () =>
        {
            Debug.Log("[Win → Closed] الإعلان أغلق – هل تمت المكافأة أم لا؟");
            // لا تعيد إظهار الشاشة تلقائياً هنا إلا إذا أردت إجبار إعادة المحاولة
            // ShowWinScreenWithAdOption();   ← علّق هذا السطر مؤقتاً
        }
    );
}

        private void OnNoThanksWin()
        {
            HideWinScreen();
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private void HideWinScreen()
        {
            if (winPanel != null) winPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}