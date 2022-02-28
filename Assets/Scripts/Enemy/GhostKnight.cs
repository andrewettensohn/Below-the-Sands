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

    private new Rigidbody2D rigidbody;
    private Animator animator;
    private new CapsuleCollider2D collider;
    private bool isGrounded;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        combatBehavior = GetComponent<GhostKnightCombatBehavior>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        CheckIfGrounded();
        combatBehavior.HandleCombat();
        AnimateMovement();
    }

    private RaycastHit2D GetPlayerHit(float direction)
    {
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, new Vector2(direction, 0.0f), detectionRange, playerLayer);
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

        if (health <= 0)
        {
            animator.SetTrigger("Die");
            collider.isTrigger = true;
            rigidbody.gravityScale = 0;
            Invoke(nameof(OnDisable), 1.3f);
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
