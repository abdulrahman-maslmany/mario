using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using MyGame;  // Add this to access LevelManager

public class GemCounterUI : MonoBehaviour
{
    public static GemCounterUI Instance;

    [Header("UI Elements")]
    public TMP_Text gemText;

    [Header("Loading Screen UI")]
    public GameObject loadingPanel;
    public TMP_Text levelNameText;
    public TMP_Text loadingText;
    public TMP_Text gemsRequiredText;
    public Slider progressBar;

    private int currentGems = 0;
    private int requiredGems = 5;
    private int currentLevelIndex;

    public int CurrentGems => currentGems;  // Add this public getter for access from other scripts

    public event Action<int, int> OnGemsUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        // 🔥 حساب عدد الجواهر المطلوبة بناءً على رقم المستوى
        requiredGems = Mathf.Clamp(currentLevelIndex, 1, 5);
    }

    private void Start()
    {
        OnGemsUpdated += UpdateGemUI;
        // أضف: مزامنة مع LevelManager لتجنب عدم التوافق
        if (LevelManager.Instance != null)
        {
            requiredGems = LevelManager.Instance.GetRequiredGemsForCurrentLevel();
        }
        UpdateGemUI(currentGems, requiredGems);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        OnGemsUpdated -= UpdateGemUI;
    }

    public void AddGems(int amount)
    {
        currentGems += amount;
        OnGemsUpdated?.Invoke(currentGems, requiredGems);
        // Removed completion check to prevent double call; handled in LevelManager.OnGemCollected
    }

    public void SetRequiredGems(int required)
    {
        requiredGems = required;
        currentGems = 0;
        OnGemsUpdated?.Invoke(currentGems, requiredGems);
    }

    private void UpdateGemUI(int current, int required)
    {
        if (gemText != null)
            gemText.text = $"{current}/{required}";
    }
}