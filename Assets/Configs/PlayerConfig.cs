using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig", order = 1)]
    public class PlayerConfig : ScriptableObject
    {
        public float moveSpeed = 5f;
        public float jumpForce = 5f;
        public float attackCooldown = 0.5f;
        public float bowAttackCooldown = 1f;
        public int meleeDamage = 5;
        public Vector2 meleeKnockback = new Vector2(5f, 2f);
        public int bowDamage = 3;
        public Vector2 bowKnockback = new Vector2(3f, 0f);
    }
}