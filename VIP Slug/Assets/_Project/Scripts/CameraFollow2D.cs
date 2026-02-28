using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private BoxCollider2D levelBounds;
    [SerializeField, Min(0f)] private float followSpeed = 5f;
    [SerializeField] private Vector2 offset;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        AutoAssignReferences();
    }

    private void LateUpdate()
    {
        AutoAssignReferences();

        if (player == null || levelBounds == null || cam == null)
        {
            return;
        }

        Vector2 targetPosition = (Vector2)player.position + offset;
        Vector2 currentPosition = transform.position;
        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        Vector2 smoothedPosition = Vector2.Lerp(currentPosition, targetPosition, t);

        Bounds bounds = levelBounds.bounds;
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = bounds.min.x + halfWidth;
        float maxX = bounds.max.x - halfWidth;
        float minY = bounds.min.y + halfHeight;
        float maxY = bounds.max.y - halfHeight;

        if (minX > maxX)
        {
            smoothedPosition.x = bounds.center.x;
        }
        else
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
        }

        if (minY > maxY)
        {
            smoothedPosition.y = bounds.center.y;
        }
        else
        {
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }

    private void AutoAssignReferences()
    {
        if (player == null)
        {
            GameObject namedPlayer = GameObject.Find("Player");
            if (namedPlayer != null)
            {
                player = namedPlayer.transform;
            }
            else
            {
                PlayerController playerController = FindFirstObjectByType<PlayerController>();
                if (playerController != null)
                {
                    player = playerController.transform;
                }
            }
        }

        if (levelBounds == null)
        {
            GameObject boundsObject = GameObject.Find("LevelBounds");
            if (boundsObject != null)
            {
                levelBounds = boundsObject.GetComponent<BoxCollider2D>();
            }
        }
    }
}
