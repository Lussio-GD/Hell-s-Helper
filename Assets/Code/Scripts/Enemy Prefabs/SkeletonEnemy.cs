using UnityEngine;
using System.Collections;

public class SkeletonEnemy : MonoBehaviour
{
    [Header("Skeleton Settings")]
    public float moveSpeed = 1.5f;
    public float detectionRange = 4f;
    public float attackRange = 1.2f;
    public float attackCooldown = 2.5f;
    public int attackDamage = 10;
    public int maxHealth = 40;

    [Header("Attack Hitbox Settings")]
    public Vector2 hitboxSize = new Vector2(1.5f, 2f);
    public float hitboxOffsetX = 0.5f;
    public Color hitboxGizmoColor = new Color(1f, 0f, 0f, 0.3f);

    [Header("Components")]
    public Transform player;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Flash Settings")]
    public Color hitFlashColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip footstepSound;
    public AudioClip attackSound;

    [Header("Footstep Settings")]
    public float footstepInterval = 0.6f;

    private Rigidbody2D rb;
    private int currentHealth;
    private bool canAttack = true;
    private bool isDead = false;
    private bool isAttacking = false;
    private Vector2 movement;
    private Color originalColor;
    private bool wasWalking = false;
    private float footstepTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0.8f;
            audioSource.volume = 0.7f;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        gameObject.tag = "Enemy";
    }

    void Update()
    {
        if (isDead) return;

        HandleAI();
        UpdateAnimations();
        HandleFootstepSounds();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        HandleMovement();
    }

    void HandleAI()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            movement = direction;

            if (player.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

            if (distanceToPlayer <= attackRange && canAttack && !isAttacking)
            {
                movement = Vector2.zero;
                Attack();
            }
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    void HandleMovement()
    {
        if (isAttacking) return;
        rb.linearVelocity = movement * moveSpeed;
    }

    void HandleFootstepSounds()
    {
        bool isWalking = movement.magnitude > 0.1f && !isAttacking && !isDead;

        if (isWalking)
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }

            wasWalking = true;
        }
        else
        {
            if (wasWalking)
            {
                footstepTimer = 0f;
                wasWalking = false;
            }
        }
    }

    void PlayFootstepSound()
    {
        if (audioSource != null && footstepSound != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(footstepSound);
            audioSource.pitch = 1f;
        }
    }

    void Attack()
    {
        canAttack = false;
        isAttacking = true;

        PlayAttackSound();
        StartCoroutine(DamagePlayerAfterDelay(0.5f));
    }

    void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(attackSound);
            audioSource.pitch = 1f;
        }
    }

    IEnumerator DamagePlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player != null && IsPlayerInHitbox())
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsDead())
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        isAttacking = false;
        StartCoroutine(ResetAttackCooldown());
    }

    bool IsPlayerInHitbox()
    {
        if (player == null) return false;

        Vector2 hitboxPosition = transform.position;
        float direction = spriteRenderer.flipX ? -1f : 1f;
        hitboxPosition.x += hitboxOffsetX * direction;

        Vector2 halfSize = hitboxSize * 0.5f;
        Vector2 min = hitboxPosition - halfSize;
        Vector2 max = hitboxPosition + halfSize;

        Vector2 playerPos = player.position;
        return (playerPos.x >= min.x && playerPos.x <= max.x &&
                playerPos.y >= min.y && playerPos.y <= max.y);
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        bool isWalking = movement.magnitude > 0.1f && !isAttacking && !isDead;
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsAttacking", isAttacking);

        if (isWalking)
        {
            animator.SetFloat("MoveX", Mathf.Abs(movement.x));
            animator.SetFloat("MoveY", movement.y);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }

        StartCoroutine(FlashColor(hitFlashColor));

        if (currentHealth <= 0)
        {
            Die();
        }
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

    void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        rb.linearVelocity = Vector2.zero;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        NotifyGameManager();

        StartCoroutine(DeathSequence());
    }

    void NotifyGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyKilled();
        }
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void SetAudioClips(AudioClip damage, AudioClip death, AudioClip footstep, AudioClip attack)
    {
        damageSound = damage;
        deathSound = death;
        footstepSound = footstep;
        attackSound = attack;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (spriteRenderer != null)
        {
            Vector2 hitboxPosition = transform.position;
            float direction = spriteRenderer.flipX ? -1f : 1f;
            hitboxPosition.x += hitboxOffsetX * direction;

            Gizmos.color = hitboxGizmoColor;
            Gizmos.DrawCube(hitboxPosition, hitboxSize);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(hitboxPosition, hitboxSize);
        }
    }
    
    void OnDrawGizmos()
    {
        if (spriteRenderer != null)
        {
            Vector2 hitboxPosition = transform.position;
            float direction = spriteRenderer.flipX ? -1f : 1f;
            hitboxPosition.x += hitboxOffsetX * direction;

            Gizmos.color = new Color(hitboxGizmoColor.r, hitboxGizmoColor.g, hitboxGizmoColor.b, 0.1f);
            Gizmos.DrawCube(hitboxPosition, hitboxSize);
        }
    }
}