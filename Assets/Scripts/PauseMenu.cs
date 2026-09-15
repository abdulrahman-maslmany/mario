using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private bool isPaused = false;

    private void Awake()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
    }

    private void Start()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        // 👇 عند الضغط على زر Escape أو زر Back على الموبايل
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
