using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("PlayGame called!");
        SceneManager.LoadScene("Level_1");
    }
}