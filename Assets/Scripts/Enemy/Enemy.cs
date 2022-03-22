using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DamageableEnemy
{
    [Range(1, 50)]
    public float health;

    public Movement movement { get; private set; }
    public EnemyChaseBehavior chase { get; private set; }
    public EnemySentryBehavior sentry { get; private set; }
    public EnemyCombatBehavior combatBehavior { get; private set; }
    public new CircleCollider2D collider { get; private set; }

    public LayerMask playerLayer;
    public Transform target;
    public bool isStaggered;
    public bool canBeStaggered;
    public bool playerDetected;
    protected Animator animator;


    private void Start()
    {
        sentry.isBehaviorEnabled = true;
        chase.isBehaviorEnabled = false;
        combatBehavior.isBehaviorEnabled = true;
    }

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        chase = GetComponent<EnemyChaseBehavior>();
        sentry = GetComponent<EnemySentryBehavior>();
        combatBehavior = GetComponent<EnemyCombatBehavior>();
    }

    private void Update()
    {
        if (playerDetected)
        {
            combatBehavior.HandleCombat();
            chase.isBehaviorEnabled = true;
        }
        else
        {
            chase.isBehaviorEnabled = false;
            movement.SetDirection(Vector2.zero);
        }

        Animate();
    }

    private void OnDeath()
    {
        collider.isTrigger = true;
        movement.rigidbody.gravityScale = 0;
        Invoke(nameof(OnDisable), 1.3f);

        if (gameObject.name == "Demon")
        {
            GameManager.instance.milestones.HasCompletedThirdLayer = true;
            GameManager.instance.PlayEndGameCutscene();
        }
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

    public override void OnDeltDamage(float damage)
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
            isStaggered = canBeStaggered;
        }
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
