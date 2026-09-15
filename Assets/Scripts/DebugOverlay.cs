using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // ✅ أضف هذا السطر


public class DebugOverlay : MonoBehaviour
{
    private TMP_Text debugText;
    private Canvas canvas;

    private void Awake()
    {
        // إن لم يكن هناك نسخة سابقة، أنشئ العنصر وثبته
        if (FindFirstObjectByType<DebugOverlay>() != this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // إنشاء Canvas بسيط فوق اللعبة
        canvas = new GameObject("DebugCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        DontDestroyOnLoad(canvas.gameObject);

        // إضافة نص
        GameObject textObj = new GameObject("DebugText");
        textObj.transform.SetParent(canvas.transform);
        debugText = textObj.AddComponent<TextMeshProUGUI>();

        debugText.fontSize = 24;
        debugText.alignment = TextAlignmentOptions.TopLeft;
        debugText.color = Color.yellow;
        debugText.rectTransform.anchorMin = new Vector2(0, 1);
        debugText.rectTransform.anchorMax = new Vector2(0, 1);
        debugText.rectTransform.pivot = new Vector2(0, 1);
        debugText.rectTransform.anchoredPosition = new Vector2(10, -10);
    }

    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        int current = 0, required = 0;
        if (GemCounterUI.Instance != null)
        {
            var ui = GemCounterUI.Instance;
            var type = ui.GetType();

            // نقرأ القيم عبر Reflection لأن المتغيرات خاصة
            current = (int)type.GetField("currentGems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(ui);
            required = (int)type.GetField("requiredGems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(ui);
        }

        debugText.text =
            $"🧭 Scene: {sceneName}\n" +
            $"💎 Gems: {current}/{required}\n" +
            $"📜 BuildIndex: {SceneManager.GetActiveScene().buildIndex}";
    }
}
