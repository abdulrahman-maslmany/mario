using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private int firstLevelIndex = 1; 
        // رقم المشهد في Build Settings (مثلاً 1 يعني ثاني مشهد بعد MainMenu)

        public void OnStartButtonClicked()
        {
            Debug.Log("🔹 Start Button Clicked! Trying to load level index: " + firstLevelIndex);

            if (firstLevelIndex >= 0 && firstLevelIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(firstLevelIndex);
            }
            else
            {
                Debug.LogError("❌ المشهد بالمؤشر " + firstLevelIndex + " غير موجود في Build Settings!");
            }
        }

        public void OnQuitButtonClicked()
        {
            Debug.Log("🚪 Quit Button Clicked! Exiting game...");
            Application.Quit();
        }
    }
}
