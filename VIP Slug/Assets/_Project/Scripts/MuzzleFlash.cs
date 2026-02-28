using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.075f;

    private void OnEnable()
    {
        Destroy(gameObject, lifetime);
    }
}
