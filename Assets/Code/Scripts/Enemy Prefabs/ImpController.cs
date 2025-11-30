using UnityEngine;
using System.Collections;

public class ImpController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;

    [Header("Combat")]
    public int maxHealth = 3;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public GameObject lavaStreamPrefab;

    [Header("Melee Attack")]
    public float meleeRange = 1.5f;
    public int meleeDamage = 2;
    public float knockbackForce = 5f;

    [Header("Lava Stream Attack")]
    public float lavaStreamWarningTime = 0.5f;

    // Private variables
    private int currentHealth;
    private int hitCount = 0;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool canAttack = true;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return;

        FlipSprite();
        CheckPlayerDistance();
    }

    void FlipSprite()
    {
        // Flip sprite based on player position
        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Player is in range - stop moving and attack
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsWalking", false);

            if (canAttack)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            // Player is out of range - move toward player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            animator.SetBool("IsWalking", true);
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        // Check if we should do melee attack (after being hit twice)
        if (hitCount >= 2)
        {
            animator.SetTrigger("Attack2");
            yield return new WaitForSeconds(0.5f); // Wait for animation
            PerformMeleeAttack();
            hitCount = 0; // Reset hit counter
        }
        else
        {
            // Lava stream attack
            animator.SetTrigger("Attack1");

            // Show warning/telegraph before lava appears
            yield return new WaitForSeconds(lavaStreamWarningTime);

            // Spawn lava stream at player's current position
            SpawnLavaStream();
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void SpawnLavaStream()
    {
        if (lavaStreamPrefab != null)
        {
            // Spawn lava stream at player's current position
            Instantiate(lavaStreamPrefab, player.position, Quaternion.identity);
        }
    }

    void PerformMeleeAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= meleeRange)
        {
            // Damage player and knockback
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamage);
            }

            // Knockback - move away from player
            Vector2 knockbackDirection = (transform.position - player.position).normalized;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            // After melee, retreat for a moment
            StartCoroutine(RetreatFromPlayer());
        }
    }

    IEnumerator RetreatFromPlayer()
    {
        float retreatTime = 1.5f;
        float timer = 0f;

        while (timer < retreatTime)
        {
            Vector2 retreatDirection = (transform.position - player.position).normalized;
            rb.linearVelocity = retreatDirection * moveSpeed;
            animator.SetBool("IsWalking", true);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        hitCount++;

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        rb.linearVelocity = Vector2.zero;

        // Disable collisions and destroy after animation
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false; // Disable this script

        // Destroy gameobject after death animation
        Destroy(gameObject, 2f);
    }

    // Visualize ranges in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}