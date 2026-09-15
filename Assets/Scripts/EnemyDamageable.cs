using UnityEngine;

namespace MyGame
{
    public class EnemyDamageable : Damageable
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Arrow"))
            {
                Arrow arrow = collision.GetComponent<Arrow>();
                if (arrow != null)
                {
                    TakeDamage(arrow.DamageAmount); // ✅ يأخذ الضرر من السهم
                    Destroy(collision.gameObject);   // ✅ تدمير السهم بعد الإصابة
                }
            }
        }

        protected override void Die()
        {
            Debug.Log($"{gameObject.name} died!");
            base.Die(); // ✅ استدعاء الحدث OnDeath من الكلاس الأب بشكل صحيح
            Destroy(gameObject); // ✅ تدمير العدو
        }
    }
}
