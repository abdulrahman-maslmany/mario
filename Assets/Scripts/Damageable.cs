using System;
using UnityEngine;

namespace MyGame
{
    public class Damageable : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] protected int maxHealth = 100;
        protected int currentHealth;

        public bool IsAlive => currentHealth > 0;
        public int Health => currentHealth;
        public int MaxHealth => maxHealth;

        public event Action<int, int> OnHealthChanged; // current, max
        public event Action OnDeath;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
            InvokeOnHealthChanged(currentHealth, maxHealth);
        }

        public virtual void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            currentHealth -= damage;
            InvokeOnHealthChanged(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Heal(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            InvokeOnHealthChanged(currentHealth, maxHealth);
        }

        public virtual void ResetHealth()
        {
            currentHealth = maxHealth;
            InvokeOnHealthChanged(currentHealth, maxHealth);
        }

        protected virtual void Die()
        {
            Debug.Log("Base Damageable Die called! (This should NOT appear for Player)"); // للتشخيص فقط
            OnDeath?.Invoke();
        }

        protected void InvokeOnHealthChanged(int current, int max)
        {
            OnHealthChanged?.Invoke(current, max);
        }
    }
}