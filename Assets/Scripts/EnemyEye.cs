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
    public new CircleCollider2D collider { get; private set; }

    private Animator animator;
    private EnemyEyeCombatBehavior enemyEyeCombatBehavior;

    private void Awake()
    {
        enemyEyeCombatBehavior = GetComponent<EnemyEyeCombatBehavior>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        enemyEyeCombatBehavior.isBehaviorEnabled = true;
    }

    private void Update()
    {
        enemyEyeCombatBehavior.HandleCombat();
        Animate();
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
