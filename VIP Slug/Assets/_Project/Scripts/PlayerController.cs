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

    [Tooltip("Small downward cast distance used to verify grounded state from below and avoid wall/corner contacts.")]
    [SerializeField, Min(0.001f)] private float groundCheckDistance = 0.05f;

    [Tooltip("Small horizontal cast distance used to detect walls while moving left/right.")]
    [SerializeField, Min(0.001f)] private float wallCheckDistance = 0.05f;

    private Rigidbody2D rb;
    private Collider2D col;
    private float horizontalInput;

    // Runtime timers for jump responsiveness.
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Input intent collected in Update and consumed in FixedUpdate.
    private bool jumpCutRequested;

    // Cast allocations reused each frame to avoid GC spikes.
    private readonly RaycastHit2D[] groundHits = new RaycastHit2D[4];
    private readonly RaycastHit2D[] wallHits = new RaycastHit2D[4];

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

        // Variable jump height request: applied in FixedUpdate.
        if (Input.GetButtonUp("Jump"))
        {
            jumpCutRequested = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        bool isGrounded = IsGrounded();
        bool isPressingIntoWall = IsPressingIntoWall();
        bool jumpExecutedThisFrame = false;

        // Horizontal movement remains velocity-based, but wall presses are zeroed out.
        velocity.x = horizontalInput * moveSpeed;
        if (isPressingIntoWall)
        {
            // Prevent continuously pushing into a wall/corner while input is held toward it.
            velocity.x = 0f;
        }

        // Execute jump when buffered input overlaps coyote-time window.
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            velocity.y = jumpForce;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            jumpExecutedThisFrame = true;
        }

        // Variable jump height: releasing jump early cuts upward velocity for a shorter hop.
        if (jumpCutRequested && velocity.y > 0f)
        {
            velocity.y *= jumpReleaseVelocityMultiplier;
        }

        // Corner-climb prevention: when airborne and pressing into a wall, discard upward velocity
        // created by contact resolution unless this frame's upward velocity came from an actual jump.
        if (!isGrounded && isPressingIntoWall && !jumpExecutedThisFrame && velocity.y > 0f)
        {
            velocity.y = 0f;
        }

        rb.velocity = velocity;
        jumpCutRequested = false;
    }

    private bool IsGrounded()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayers);
        filter.useLayerMask = true;
        filter.useTriggers = false;

        int hitCount = col.Cast(Vector2.down, filter, groundHits, groundCheckDistance);
        for (int i = 0; i < hitCount; i++)
        {
            if (groundHits[i].normal.y > 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPressingIntoWall()
    {
        if (Mathf.Abs(horizontalInput) < 0.01f)
        {
            return false;
        }

        Vector2 castDirection = horizontalInput > 0f ? Vector2.right : Vector2.left;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayers);
        filter.useLayerMask = true;
        filter.useTriggers = false;

        int hitCount = col.Cast(castDirection, filter, wallHits, wallCheckDistance);
        for (int i = 0; i < hitCount; i++)
        {
            // A wall hit should have a normal mostly opposite our cast direction.
            if (Vector2.Dot(wallHits[i].normal, -castDirection) > 0.1f)
            {
                return true;
            }
        }

        return false;
    }
}
