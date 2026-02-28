using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float fireCooldown = 0.15f;

    private float cooldownTimer;
    private float facingDirection = 1f;

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal > 0f)
        {
            facingDirection = 1f;
        }
        else if (horizontal < 0f)
        {
            facingDirection = -1f;
        }

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

        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.SetDirection(Vector2.right * facingDirection);
        cooldownTimer = fireCooldown;
    }
}
