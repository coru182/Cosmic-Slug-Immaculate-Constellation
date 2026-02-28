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

    [Tooltip("Size of the box used to check for ground under the player.")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.2f);

    [Tooltip("Offset from the player's collider center to place the ground check box.")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.6f);

    [Tooltip("Small horizontal cast distance used to detect walls while moving left/right.")]
    [SerializeField, Min(0.001f)] private float wallCheckDistance = 0.05f;

    [Tooltip("Maximum downward speed while airborne and pressing into a wall. Set <= 0 to disable wall sliding.")]
    [SerializeField] private float maxFallSpeedWhenWallSliding = 3f;

    [Tooltip("Only upward velocities at or below this value are clamped for corner-climb prevention.")]
    [SerializeField, Min(0f)] private float cornerClimbUpwardClampThreshold = 0.5f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator animator;
    private float horizontalInput;
    private bool hasSpeedParameter;
    private bool hasGroundedParameter;
    private bool animatorDebugLogged;

    // Runtime timers for jump responsiveness.
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Input intent collected in Update and consumed in FixedUpdate.
    private bool jumpCutRequested;

    // Cast allocations reused each frame to avoid GC spikes.
    private readonly Collider2D[] groundOverlaps = new Collider2D[4];
    private readonly RaycastHit2D[] wallHits = new RaycastHit2D[4];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>(true);
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        LogAnimatorDebugInfoOnce();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("Grounded", IsGrounded());
        }

		// Coyote time: refresh while grounded, count down while airborne.
	if (IsGrounded())
	{
		coyoteTimeCounter = coyoteTime;
	}
	else
	{
		coyoteTimeCounter = Mathf.Max(0f, coyoteTimeCounter - Time.deltaTime);
	}
        // Jump buffer using Space directly (bypasses Input Manager issues)
		if (Input.GetKeyDown(KeyCode.Space))
		{
			jumpBufferCounter = jumpBufferTime;
		}
		else
		{
			jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - Time.deltaTime);
	}

		// Variable jump height (release Space early for shorter hop)
		if (Input.GetKeyUp(KeyCode.Space))
	{
		jumpCutRequested = true;
	}
    }

    private void LogAnimatorDebugInfoOnce()
    {
        if (animatorDebugLogged)
        {
            return;
        }

        animatorDebugLogged = true;

        if (animator == null)
        {
            Debug.Log("[PlayerController] Animator GameObject: NULL | Controller: NULL | Has Speed: False | Has Grounded: False", this);
            return;
        }

        AnimatorControllerParameter[] parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            string parameterName = parameters[i].name;
            if (parameterName == "Speed")
            {
                hasSpeedParameter = true;
            }
            else if (parameterName == "Grounded")
            {
                hasGroundedParameter = true;
            }
        }

        string controllerName = animator.runtimeAnimatorController != null
            ? animator.runtimeAnimatorController.name
            : "NULL";

        Debug.Log($"[PlayerController] Animator GameObject: {animator.gameObject.name} | Controller: {controllerName} | Has Speed: {hasSpeedParameter} | Has Grounded: {hasGroundedParameter}", this);
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;
        bool isGrounded = IsGrounded();
        bool hasWallOnLeft;
        bool hasWallOnRight;
        DetectWalls(out hasWallOnLeft, out hasWallOnRight);

        bool isPressingIntoWall = (horizontalInput > 0f && hasWallOnRight) || (horizontalInput < 0f && hasWallOnLeft);
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

        // Prevent "corner climbing": when airborne and pressing into a wall/corner, don't allow contact resolution
        // to push the capsule upward unless a jump was executed this frame.
        if (!isGrounded && isPressingIntoWall && !jumpExecutedThisFrame && velocity.y > 0f && velocity.y <= cornerClimbUpwardClampThreshold)
        {
            velocity.y = 0f;
        }

        // Optional wall-slide cap: while airborne and pressing into a wall, limit downward speed for smoother falls.
        if (!isGrounded && isPressingIntoWall && !jumpExecutedThisFrame && maxFallSpeedWhenWallSliding > 0f)
        {
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeedWhenWallSliding);
        }

        rb.linearVelocity = velocity;

        jumpCutRequested = false;
    }

    private bool IsGrounded()
    {
        Vector2 checkCenter = (Vector2)col.bounds.center + groundCheckOffset;
        int hitCount = Physics2D.OverlapBoxNonAlloc(checkCenter, groundCheckSize, 0f, groundOverlaps, groundLayers);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = groundOverlaps[i];
            if (hit != null && hit != col)
            {
                return true;
            }
        }

        return false;
    }

    private void DetectWalls(out bool hasWallOnLeft, out bool hasWallOnRight)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayers);
        filter.useLayerMask = true;
        filter.useTriggers = false;

        hasWallOnLeft = HasWallHit(Vector2.left, filter);
        hasWallOnRight = HasWallHit(Vector2.right, filter);
    }

    private bool HasWallHit(Vector2 castDirection, ContactFilter2D filter)
    {
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
	
	private void OnDrawGizmosSelected()
	{
		if (!TryGetComponent<Collider2D>(out var c)) return;
		Vector2 center = (Vector2)c.bounds.center + groundCheckOffset;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(center, groundCheckSize);
	}
}
