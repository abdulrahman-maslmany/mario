using UnityEngine;

namespace MyGame
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private int damageAmount = 10;
        public int DamageAmount => damageAmount; // 🔥 للوصول من EnemyDamageable

        private Vector2 direction;

        public void SetDirection(Vector2 dir) => direction = dir.normalized;

        private void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Damageable damageable))
            {
                damageable.TakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }
}
