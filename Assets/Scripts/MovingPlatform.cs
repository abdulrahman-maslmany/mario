using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("حركة الجسر")]
    public Transform نقطة_البداية;  // نقطة البداية (اسحب GameObject فارغ هنا)
    public Transform نقطة_النهاية;   // نقطة النهاية (اسحب GameObject فارغ هنا)
    public float سرعة_الحركة = 2f;   // سرعة الجسر
    public bool حركة_تلقائية = true; // هل يتحرك تلقائيًا؟

    private Vector3 نقطة_حالية;
    private float الانتظار_في_النقطة = 1f; // وقت الانتظار في كل نقطة (ثواني)
    private bool ينتظر = false;

    void Start()
    {
        if (نقطة_البداية == null) نقطة_البداية = transform; // إذا لم تحدد، ابدأ من الموقع الحالي
        نقطة_حالية = نقطة_البداية.position;
    }

    void Update()
    {
        if (حركة_تلقائية && !ينتظر)
        {
            // حرك الجسر نحو النقطة التالية
            transform.position = Vector3.MoveTowards(transform.position, نقطة_حالية, سرعة_الحركة * Time.deltaTime);

            // إذا وصل، انتظر ثم غيِّر الاتجاه
            if (Vector3.Distance(transform.position, نقطة_حالية) < 0.01f)
            {
                StartCoroutine(انتظار_ثم_تغيير_الاتجاه());
            }
        }
    }

    IEnumerator انتظار_ثم_تغيير_الاتجاه()
    {
        ينتظر = true;
        yield return new WaitForSeconds(الانتظار_في_النقطة);
        if (نقطة_حالية == نقطة_البداية.position)
            نقطة_حالية = نقطة_النهاية.position;
        else
            نقطة_حالية = نقطة_البداية.position;
        ينتظر = false;
    }

    // ربط اللاعب بالجسر عند الوقوف عليه
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // تأكد من أن اللاعب لديه Tag "Player"
        {
            collision.transform.SetParent(transform); // اربط اللاعب بالجسر
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // أفلت اللاعب
        }
    }
}