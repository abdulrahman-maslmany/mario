using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sprite activatedSprite; // لتغيير شكل الـ Checkpoint عند التفعيل (اختياري)
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated) return;

        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out PlayerController player))
            {
                player.SetRespawnPoint(transform.position);
                Debug.Log("Checkpoint activated at: " + transform.position);

                isActivated = true;
                if (activatedSprite != null && TryGetComponent(out SpriteRenderer sr))
                {
                    sr.sprite = activatedSprite;
                }
            }
        }
    }
}
