using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform patrolA;
    [SerializeField] private Transform patrolB;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Combat")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float shootInterval = 0.8f;
    [SerializeField] private LayerMask lineOfSightMask;

    private bool movingToB = true;
    private float shootTimer;

    private void Update()
    {
        if (player == null)
        {
            Patrol();
            return;
        }

        if (CanDetectPlayer())
        {
            FaceDirection(player.position - transform.position);
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                ShootAtPlayer();
                shootTimer = shootInterval;
            }
        }
        else
        {
            Patrol();
        }
    }

    private bool CanDetectPlayer()
    {
        Vector2 toPlayer = player.position - transform.position;
        if (toPlayer.sqrMagnitude > detectionRange * detectionRange)
        {
            return false;
        }

        if (lineOfSightMask.value == 0)
        {
            return true;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, detectionRange, lineOfSightMask);
        if (hit.collider == null)
        {
            return false;
        }

        Transform hitTransform = hit.collider.transform;
        return hitTransform == player || hitTransform.IsChildOf(player);
    }

    private void Patrol()
    {
        Transform target = movingToB ? patrolB : patrolA;
        if (target == null)
        {
            return;
        }

        Vector3 nextPosition = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        Vector3 moveDelta = nextPosition - transform.position;
        transform.position = nextPosition;
        FaceDirection(moveDelta);

        if ((transform.position - target.position).sqrMagnitude < 0.01f)
        {
            movingToB = !movingToB;
        }
    }

    private void ShootAtPlayer()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)(player.position - firePoint.position)).normalized;
        if (direction.sqrMagnitude <= 0f)
        {
            direction = transform.localScale.x >= 0f ? Vector2.right : Vector2.left;
        }

        Bullet spawnedBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        spawnedBullet.SetDirection(direction);
        IgnoreEnemyCollision(spawnedBullet);
    }

    private void IgnoreEnemyCollision(Bullet spawnedBullet)
    {
        Collider2D bulletCollider = spawnedBullet.GetComponent<Collider2D>();
        if (bulletCollider == null)
        {
            return;
        }

        Collider2D[] enemyColliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            if (enemyColliders[i] != null)
            {
                Physics2D.IgnoreCollision(enemyColliders[i], bulletCollider, true);
            }
        }
    }

    private void FaceDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) < 0.001f)
        {
            return;
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
