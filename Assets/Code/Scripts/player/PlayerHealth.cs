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

        // Store original color
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Audio Source setup
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

    
    public void OnPotion(InputAction.CallbackContext context)
    {
        if (context.performed && potionCount > 0 && currentHealth < maxHealth && !isDead)
        {
            UsePotion();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible || playerMovement.IsDashing) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Play damage sound
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Flash red when hit
        StartCoroutine(FlashColor(hitFlashColor));
        StartCoroutine(InvincibilityFrames());

        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashColor(Color flashColor)
    {
        if (spriteRenderer != null)
        {
            // Flash to the specified color
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            // Return to original color
            spriteRenderer.color = originalColor;
        }
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        // Blinking effect during invincibility
        float timer = 0f;
        while (timer < invincibilityTime)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        // Ensure sprite is visible and original color after invincibility
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }
        isInvincible = false;
    }

    void UsePotion()
    {
        potionCount--;
        currentHealth = Mathf.Min(currentHealth + 30, maxHealth);

        // Play heal sound
        if (audioSource != null && healSound != null)
        {
            audioSource.PlayOneShot(healSound);
        }

        // Flash green when healing
        StartCoroutine(FlashColor(healFlashColor));

        Debug.Log($"Used potion! Health: {currentHealth}/{maxHealth}. Potions left: {potionCount}");
    }

    void Die()
    {
        isDead = true;

        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Flash black and hide on death
        StartCoroutine(DeathSequence());

        Debug.Log("Player died!");
    }

    IEnumerator DeathSequence()
    {
        // Flash black
        if (spriteRenderer != null)
        {
            spriteRenderer.color = deathFlashColor;
        }

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Wait a moment
        yield return new WaitForSeconds(0.5f);

        // Hide sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Disable other components
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }

        // Wait for respawn
        yield return new WaitForSeconds(2.5f);

        Respawn();
    }

    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;

        // Show sprite and reset color
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }

        // Re-enable components
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        if (playerCombat != null)
        {
            playerCombat.enabled = true;
        }

        Debug.Log("Player respawned!");
    }

    // Public method to set audio clips
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