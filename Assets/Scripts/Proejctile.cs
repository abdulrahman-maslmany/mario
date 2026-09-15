using UnityEngine;
using MyGame;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private Vector2 moveSpeed = new Vector2(5f, 0f);

    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void Start() => Destroy(gameObject, 5f);

    private void FixedUpdate() => rb.linearVelocity = direction * moveSpeed;

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
        transform.localScale = new Vector3(direction.x < 0 ? -1 : 1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.TakeDamage(damage);

            Rigidbody2D rbTarget = other.GetComponent<Rigidbody2D>();
            if (rbTarget != null)
                rbTarget.AddForce(direction * 2f, ForceMode2D.Impulse);

            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
