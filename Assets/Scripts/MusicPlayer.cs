using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance; // متغير لتتبع النسخة الوحيدة
    private AudioSource audioSource;

    private void Awake()
    {
        // التحقق مما إذا كان هناك نسخة أخرى من MusicPlayer
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // تدمير النسخة الجديدة إذا كان هناك نسخة موجودة بالفعل
            return;
        }

        instance = this; // تعيين هذه النسخة كالنسخة الوحيدة
        DontDestroyOnLoad(gameObject); // منع التدمير عند تحميل مشهد جديد

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}