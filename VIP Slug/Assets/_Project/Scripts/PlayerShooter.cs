using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private MuzzleFlash muzzleFlashPrefab;
    [SerializeField] private float fireCooldown = 0.15f;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 firePointForwardLocal = new Vector2(0.8f, 0f);
    [SerializeField] private Vector2 firePointUpLocal = new Vector2(0f, 0.85f);
    [SerializeField] private Vector2 firePointDiagUpLocal = new Vector2(0.65f, 0.6f);

    private const float AxisDeadZone = 0.01f;

    private float cooldownTimer;
    private PlayerVisual playerVisual;
    private Vector2 aimDirection = Vector2.right;
    private bool isAimUp;
    private bool isAimDiagUp;

    private void Awake()
    {
        playerVisual = GetComponent<PlayerVisual>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>(true);
            Debug.LogWarning("PlayerShooter animator reference was not assigned. Falling back to GetComponentInChildren<Animator>().", this);
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        UpdateAimState();
        UpdateAnimatorAim();

        if (Input.GetKeyDown(KeyCode.Z) && cooldownTimer <= 0f)
        {
            Fire();
        }
    }

    private void UpdateAimState()
    {
        int facingDirection = playerVisual != null ? playerVisual.Facing : 1;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        aimDirection = Vector2.right * facingDirection;
        isAimUp = false;
        isAimDiagUp = false;

        bool isVerticalUp = vertical > 0f;
        bool isHorizontalNeutral = Mathf.Abs(horizontal) <= AxisDeadZone;
        bool isHorizontalFacing = horizontal * facingDirection > AxisDeadZone;

        if (isVerticalUp)
        {
            if (isHorizontalNeutral)
            {
                aimDirection = Vector2.up;
                isAimUp = true;
            }
            else if (isHorizontalFacing)
            {
                aimDirection = new Vector2(facingDirection, 1f).normalized;
                isAimDiagUp = true;
            }
        }

        UpdateFirePointPosition(facingDirection);
    }

    private void UpdateFirePointPosition(int facingDirection)
    {
        if (firePoint == null)
        {
            return;
        }

        Vector2 localPosition = firePointForwardLocal;
        if (isAimUp)
        {
            localPosition = firePointUpLocal;
        }
        else if (isAimDiagUp)
        {
            localPosition = firePointDiagUpLocal;
        }

        localPosition.x *= facingDirection;
        firePoint.localPosition = localPosition;
    }

    private void UpdateAnimatorAim()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("AimUp", isAimUp);
        animator.SetBool("AimDiagUp", isAimDiagUp);
    }

    private void Fire()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            return;
        }

        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.SetDirection(aimDirection);

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        if (muzzleFlashPrefab != null)
        {
            Instantiate(muzzleFlashPrefab, firePoint.position, Quaternion.identity);
        }

        cooldownTimer = fireCooldown;
    }
}
