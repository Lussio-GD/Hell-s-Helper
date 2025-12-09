using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI References - DRAG THESE IN INSPECTOR")]
    public Image healthFillImage;         
    public Image healthFrameImage;         
    public Image healthOverlayImage;      

    [Header("Color Settings")]
    public Color fullHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public Color frameFlashColor = Color.white;

    [Header("Animation Settings")]
    public float updateSpeed = 10f;       
    public float damageShakeAmount = 5f; 
    public float damageShakeDuration = 0.2f;
    public float healPulseAmount = 1.2f;  
    public float healPulseDuration = 0.3f;

    [Header("Player Reference")]
    public PlayerHealth playerHealth;     


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


        originalPosition = healthFillImage.rectTransform.localPosition;

        if (healthFrameImage != null)
            originalFrameColor = healthFrameImage.color;


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

        float targetHealth = (float)playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth();


        currentDisplayHealth = Mathf.Lerp(currentDisplayHealth, targetHealth, Time.deltaTime * updateSpeed);


        healthFillImage.fillAmount = currentDisplayHealth;


        UpdateHealthColor(targetHealth);
    }

    void UpdateHealthColor(float healthPercentage)
    {
        if (healthFillImage == null) return;


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


    public void OnPlayerDamaged(int damageAmount)
    {
        if (!isInitialized) return;
        

        StartCoroutine(ShakeHealthBar());


        StartCoroutine(FlashFrame());
    }


    public void OnPlayerHealed(int healAmount)
    {
        if (!isInitialized) return;


        StartCoroutine(PulseHealthBar());
    }


    private IEnumerator ShakeHealthBar()
    {
        float elapsed = 0f;

        while (elapsed < damageShakeDuration)
        {
            elapsed += Time.deltaTime;


            float x = Random.Range(-1f, 1f) * damageShakeAmount;
            float y = Random.Range(-1f, 1f) * damageShakeAmount;


            healthFillImage.rectTransform.localPosition = originalPosition + new Vector3(x, y, 0);

            yield return null;
        }


        healthFillImage.rectTransform.localPosition = originalPosition;
    }

    private IEnumerator FlashFrame()
    {
        if (healthFrameImage == null) yield break;


        healthFrameImage.color = frameFlashColor;


        yield return new WaitForSeconds(0.1f);


        healthFrameImage.color = originalFrameColor;
    }

    private IEnumerator PulseHealthBar()
    {
        if (healthFillImage == null) yield break;

        RectTransform fillRect = healthFillImage.rectTransform;
        Vector3 originalScale = fillRect.localScale;
        Vector3 targetScale = originalScale * healPulseAmount;


        float elapsed = 0f;
        while (elapsed < healPulseDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (healPulseDuration / 2f);
            fillRect.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }


        elapsed = 0f;
        while (elapsed < healPulseDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (healPulseDuration / 2f);
            fillRect.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }


        fillRect.localScale = originalScale;
    }


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


    public void TestDamage(int amount = 10)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(amount);
            OnPlayerDamaged(amount);
        }
    }


    void Update2()
    {
        if (!isInitialized) return;


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestDamage(10);
        }
    }
}