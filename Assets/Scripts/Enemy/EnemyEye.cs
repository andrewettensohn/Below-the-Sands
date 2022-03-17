using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyEye : DamageableEnemy
{
    [Range(1, 10)]
    public float health;

    public AIPath aiPath;
    public LayerMask playerLayer;
    public bool isStaggered;
    public new CapsuleCollider2D collider { get; private set; }

    private Animator animator;
    private EnemyEyeCombatBehavior enemyEyeCombatBehavior;
    private bool isPlayerDetected;

    private void Awake()
    {
        enemyEyeCombatBehavior = GetComponent<EnemyEyeCombatBehavior>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        enemyEyeCombatBehavior.isBehaviorEnabled = true;
        aiPath.isStopped = true;
    }

    private void Update()
    {
        if (!isPlayerDetected)
        {
            Sentry();
        }
        else
        {
            enemyEyeCombatBehavior.HandleCombat();
        }

        Animate();
    }

    private void Sentry()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 10.0f, aiPath.desiredVelocity, enemyEyeCombatBehavior.attackRange, playerLayer);

        if (hit.collider != null)
        {
            isPlayerDetected = true;
            aiPath.isStopped = false;
        }
    }

    private void Animate()
    {
        if (health <= 0)
        {
            animator.SetTrigger("Die");
            OnDeath();
            return;
        }

        animator.SetBool("Is Attacking", enemyEyeCombatBehavior.isAttacking);
        animator.SetFloat("Look X", aiPath.desiredVelocity.x);
    }

    private void OnDeath()
    {
        collider.isTrigger = true;
        Invoke(nameof(OnDisable), 1.0f);
    }

    public override void OnDeltDamage(float damage)
    {
        damage = Mathf.Abs(damage);
        health -= damage;
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
