using UnityEngine;
using UnityEngine.UI;

public class MobileDebug : MonoBehaviour
{
    [SerializeField] private Text debugText; // اسحب Text UI هنا في Inspector (أضف Canvas > Text إذا لم يوجد)

    // للاتصال من EnemyController، أضف هذا في FixedUpdate() (مؤقتاً):
    // MobileDebug.Instance?.UpdateDebug($"Distance: {distanceToPlayer}\nHits: {hits.Length}\nInAttack: {isInAttackRange}");

    public static MobileDebug Instance;

    private void Awake()
    {
        Instance = this;
        if (debugText == null)
            debugText = GetComponent<Text>(); // إذا كان الـ Script على الـ Text نفسه
    }

    public void UpdateDebug(string message)
    {
        if (debugText != null)
            debugText.text = message;
        else
            Debug.Log("[MobileDebug] " + message); // fallback للـ Editor
    }
    }