using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [Range(1, 10)]
    public float health;

    public Movement movement { get; private set; }
    public SkeletonChaseBehavior chase { get; private set; }
    public SkeletonSentryBehavior sentry { get; private set; }
    public SkeletonCombatBehavior combatBehavior { get; private set; }
    public new CircleCollider2D collider { get; private set; }

    public LayerMask playerLayer;
    public Transform target;
    public bool isStaggered;
    public bool playerDetected;
    protected Animator animator;


    private void Start()
    {
        ToggleBehaviors(false);
        combatBehavior.enabled = true;
    }

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        chase = GetComponent<SkeletonChaseBehavior>();
        sentry = GetComponent<SkeletonSentryBehavior>();
        combatBehavior = GetComponent<SkeletonCombatBehavior>();
    }

    private void Update()
    {
        combatBehavior.HandleCombat();
        Animate();
    }

    private void FixedUpdate()
    {
        if (playerDetected && !chase.isBehaviorEnabled)
        {
            ToggleBehaviors(true);
        }
        else if (!playerDetected && !sentry.isBehaviorEnabled)
        {
            ToggleBehaviors(false);
        }
    }

    private void ToggleBehaviors(bool isChaseEnabled)
    {
        sentry.isBehaviorEnabled = !isChaseEnabled;
        chase.isBehaviorEnabled = isChaseEnabled;
    }

    private void OnDeath()
    {
        collider.isTrigger = true;
        movement.rigidbody.gravityScale = 0;
        Invoke(nameof(OnDisable), 1.3f);
    }

    private void Animate()
    {
        if (health <= 0)
        {
            animator.SetTrigger("Die");
            return;
        }

        animator.SetBool("Attacking", combatBehavior.isAttacking && !isStaggered);
        animator.SetBool("Block", combatBehavior.isBlocking);

        animator.SetFloat("Speed", movement.rigidbody.velocity.sqrMagnitude);
        animator.SetFloat("Move Y", movement.rigidbody.velocity.y);
        animator.SetFloat("Look X", movement.lookDirection.x);
    }

    public void OnDeltDamage(float damage)
    {
        if (combatBehavior.isBlocking) return;

        damage = Math.Abs(damage);
        health -= damage;

        if (health <= 0)
        {
            OnDeath();
        }
        else
        {
            animator.SetTrigger("Hit");
            isStaggered = true;
        }
    }

    public RaycastHit2D GetPlayerHit(float distance)
    {
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, movement.lookDirection, distance, playerLayer);
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
