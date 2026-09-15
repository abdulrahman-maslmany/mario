using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button quitButton;

    private void Start()
    {
        // ربط الأزرار بالوظائف
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void PlayGame()
    {
        // استبدل "Level_1" باسم المشهد الذي يحتوي لعبتك
        SceneManager.LoadScene("Level_1");
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // للخروج أثناء التشغيل في Unity Editor
#else
        Application.Quit(); // للخروج على الهاتف أو Build
#endif
    }
}
