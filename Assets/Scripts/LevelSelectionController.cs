using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class LevelSelectionController : MonoBehaviour
    {
        [SerializeField] private Button[] levelButtons; // أزرار المستويات (Level_1 إلى Level_4)

        private void Start()
        {
            // الحصول على أعلى مستوى تم فتحه
            int highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 1);

            // تفعيل/تعطيل الأزرار بناءً على أعلى مستوى تم فتحه
            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelIndex = i + 1; // المستويات تبدأ من 1 (Level_1)
                levelButtons[i].interactable = (levelIndex <= highestLevelUnlocked);
            }
        }

        // دوال لتحميل المستويات عند النقر على الأزرار
        public void LoadLevel(int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }
    }
}