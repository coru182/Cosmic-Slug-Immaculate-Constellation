using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 8f;

    [Tooltip("Allows jumping for a short time after leaving the ground. Typical tuning range: 0.05-0.2 seconds.")]
    [SerializeField, Min(0f)] private float coyoteTime = 0.1f;

    [Tooltip("Stores jump input briefly before landing so jumps still trigger. Typical tuning range: 0.05-0.2 seconds.")]
    [SerializeField, Min(0f)] private float jumpBufferTime = 0.1f;

    [Tooltip("When jump is released while rising, upward velocity is multiplied by this value. Lower = shorter hop. Typical range: 0.3-0.8.")]
    [SerializeField, Range(0f, 1f)] private float jumpReleaseVelocityMultiplier = 0.5f;

    [SerializeField] private LayerMask groundLayers = ~0;

    private Rigidbody2D rb;
    private Collider2D col;
    private float horizontalInput;

    // Runtime timers for jump responsiveness.
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Coyote time: refresh while grounded, count down while airborne.
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer: remember jump press briefly to allow cleaner land-then-jump timing.
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Execute jump when both timing windows overlap.
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Variable jump height: releasing jump early cuts upward velocity for a shorter hop.
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpReleaseVelocityMultiplier);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        return col.IsTouchingLayers(groundLayers);
    }
}
