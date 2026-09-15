using UnityEngine;
using UnityEngine.UI;
using MyGame;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Damageable player;
    [SerializeField] private Image healthFill;

    private void OnEnable()
    {
        if (player != null)
            player.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int current, int max)
    {
        if (healthFill != null)
        {
            // تحديث كمية الملء
            healthFill.fillAmount = (float)current / max;

            // تغيير اللون بناءً على نسبة الصحة
            float healthPercentage = (float)current / max;
            healthFill.color = Color.Lerp(Color.red, Color.green, healthPercentage);
        }
    }
}