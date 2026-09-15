using UnityEngine;
using UnityEngine.UI;
using MyGame; // إذا كنت تستخدم GameOverManager أو LevelManager هنا (اختياري)

public class AdConfirmationPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Text messageText;         // أو TMP_Text إذا كنت تستخدم TextMeshPro
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private System.Action onConfirmedAction;

    private void Awake()
    {
        // إخفاء الحوار في البداية
        if (popupPanel != null)
            popupPanel.SetActive(false);

        // ربط الأزرار
        if (yesButton != null)
            yesButton.onClick.AddListener(OnYesClicked);

        if (noButton != null)
            noButton.onClick.AddListener(OnNoClicked);
    }

    /// <summary>
    /// عرض حوار التأكيد للإعلان المكافئ
    /// </summary>
    /// <param name="rewardDescription">وصف المكافأة، مثل "حياة إضافية" أو "100 عملة"</param>
    /// <param name="onConfirmed">الإجراء الذي يتم تنفيذه إذا ضغط "نعم" (عادة عرض الإعلان)</param>
    public void Show(string rewardDescription, System.Action onConfirmed)
    {
        if (popupPanel == null || messageText == null)
        {
            Debug.LogError("[AdConfirmationPopup] بعض العناصر غير مربوطة في Inspector!");
            onConfirmed?.Invoke(); // fallback
            return;
        }

        messageText.text = $"شاهد إعلاناً قصيراً لتحصل على {rewardDescription}؟";
        onConfirmedAction = onConfirmed;
        popupPanel.SetActive(true);

        // وقف اللعبة أثناء الحوار
        Time.timeScale = 0f;
    }

    private void OnYesClicked()
    {
        popupPanel.SetActive(false);
        Time.timeScale = 1f;
        onConfirmedAction?.Invoke(); // عرض الإعلان الآن
    }

    private void OnNoClicked()
    {
        popupPanel.SetActive(false);
        Time.timeScale = 1f;

        // العودة إلى شاشة الخسارة (اختياري)
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowLoseScreen();
        }
    }

    // دالة مساعدة لإخفاء الحوار يدويًا إذا لزم الأمر
    public void Hide()
    {
        popupPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}