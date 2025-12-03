using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public float invincibilityTime = 0.5f;

    [Header("Flash Colors")]
    public Color hitFlashColor = Color.red;
    public Color healFlashColor = Color.green;
    public Color deathFlashColor = Color.black;
    public float flashDuration = 0.2f;
    public int potionCount = 3;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip healSound;
    public AudioClip deathSound;

    private int currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private Rigidbody2D rb;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.spatialBlend = 0.7f;
            audioSource.volume = 0.8f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack") || collision.CompareTag("Enemy"))
        {
            SkeletonEnemy enemy = collision.GetComponent<SkeletonEnemy>();
            if (enemy != null && !isInvincible && !isDead)
            {
                TakeDamage(enemy.attackDamage);
            }
        }
    }

    public void OnPotion(InputAction.CallbackContext context)
    {
        if (context.performed && potionCount > 0 && currentHealth < maxHealth && !isDead)
        {
            UsePotion();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        StartCoroutine(FlashColor(hitFlashColor));
        StartCoroutine(ActivateInvincibility());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator ActivateInvincibility()
    {
        isInvincible = true;

        float elapsedTime = 0f;
        while (elapsedTime < invincibilityTime)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(hitFlashColor, originalColor, elapsedTime / invincibilityTime);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isInvincible = false;
    }

    IEnumerator FlashColor(Color flashColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    void UsePotion()
    {
        potionCount--;
        currentHealth = Mathf.Min(currentHealth + 30, maxHealth);

        if (audioSource != null && healSound != null)
        {
            audioSource.PlayOneShot(healSound);
        }

        StartCoroutine(FlashColor(healFlashColor));

        Debug.Log($"Used potion! Health: {currentHealth}/{maxHealth}. Potions left: {potionCount}");
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died!");

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(FlashColor(deathFlashColor));

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }

    public void SetAudioClips(AudioClip damage, AudioClip heal = null, AudioClip death = null)
    {
        damageSound = damage;
        healSound = heal;
        deathSound = death;
    }

    public int GetCurrentHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }
    public int GetPotionCount() { return potionCount; }
    public void AddPotion(int amount = 1) { potionCount += amount; }
    public bool IsDead() { return isDead; }
}