using UnityEngine;
using System.Collections;

public class LavaStreamController : MonoBehaviour
{
    [Header("Lava Stream Settings")]
    public float damageInterval = 0.5f;
    public int damagePerTick = 1;
    public float lifetime = 3f;

    private Animator animator;
    private bool isActive = false;
    private float damageTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(LavaStreamLifecycle());
    }

    IEnumerator LavaStreamLifecycle()
    {
        // Play appear animation
        animator.Play("LavaStream_Appear");

        // Wait for appear animation to finish
        yield return new WaitForSeconds(0.5f);

        // Set to active state
        isActive = true;
        animator.Play("LavaStream_Idle");

        // Wait for main duration
        yield return new WaitForSeconds(lifetime - 1f);

        // Play disappear animation
        isActive = false;
        animator.Play("LavaStream_Disappear");

        // Wait for disappear animation then destroy
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    void Update()
    {
        if (isActive)
        {
            damageTimer += Time.deltaTime;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isActive && other.CompareTag("Player") && damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            // Damage player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerTick);
            }
        }
    }
}