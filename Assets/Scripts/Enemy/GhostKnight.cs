using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GhostKnight : DamageableEnemy
{
    [Range(1, 10)]
    public float health;

    [Range(1, 20)]
    public float detectionRange;

    public GhostKnightCombatBehavior combatBehavior { get; private set; }
    public AIPath aiPath;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public bool isStaggered;
    public AudioClip DeathAudioClip;
    public AudioClip AttackingAudioClip;
    public AudioClip HitAudioClip;
    public AudioSource audioSource;
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private new CircleCollider2D collider;
    private bool isGrounded;
    private bool isPlayerDetected;
    private bool isDying;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        combatBehavior = GetComponent<GhostKnightCombatBehavior>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        aiPath.isStopped = true;
    }

    private void Update()
    {
        if (!isPlayerDetected)
        {
            Sentry();
        }

        CheckIfGrounded();
        combatBehavior.HandleCombat();
        AnimateMovement();
    }

    private void Sentry()
    {
        HandleSentryForDirection(0.1f);
        HandleSentryForDirection(-0.1f);
    }

    private void HandleSentryForDirection(float direction)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 10.0f, aiPath.desiredVelocity, combatBehavior.attackRange, playerLayer);

        if (hit.collider != null)
        {
            isPlayerDetected = true;
            aiPath.isStopped = false;
        }
    }

    private void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, obstacleLayer);

        isGrounded = hit.collider != null;
    }

    private void AnimateMovement()
    {
        animator.SetBool("Attacking", combatBehavior.isAttacking);
        animator.SetFloat("Speed", aiPath.desiredVelocity.sqrMagnitude);
        animator.SetFloat("Move Y", aiPath.desiredVelocity.y);
        animator.SetFloat("Look X", aiPath.desiredVelocity.x);
        animator.SetBool("Is Grounded", isGrounded);
    }

    public override void OnDeltDamage(float damage)
    {
        damage = Mathf.Abs(damage);

        health -= damage;

        if (health <= 0 && !isDying)
        {
            isDying = true;
            audioSource.PlayOneShot(DeathAudioClip);
            animator.SetTrigger("Die");
            collider.isTrigger = true;
            rigidbody.gravityScale = 0;
            Invoke(nameof(OnDisable), 1.3f);
        }
        else
        {
            audioSource.PlayOneShot(HitAudioClip);
            animator.SetTrigger("Hit");
        }
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
