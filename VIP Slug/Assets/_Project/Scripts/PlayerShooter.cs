using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private MuzzleFlash muzzleFlashPrefab;
    [SerializeField] private float fireCooldown = 0.15f;

    private float cooldownTimer;
    private PlayerVisual playerVisual;
    private Animator animator;

    private void Awake()
    {
        playerVisual = GetComponent<PlayerVisual>();
        animator = GetComponentInChildren<Animator>(true);
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Z) && cooldownTimer <= 0f)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            return;
        }

        int facingDirection = playerVisual != null ? playerVisual.Facing : 1;
        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.SetDirection(new Vector2(facingDirection, 0f));

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
