using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip footstepSound;
    public AudioClip dashSound;

    [Header("Footstep Settings")]
    public float footstepInterval = 0.4f;

    // Input
    private Vector2 moveInput;
    public Vector2 MoveInput { get; private set; }
    public float LastInputX { get; private set; }
    public float LastInputY { get; private set; }
    public bool IsWalking { get; private set; }
    public bool IsDashing { get; private set; }

    private bool canDash = true;
    private bool wasWalking = false;
    private float footstepTimer = 0f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Audio Source setup
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0.7f; // Some 3D effect
            audioSource.volume = 0.6f;
        }

        LastInputX = 0f;
        LastInputY = -1f;
    }

    void Update()
    {
        UpdateAnimations();
        HandleFootstepSounds();
    }

    void FixedUpdate()
    {
        if (IsDashing) return;
        HandleMovement();
    }

    // INPUT SYSTEM METHODS
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        MoveInput = moveInput;

        if (moveInput.magnitude > 0.1f)
        {
            LastInputX = moveInput.x;
            LastInputY = moveInput.y;
        }

        IsWalking = moveInput.magnitude > 0.1f && !IsDashing;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !IsDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    void HandleMovement()
    {
        if (IsDashing) return;
        rb.linearVelocity = moveInput * moveSpeed;
    }

    System.Collections.IEnumerator PerformDash()
    {
        // START DASH
        IsDashing = true;
        canDash = false;

        // Play dash sound
        if (audioSource != null && dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        // Calculate dash direction
        Vector2 dashDirection = moveInput.magnitude > 0.1f ? moveInput.normalized : new Vector2(LastInputX, LastInputY).normalized;
        rb.linearVelocity = dashDirection * dashSpeed;

        // DASH DURATION
        yield return new WaitForSeconds(dashDuration);

        // END DASH
        IsDashing = false;
        rb.linearVelocity = Vector2.zero;

        // COOLDOWN
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void HandleFootstepSounds()
    {
        if (IsWalking && !IsDashing)
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
                // Just stopped walking
                footstepTimer = 0f;
                wasWalking = false;
            }
        }
    }

    void PlayFootstepSound()
    {
        if (audioSource != null && footstepSound != null)
        {
            // Add some random pitch variation for more natural footsteps
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(footstepSound);
            // Reset pitch to default after playing
            audioSource.pitch = 1f;
        }
    }

    void UpdateAnimations()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }

        animator.SetBool("IsWalking", IsWalking);
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
        animator.SetFloat("LastInputX", LastInputX);
        animator.SetFloat("LastInputY", LastInputY);
        animator.SetBool("IsDashing", IsDashing);

        // Sprite flipping
        if (spriteRenderer != null)
        {
            if (LastInputX < 0 && !spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
            else if (LastInputX > 0 && spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    // Public method to set audio clips (useful for initialization)
    public void SetAudioClips(AudioClip footstep, AudioClip dash)
    {
        footstepSound = footstep;
        dashSound = dash;
    }
}