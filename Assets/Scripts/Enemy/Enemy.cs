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
    public AudioClip SecondaryAttackAudioClip;
    public AudioClip HitAudioClip;
    public AudioClip BlockAudioClip;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public LayerMask nodeLayer;
    public Transform target { get; protected set; }
    public bool isStaggered;
    public float staggerTime;
    public bool canBeStaggered;
    public bool isPlayerDetected;
    public AudioSource audioSource;
    public int scoreValue;
    public Animator animator;
    public NavMeshAgent navMeshAgent { get; private set; }
    public Vector2 lookDirection;
    public int focusPointReward;
    public float debugAttackRange;
    public float debugStoppingDistance;
    public float detectionSizeModifier = 0.75f;
    public float shootingRange;
    public bool canFly;

    protected bool isStaggerTimerActive;
    public bool isArcher;
    
    protected bool isDying;
    public bool isCustomBeahviorRunning;

    protected virtual void Start()
    {
        SetDefaultBehaviors();

        float stoppingDistance = isArcher ? shootingRange : combatBehavior.attackRange;

        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.speed = speed;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        GameObject targetGameObject = GameObject.Find(GameManager.instance.playerCharacterName);
        if (targetGameObject != null)
        {
            target = targetGameObject.transform;
        }
    }

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        chase = GetComponent<EnemyChaseBehavior>();
        sentry = GetComponent<EnemySentryBehavior>();
        combatBehavior = GetComponent<EnemyCombatBehavior>();
        enemyWaypointBehavior = GetComponent<EnemyWaypointBehavior>();
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        if(isStaggered && !isStaggerTimerActive)
        {
            isStaggerTimerActive = true;
            StartCoroutine(HandleStaggerTimer());
        }

        if(navMeshAgent.destination == target.position)
        {
            SetDestinationToPlayer();
        }

        DetermineLookDirection();
        Animate();
    }

    protected virtual void DetermineLookDirection()
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

    public void SetDestinationToPlayer()
    {
        navMeshAgent.SetDestination(target.position);
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

    public void DisableAllBehaviors()
    {
        chase.isBehaviorEnabled = false;
        combatBehavior.isBehaviorEnabled = false;
        sentry.isBehaviorEnabled = false;
    }

    protected virtual IEnumerator HandleStaggerTimer()
    {
        yield return new WaitForSeconds(staggerTime);

        isStaggered = false;
        isStaggerTimerActive = false; 
    }

    protected virtual void OnDeath()
    {
        isDying = true;
        animator.SetTrigger("Die");
        audioSource.PlayOneShot(DeathAudioClip);
        GivePlayerRewardForDeath();
        StopMovement();
        DisableAllBehaviors();

        Invoke(nameof(OnDisable), 1.3f);

        if (gameObject.name == "Demon")
        {
            GameManager.instance.milestones.HasCompletedThirdLayer = true;
            GameManager.instance.PlayEndGameCutscene();
        }
    }

    protected virtual void GivePlayerRewardForDeath()
    {
        GameManager.instance.UpdateScore(scoreValue);
    }

    protected void Animate()
    {
        animator.SetBool("Block", combatBehavior.isBlocking);

        animator.SetFloat("Speed", navMeshAgent.velocity.sqrMagnitude);
        animator.SetFloat("Move Y", navMeshAgent.velocity.y);
        animator.SetFloat("Look X", 1);
    }

    public override void OnDeltDamage(float damage, Player player = null)
    {
        if(isStaggered) return;

        if (combatBehavior.isBlocking)
        {
            audioSource.PlayOneShot(BlockAudioClip);
            return;
        }

        damage = Math.Abs(damage);
        health -= damage;

        if (health <= 0 && !isDying)
        {
            OnDeath();
        }
        else if(health > 0)
        {
            audioSource.PlayOneShot(HitAudioClip);
            animator.SetTrigger("Hit");
            isStaggered = canBeStaggered;
            Debug.Log(isStaggered);
        }
    }

    public virtual void OnSuccessfulAttack()
    {
        //Implement in override on custom enemy script
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, debugAttackRange);
        Gizmos.DrawWireSphere(transform.position, 7.0f * detectionSizeModifier);
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 1f), new Vector2(transform.position.x + shootingRange, transform.position.y + 1f));
    }
}
