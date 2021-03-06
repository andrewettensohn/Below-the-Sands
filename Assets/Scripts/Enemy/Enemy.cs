using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : DamageableEnemy
{
    [Range(1, 50)]
    public float health;

    public float speed;

    public EnemyChaseBehavior chase { get; private set; }
    public EnemySentryBehavior sentry { get; private set; }
    public EnemyCombatBehavior combatBehavior { get; private set; }
    public EnemyWaypointBehavior enemyWaypointBehavior { get; private set; }
    public Transform attackPoint;
    public AudioClip DeathAudioClip;
    public AudioClip AttackingAudioClip;
    public AudioClip HitAudioClip;
    public AudioClip BlockAudioClip;
    public LayerMask playerLayer;
    public Transform target { get; private set; }
    public bool isStaggered;
    public bool canBeStaggered;
    public bool playerDetected;
    public AudioSource audioSource;
    public int scoreValue;
    protected Animator animator;
    public NavMeshAgent navMeshAgent { get; private set; }
    public Vector2 lookDirection;

    private void Start()
    {
        SetDefaultBehaviors();

        navMeshAgent.stoppingDistance = combatBehavior.attackRange;
        navMeshAgent.speed = speed;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        GameObject targetGameObject = GameObject.Find("Ronin");
        if (targetGameObject != null)
        {
            target = targetGameObject.transform;
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        chase = GetComponent<EnemyChaseBehavior>();
        sentry = GetComponent<EnemySentryBehavior>();
        combatBehavior = GetComponent<EnemyCombatBehavior>();
        enemyWaypointBehavior = GetComponent<EnemyWaypointBehavior>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        DetermineLookDirection();
        HandleBehaviors();
        Animate();
    }

    private void DetermineLookDirection()
    {
        if (health <= 0) return;

        lookDirection = navMeshAgent.desiredVelocity;

        if (lookDirection.x > 0.1)
        {
            transform.rotation = Quaternion.identity;
        }
        else if (lookDirection.x < -0.1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void StopMovement()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
    }

    public void AllowMovement()
    {
        navMeshAgent.isStopped = false;
    }

    public void SetDefaultBehaviors()
    {
        sentry.isBehaviorEnabled = true;
        chase.isBehaviorEnabled = false;
        combatBehavior.isBehaviorEnabled = false;
    }

    protected void HandleBehaviors()
    {
        if (health <= 0)
        {
            chase.isBehaviorEnabled = false;
            combatBehavior.isBehaviorEnabled = false;
            StopMovement();
            return;
        }

        if (enemyWaypointBehavior?.isBehaviorEnabled == true)
        {
            chase.isBehaviorEnabled = false;
            combatBehavior.isBehaviorEnabled = false;
            sentry.isBehaviorEnabled = false;
            AllowMovement();
        }
        else if (playerDetected)
        {
            combatBehavior.HandleCombat();
            chase.isBehaviorEnabled = true;
            AllowMovement();
        }
        else
        {
            chase.isBehaviorEnabled = false;
            StopMovement();
        }
    }

    protected void OnDeath()
    {
        animator.SetTrigger("Die");

        StopMovement();

        Invoke(nameof(OnDisable), 1.3f);
        GameManager.instance.UpdateScore(scoreValue);

        if (gameObject.name == "Demon")
        {
            GameManager.instance.milestones.HasCompletedThirdLayer = true;
            GameManager.instance.PlayEndGameCutscene();
        }
    }

    private void Animate()
    {
        animator.SetBool("Attacking", combatBehavior.isAttacking && !isStaggered);
        animator.SetBool("Block", combatBehavior.isBlocking);

        animator.SetFloat("Speed", navMeshAgent.velocity.sqrMagnitude);
        animator.SetFloat("Move Y", navMeshAgent.velocity.y);
        animator.SetFloat("Look X", 1);
    }

    public override void OnDeltDamage(float damage)
    {
        if (combatBehavior.isBlocking)
        {
            audioSource.PlayOneShot(BlockAudioClip);
            return;
        }

        damage = Math.Abs(damage);
        health -= damage;

        if (health <= 0)
        {
            OnDeath();
            if (!PlayerInfo.instance.isBlessed) audioSource.PlayOneShot(DeathAudioClip);
        }
        else
        {
            animator.SetTrigger("Hit");
            isStaggered = canBeStaggered;
            if (!PlayerInfo.instance.isBlessed) audioSource.PlayOneShot(HitAudioClip);
        }
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, combatBehavior.attackRange);
    }
}
