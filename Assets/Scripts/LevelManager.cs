using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace MyGame
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        [Header("References")]
        public GameObject player;
        public Vector3 playerStartPos;

        private bool isFinalLevel = false;

        // ✅ عدد الجواهر المطلوبة لكل مستوى
        private Dictionary<int, int> gemsRequiredPerLevel = new Dictionary<int, int>()
        {
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 4 },
            { 5, 5 }
        };

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
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerStartPos = player.transform.position;
            }

            // Sync CurrentLevel
            if (scene.name.StartsWith("Level"))
            {
                if (int.TryParse(scene.name.Substring(5), out int levelNum))
                {
                    PlayerPrefs.SetInt("CurrentLevel", levelNum);
                    PlayerPrefs.Save();
                }
            }

            ReLinkMovementButtons();

            // تأكد من وجود EventSystem
            var eventSystems = FindObjectsByType<EventSystem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (eventSystems.Length > 1)
            {
                for (int i = 1; i < eventSystems.Length; i++)
                {
                    Destroy(eventSystems[i].gameObject);
                }
            }
            else if (eventSystems.Length == 0)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        public int GetRequiredGemsForCurrentLevel()
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            return gemsRequiredPerLevel.ContainsKey(currentLevel) ? gemsRequiredPerLevel[currentLevel] : 5;
        }

        public void OnGemCollected(int currentGems)
{
    int required = GetRequiredGemsForCurrentLevel();
    Debug.Log($"تم جمع {currentGems}/{required} جواهر");
    
    if (currentGems >= required)
    {
        Debug.Log("🎉 اكتمل المستوى! عرض شاشة الفوز...");
        OnLevelCompleted();
    }
}

        public void OnLevelCompleted()
        {
            // أظهر شاشة الفوز مع خيار مشاهدة الإعلان للمستوى التالي
            GameOverManager.Instance.ShowWinScreenWithAdOption();
        }

        public void UnlockNextLevelAfterAd()
        {
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            int nextLevel = currentLevel + 1;

            if (nextLevel > gemsRequiredPerLevel.Count)
            {
                // المستوى الأخير → انتصار نهائي
                ShowVictoryScene();
            }
            else
            {
                // الانتقال للمستوى التالي
                PlayerPrefs.SetInt("CurrentLevel", nextLevel);
                PlayerPrefs.Save();
                SceneManager.LoadScene("Level" + nextLevel);
            }
        }

        private void ShowVictoryScene()
        {
            var music = GameObject.Find("Music");
            if (music != null) Destroy(music);

            var adManager = FindFirstObjectByType<AdManager>();
            if (adManager != null) Destroy(adManager.gameObject);

            SceneManager.LoadScene("VictoryScene", LoadSceneMode.Single);
            Destroy(gameObject);
        }

        public void ReplayGame()
        {
            PlayerPrefs.SetInt("CurrentLevel", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Level1");
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void RespawnPlayerAtStart()
        {
            if (player != null)
            {
                player.transform.position = playerStartPos;
            }
        }

        public void RestoreAfterAd()
        {
            if (player == null) return;
            var damageable = player.GetComponent<PlayerDamageable>();
            if (damageable != null)
            {
                damageable.RestoreHealth();
            }
            RestorePlayerInput();
            ReLinkMovementButtons();
        }

        public void RestorePlayerInput()
        {
            if (player == null) return;
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = true;
        }

        public void ReLinkMovementButtons()
        {
            var uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ReconnectButtons();
            }
        }
    }
}