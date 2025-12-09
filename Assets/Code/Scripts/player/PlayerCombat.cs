using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackCooldown = 0.5f;
    public float attackPushForce = 8f;
    public int attackDamage = 20;
    public float attackRange = 0.8f;
    public LayerMask enemyLayers;

    [Header("References")]
    public Transform attackPoint;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip bowAttackSound;

    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isAttacking = false;
    private bool isShooting = false;
    private bool canAttack = true;
    private float lastAttackTime = 0f;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();


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

        Debug.Log("PlayerCombat initialized. Enemy layers: " + enemyLayers.value);
    }

    void Update()
    {
        HandleAttack();
        UpdateCombatAnimations();
    }

    void HandleAttack()
    {
        if (canAttack)
        {
            if (Input.GetMouseButtonDown(0) && !isAttacking && !isShooting && !playerMovement.IsDashing)
            {
                Debug.Log("Mouse click detected - starting attack");
                StartCoroutine(PerformAttack());
            }

            if (Input.GetKeyDown(KeyCode.F) && !isAttacking && !isShooting && !playerMovement.IsDashing)
            {
                Debug.Log("F key detected - starting bow attack");
                StartCoroutine(PerformBowAttack());
            }
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        canAttack = false;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Attack started");

        // Play attack sound
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Wait for animation to start
        yield return new WaitForSeconds(0.1f);

        ExecuteAttack();
        yield return new WaitForSeconds(0.4f);

        isAttacking = false;

        // Start cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        lastAttackTime = Time.time;
        Debug.Log("Attack completed");
    }

    IEnumerator PerformBowAttack()
    {
        isShooting = true;
        canAttack = false;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Bow attack!");

        
        if (audioSource != null && bowAttackSound != null)
        {
            audioSource.PlayOneShot(bowAttackSound);
        }

        yield return new WaitForSeconds(0.5f);
        isShooting = false;

        // Start cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        lastAttackTime = Time.time;
    }

    void ExecuteAttack()
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is null!");
            return;
        }

        UpdateAttackPointPosition();

        Debug.Log($"Checking for enemies at position: {attackPoint.position}, range: {attackRange}");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        Debug.Log($"Found {hitEnemies.Length} colliders in attack range");

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"Hit: {enemy.gameObject.name} with layer: {enemy.gameObject.layer}");

            // Push enemy
            Vector2 pushDirection = (enemy.transform.position - transform.position).normalized;
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(pushDirection * attackPushForce, ForceMode2D.Impulse);
                Debug.Log("Pushed enemy");
            }

            // Damage enemy
            SkeletonEnemy enemyScript = enemy.GetComponent<SkeletonEnemy>();
            if (enemyScript != null)
            {
                Debug.Log("SkeletonEnemy component found - dealing damage");
                enemyScript.TakeDamage(attackDamage);
            }
            else
            {
                Debug.Log("No SkeletonEnemy component found on: " + enemy.gameObject.name);
            }
        }
    }

    void UpdateAttackPointPosition()
    {
        if (attackPoint == null) return;
        float offset = 0.7f;
        Vector2 attackDirection = new Vector2(playerMovement.LastInputX, playerMovement.LastInputY).normalized;
        attackPoint.localPosition = attackDirection * offset;
        Debug.Log($"Attack point positioned at: {attackPoint.localPosition}");
    }

    void UpdateCombatAnimations()
    {
        if (animator == null) return;
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsShooting", isShooting);
    }

    
    public void SetAudioClips(AudioClip attack, AudioClip bowAttack = null)
    {
        attackSound = attack;
        bowAttackSound = bowAttack;
    }

    
    public bool IsAttacking() => isAttacking;
    public bool IsShooting() => isShooting;
    public bool CanAttack() => canAttack;

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}