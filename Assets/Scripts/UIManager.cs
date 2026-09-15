using UnityEngine;
using MyGame; // 🔥 إضافة: لـ LevelManager، GameOverManager، IResettableEnemy
using System.Linq;  // 🔥 لاستخدام OfType<> في البحث عن الـ Interface

public class UIManager : MonoBehaviour
{
    // [Header("UI References")]
    // [SerializeField] private GameObject gameOverPanel;
    // [SerializeField] private Button retryButton;

    public void OnRetryButton()
    {
        Debug.Log("[UIManager] Retry button pressed ✅");

        // إخفاء شاشة GameOver
        GameOverManager.Instance.HideLoseScreen();

        // أعد إحياء اللاعب
        LevelManager.Instance.RespawnPlayerAtStart(); // 🔥 إصلاح: الآن public

        // 🔥 إصلاح: ابحث عن الأعداء عبر IResettableEnemy للتوافق مع جميع الأنواع
        var enemies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IResettableEnemy>();
        foreach (var enemy in enemies)
        {
            enemy?.ResetEnemy();
        }

        Time.timeScale = 1f;

        Debug.Log("[UIManager] Retry completed: Player + Enemies reset, Time resumed.");
    }

    public void ReconnectButtons()
    {
        Debug.Log("[UIManager] Reconnected movement & UI buttons ✅");

        var buttons = FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None);
        foreach (var btn in buttons)
            btn.interactable = true;

        // Add more reconnection logic if needed, e.g., re-adding listeners
    }

    // ... باقي الكود للـ UI (مثل Pause، Settings، إلخ) ...
}