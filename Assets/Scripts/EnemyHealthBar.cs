using UnityEngine;
using UnityEngine.UI;
using MyGame;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyDamageable enemy;
    [SerializeField] private Image healthFill;
    [SerializeField] private Gradient healthGradient;

    private void OnEnable()
    {
        if (enemy != null)
            enemy.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        if (enemy != null)
            enemy.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int current, int max)
    {
        if (healthFill != null && healthGradient != null)
        {
            healthFill.fillAmount = (float)current / max;
            float healthPercentage = (float)current / max;
            healthFill.color = healthGradient.Evaluate(healthPercentage);
        }
    }

    private void OnValidate()
    {
        if (healthGradient == null)
        {
            healthGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.red, 0f);
            colorKeys[1] = new GradientColorKey(Color.yellow, 0.5f);
            colorKeys[2] = new GradientColorKey(Color.green, 1f);
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[1];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            healthGradient.SetKeys(colorKeys, alphaKeys);
        }
    }
}