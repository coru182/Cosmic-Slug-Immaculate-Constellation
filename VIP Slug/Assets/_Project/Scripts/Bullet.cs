using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 14f;
    [SerializeField] private float lifetime = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.right;
    private float lifetimeTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        lifetimeTimer = lifetime;
    }

    private void Update()
    {
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    public void SetDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0f)
        {
            moveDirection = direction.normalized;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
