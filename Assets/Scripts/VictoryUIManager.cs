using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;

    void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void OnReplayButton()
    {
        Debug.Log("🔁 Replay pressed - Restarting from Level 1");
        SceneManager.LoadScene("Level1");
    }

    public void OnMainMenuButton()
    {
        Debug.Log("🏠 Main menu pressed - Returning to main menu");
        SceneManager.LoadScene("MainMenu");
    }
}
