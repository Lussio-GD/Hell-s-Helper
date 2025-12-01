using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI References - DRAG THESE IN INSPECTOR")]
    public Image healthFillImage;          // The green bar that shrinks
    public Image healthFrameImage;         // Your frame/border
    public Image healthOverlayImage;       // Your decorative overlay

    [Header("Color Settings")]
    public Color fullHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public Color frameFlashColor = Color.white;

    [Header("Animation Settings")]
    public float updateSpeed = 10f;        // How fast health bar updates
    public float damageShakeAmount = 5f;   // How much it shakes when damaged
    public float damageShakeDuration = 0.2f;
    public float healPulseAmount = 1.2f;   // How much it scales when healing
    public float healPulseDuration = 0.3f;

    [Header("Player Reference")]
    public PlayerHealth playerHealth;      // Your existing PlayerHealth script

    // Private variables
    private float currentDisplayHealth = 1f;
    private Vector3 originalPosition;
    private Color originalFrameColor;
    private bool isInitialized = false;

    void Start()
    {
        InitializeHealthBar();
    }

    void InitializeHealthBar()
    {
        // Get references if not set
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        if (healthFillImage == null)
        {
            Debug.LogError("HealthFillImage not assigned! Drag the Fill image into the slot.");
            return;
        }

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found! Make sure your player has the PlayerHealth script.");
            return;
        }

        // Store original values
        originalPosition = healthFillImage.rectTransform.localPosition;

        if (healthFrameImage != null)
            originalFrameColor = healthFrameImage.color;

        // Set initial health
        currentDisplayHealth = (float)playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth();
        healthFillImage.fillAmount = currentDisplayHealth;

        isInitialized = true;
        Debug.Log("Health Bar Initialized! Player Health: " + playerHealth.GetCurrentHealth());
    }

    void Update()
    {
        if (!isInitialized || playerHealth == null || healthFillImage == null)
            return;

        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        // Calculate target health (0 to 1)
        float targetHealth = (float)playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth();

        // Smoothly move toward target health
        currentDisplayHealth = Mathf.Lerp(currentDisplayHealth, targetHealth, Time.deltaTime * updateSpeed);

        // Update the fill amount
        healthFillImage.fillAmount = currentDisplayHealth;

        // Update color based on health percentage
        UpdateHealthColor(targetHealth);
    }

    void UpdateHealthColor(float healthPercentage)
    {
        if (healthFillImage == null) return;

        // Change color based on health
        if (healthPercentage > 0.6f)
        {
            healthFillImage.color = fullHealthColor;
        }
        else if (healthPercentage > 0.3f)
        {
            healthFillImage.color = mediumHealthColor;
        }
        else
        {
            healthFillImage.color = lowHealthColor;
        }
    }

    // Call this when player takes damage (from your PlayerHealth script)
    public void OnPlayerDamaged(int damageAmount)
    {
        if (!isInitialized) return;

        // Shake effect
        StartCoroutine(ShakeHealthBar());

        // Flash frame
        StartCoroutine(FlashFrame());
    }

    // Call this when player heals
    public void OnPlayerHealed(int healAmount)
    {
        if (!isInitialized) return;

        // Pulse effect
        StartCoroutine(PulseHealthBar());
    }

    // Animation Coroutines
    private IEnumerator ShakeHealthBar()
    {
        float elapsed = 0f;

        while (elapsed < damageShakeDuration)
        {
            elapsed += Time.deltaTime;

            // Calculate shake offset
            float x = Random.Range(-1f, 1f) * damageShakeAmount;
            float y = Random.Range(-1f, 1f) * damageShakeAmount;

            // Apply shake to fill image
            healthFillImage.rectTransform.localPosition = originalPosition + new Vector3(x, y, 0);

            yield return null;
        }

        // Reset position
        healthFillImage.rectTransform.localPosition = originalPosition;
    }

    private IEnumerator FlashFrame()
    {
        if (healthFrameImage == null) yield break;

        // Flash to white
        healthFrameImage.color = frameFlashColor;

        // Wait briefly
        yield return new WaitForSeconds(0.1f);

        // Return to original color
        healthFrameImage.color = originalFrameColor;
    }

    private IEnumerator PulseHealthBar()
    {
        if (healthFillImage == null) yield break;

        RectTransform fillRect = healthFillImage.rectTransform;
        Vector3 originalScale = fillRect.localScale;
        Vector3 targetScale = originalScale * healPulseAmount;

        // Scale up
        float elapsed = 0f;
        while (elapsed < healPulseDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (healPulseDuration / 2f);
            fillRect.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Scale down
        elapsed = 0f;
        while (elapsed < healPulseDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (healPulseDuration / 2f);
            fillRect.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        // Ensure back to original
        fillRect.localScale = originalScale;
    }

    // Public method to update health bar manually
    public void ForceUpdate()
    {
        if (playerHealth != null && healthFillImage != null)
        {
            float healthPercent = (float)playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth();
            currentDisplayHealth = healthPercent;
            healthFillImage.fillAmount = healthPercent;
            UpdateHealthColor(healthPercent);
        }
    }

    // Debug method for testing
    public void TestDamage(int amount = 10)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(amount);
            OnPlayerDamaged(amount);
        }
    }

    // Simple test from keyboard
    void Update2()
    {
        if (!isInitialized) return;

        // Test with keyboard (remove in final game)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestDamage(10);
        }
    }
}