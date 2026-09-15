using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public Vector3 moveSpeed = new Vector3(0, 75, 0);
    public float timeToFade = 1f;

    private TextMeshProUGUI textMeshPro;
    private RectTransform textTransform;
    private float timeElapsed = 0f;
    private Color startColor;

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        timeElapsed = 0f;
    }

    private void Update()
    {
        textTransform.position += moveSpeed * Time.deltaTime;
        timeElapsed += Time.deltaTime;

        if (timeElapsed < timeToFade)
        {
            float fadeAlpha = startColor.a * (1 - (timeElapsed / timeToFade));
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetText(string text, Color color)
    {
        textMeshPro.text = text;
        startColor = color;
        textMeshPro.color = startColor;
        timeElapsed = 0f;
        gameObject.SetActive(true);
    }
}
