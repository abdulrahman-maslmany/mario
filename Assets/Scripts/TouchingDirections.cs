using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    [SerializeField] private ContactFilter2D castFilter;
    [SerializeField] private float groundDistance = 0.05f;
    [SerializeField] private float wallDistance = 0.2f;
    [SerializeField] private float ceilingDistance = 0.05f;

    private Animator animator;
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    [SerializeField] private new Collider2D collider2D;

    private bool _isGrounded;
    private bool _isOnWall;
    private bool _isOnCeiling;

    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            _isGrounded = value;
            animator.SetBool("isGrounded", value);
        }
    }

    public bool IsOnWall
    {
        get => _isOnWall;
        private set
        {
            _isOnWall = value;
            animator.SetBool("isOnWall", value);
        }
    }

    public bool IsOnCeiling
    {
        get => _isOnCeiling;
        private set
        {
            _isOnCeiling = value;
            animator.SetBool("isOnCeiling", value);
        }
    }

    private Vector2 WallCheckDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        if (collider2D == null) Debug.LogError("Collider2D is missing on this GameObject!");
        if (animator == null) Debug.LogError("Animator is missing on this GameObject!");
    }

    private void FixedUpdate()
    {
        IsGrounded = collider2D.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = collider2D.Cast(WallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCeiling = collider2D.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }
}