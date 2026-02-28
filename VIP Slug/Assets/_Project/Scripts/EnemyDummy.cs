using System.Collections;
using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float hitFlashDuration = 0.08f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;
    private Color originalColor = Color.white;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Bullet>() == null)
        {
            return;
        }

        TakeDamage(1);
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (spriteRenderer != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }

            flashCoroutine = StartCoroutine(HitFlash());
        }
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }
}
