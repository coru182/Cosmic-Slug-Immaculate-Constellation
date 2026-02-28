using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;

    public int Facing { get; private set; } = 1;

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal > 0f)
        {
            Facing = 1;
        }
        else if (horizontal < 0f)
        {
            Facing = -1;
        }

        ApplyFacing();
    }

    private void ApplyFacing()
    {
        if (visualRoot != null)
        {
            Vector3 scale = visualRoot.localScale;
            scale.x = Mathf.Abs(scale.x) * Facing;
            visualRoot.localScale = scale;
        }
    }
}
