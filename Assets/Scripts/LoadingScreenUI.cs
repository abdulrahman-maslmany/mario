using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenUI : MonoBehaviour
{
    public static LoadingScreenUI Instance;

    [Header("UI Elements")]
    public GameObject loadingPanel;
    public Slider progressBar;
    public TMP_Text loadingText;
    public TMP_Text levelNameText;
    public TMP_Text gemsRequiredText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    public void LoadLevel(int sceneIndex, int gemsRequired)
    {
        StartCoroutine(LoadLevelAsync(sceneIndex, gemsRequired));
    }

    private IEnumerator LoadLevelAsync(int sceneIndex, int gemsRequired)
    {
        loadingPanel.SetActive(true);
        progressBar.value = 0f;

        string levelName = $"Level {sceneIndex + 1}";
        if (levelNameText != null)
            levelNameText.text = levelName;

        if (gemsRequiredText != null)
            gemsRequiredText.text = $"Required Gems: {gemsRequired}";

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            progressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingText.text = $"Loading... {Mathf.RoundToInt(progressBar.value * 100)}%";

            if (asyncLoad.progress >= 0.9f)
            {
                loadingText.text = "Tap to continue...";
                if (Input.anyKeyDown || Input.touchCount > 0)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        loadingPanel.SetActive(false);
    }
}
