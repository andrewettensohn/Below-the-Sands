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
    public EnemyWaypointBehavior enemyWaypointBehavior { get; private set; }
    public AudioClip DeathAudioClip;
    public AudioClip AttackingAudioClip;
    public AudioClip HitAudioClip;
    public AudioClip BlockAudioClip;
    public LayerMask playerLayer;
    public Transform target;
    public bool isStaggered;
    public bool canBeStaggered;
    public bool playerDetected;
    public AudioSource audioSource;
    public int scoreValue;
    protected Animator animator;

    private void Start()
    {
        SetDefaultBehaviors();

        GameObject targetGameObject = GameObject.Find("Ronin");
        if (targetGameObject != null)
        {
            target = targetGameObject.transform;
        }
    }

    private void Awake()
    {
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        chase = GetComponent<EnemyChaseBehavior>();
        sentry = GetComponent<EnemySentryBehavior>();
        combatBehavior = GetComponent<EnemyCombatBehavior>();
        enemyWaypointBehavior = GetComponent<EnemyWaypointBehavior>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

        HandleBehaviors();
        Animate();
    }

    public void SetDefaultBehaviors()
    {
        sentry.isBehaviorEnabled = true;
        chase.isBehaviorEnabled = false;
        combatBehavior.isBehaviorEnabled = true;
    }

    protected void HandleBehaviors()
    {
        if (enemyWaypointBehavior?.isBehaviorEnabled == true)
        {
            chase.isBehaviorEnabled = false;
            combatBehavior.isBehaviorEnabled = false;
            sentry.isBehaviorEnabled = false;
        }
        else if (playerDetected)
        {
            combatBehavior.HandleCombat();
            chase.isBehaviorEnabled = true;
        }
        else
        {
            chase.isBehaviorEnabled = false;
            movement.SetDirection(Vector2.zero);
        }
    }

    protected void OnDeath()
    {
        movement.rigidbody.gravityScale = 0;
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
}
