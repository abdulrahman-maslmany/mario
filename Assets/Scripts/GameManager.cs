using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI; // إضافة لـ Button
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Vector3 lastCheckpointPosition;
    private bool checkpointSet = false;

    [Header("Internet Check Settings")]
    [SerializeField] private GameObject noInternetPanelPrefab;  // بريفاب للبانل (إذا كنت تريد إنشاءه ديناميكيًا)
    [SerializeField] private float checkInterval = 10f;         // كل كام ثانية يتحقق
    [SerializeField] private bool requireInternetToStart = true; // هل يمنع تحميل المشهد إذا مفيش نت في البداية؟

    private GameObject noInternetPanelInstance; // نسخة البانل المُنشأة
    private bool isPaused = false;
    private Button retryButton; // مرجع للزر للتحكم فيه (اختياري لتعطيله أثناء التحقق)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // تحقق فوري في البداية
        StartCoroutine(CheckInternetAtStart());
    }

    private IEnumerator CheckInternetAtStart()
    {
        yield return CheckInternetConnection();

        // إذا مفيش نت وتريد منع التحميل، ابقى في مشهد التحميل أو أظهر بانل
        if (requireInternetToStart && isPaused)
        {
            Debug.Log("[GameManager] No internet at start. Waiting for connection...");
        }
        else
        {
            // إذا فيه نت، حمل المشهد الرئيسي إذا لزم الأمر (غير مطلوب إذا كنت بالفعل فيه)
            // SceneManager.LoadScene("MainMenu"); // إذا أردت استخدام mainSceneName، أعد تعريفه واستخدمه هنا
        }

        // ابدأ التحقق الدوري بعد التحقق الأولي
        StartCoroutine(CheckInternetLoop());
    }

    // ✅ التحقق بشكل دوري من وجود إنترنت
    private IEnumerator CheckInternetLoop()
    {
        while (true)
        {
            yield return CheckInternetConnection();
            yield return new WaitForSecondsRealtime(checkInterval); // استخدم Realtime لتجاهل timeScale
        }
    }

    private IEnumerator CheckInternetConnection()
    {
        // تعطيل الزر أثناء التحقق (إذا وجد)
        if (retryButton != null)
            retryButton.interactable = false;

        // استخدم HTTPS لأمان أفضل وتوافق حديث
        UnityWebRequest request = new UnityWebRequest("https://connectivitycheck.gstatic.com/generate_204");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = 5;

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        bool hasInternet = (request.result == UnityWebRequest.Result.Success);
#else
        bool hasInternet = !(request.isNetworkError || request.isHttpError);
#endif

        if (!hasInternet)
        {
            // مفيش نت → يظهر بانل ويوقف اللعبة
            ShowNoInternetPanel(true);
            if (!isPaused)
            {
                Time.timeScale = 0f; // ⛔ وقف اللعب
                isPaused = true;
            }
        }
        else
        {
            // فيه نت → يخفي البانل ويرجع اللعبة تشتغل
            ShowNoInternetPanel(false);
            if (isPaused)
            {
                Time.timeScale = 1f; // ✅ رجع اللعب
                isPaused = false;
                
                // 🔥 إضافة: أعد تهيئة UI بعد إعادة timeScale (للأزرار على Android)
                EnsureUIActive();
            }
        }

        // إعادة تفعيل الزر بعد التحقق
        if (retryButton != null)
            retryButton.interactable = true;
    }

    private void ShowNoInternetPanel(bool show)
    {
        // ابحث عن البانل في المشهد الحالي (افترض اسمه "NoInternetPanel")
        GameObject panel = GameObject.Find("NoInternetPanel");

        if (panel != null)
        {
            panel.SetActive(show);
            if (show) SetupRetryButton(panel); // ربط الزر إذا وجد
            return;
        }

        // إذا مش موجود، أنشئ من البريفاب (افترض أنه يحتاج كانفاس)
        if (show && noInternetPanelPrefab != null && noInternetPanelInstance == null)
        {
            // 🔥 إصلاح التحذير CS0618: استخدم FindFirstObjectByType بدلاً من FindObjectOfType
            Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                noInternetPanelInstance = Instantiate(noInternetPanelPrefab, canvas.transform);
                noInternetPanelInstance.SetActive(true);
                SetupRetryButton(noInternetPanelInstance); // ربط الزر
                Debug.Log("[GameManager] NoInternetPanel instantiated.");
            }
            else
            {
                Debug.LogWarning("[GameManager] No Canvas found to parent NoInternetPanel.");
            }
        }
        else if (!show && noInternetPanelInstance != null)
        {
            Destroy(noInternetPanelInstance);
            noInternetPanelInstance = null;
            retryButton = null; // إعادة تعيين المرجع
        }
    }

    private void SetupRetryButton(GameObject panel)
    {
        // ابحث عن الزر داخل البانل
        retryButton = panel.GetComponentInChildren<Button>();
        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners(); // إزالة الاستماعات السابقة
            retryButton.onClick.AddListener(ManualCheckInternet); // ربط الدالة
            Debug.Log("[GameManager] RetryButton setup complete.");
        }
        else
        {
            Debug.LogWarning("[GameManager] RetryButton not found in NoInternetPanel. Add a Button named 'RetryButton' to the panel.");
        }
    }

    // دالة عامة للتحقق اليدوي (تُنادى من الزر)
    public void ManualCheckInternet()
    {
        StartCoroutine(CheckInternetConnection());
        Debug.Log("[GameManager] Manual internet check initiated by user.");
    }

    // 🔥 إضافة جديدة: دالة لإعادة تفعيل UI (للأزرار على Android بعد timeScale)
    public void EnsureUIActive() // جعلتها public
    {
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None); // 🔥 إصلاح: استخدم FindObjectsByType مع None
        foreach (var canvas in canvases)
        {
            canvas.enabled = true;
            Debug.Log("[GameManager] Canvas reactivated: " + canvas.name);
        }
        
        // أعد تفعيل الأزرار إذا كانت معطلة
        Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None); // 🔥 إصلاح: استخدم FindObjectsByType مع None
        foreach (var button in buttons)
        {
            button.interactable = true;
        }

        // 🔥 إضافة: إعادة تعيين EventSystem
        //if (LevelManager.Instance != null)
        {
          //  LevelManager.Instance.EnsureEventSystem();
        }
    }

    // ----------------- باقي وظائف GameManager -----------------

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        checkpointSet = true;
        Debug.Log("[GameManager] Checkpoint saved at: " + position);
    }

    public Vector3 LastCheckpointPosition => lastCheckpointPosition;
    public bool HasCheckpoint => checkpointSet;

    public void RespawnPlayer(GameObject playerObj)
    {
        if (!checkpointSet)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player != null)
        {
            player.RespawnPlayer();
            Debug.Log("[GameManager] Player respawned at checkpoint via PlayerController");
        }
        else
        {
            playerObj.transform.position = lastCheckpointPosition;
            Debug.Log("[GameManager] Player respawned at checkpoint (fallback)");
        }
    }

    public void ResetGame()
    {
        checkpointSet = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}