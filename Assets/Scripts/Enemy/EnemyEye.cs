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
    public AudioClip DeathAudioClip;
    public AudioClip AttackingAudioClip;
    public AudioClip HitAudioClip;
    public AudioSource audioSource;
    private Animator animator;
    private EnemyEyeCombatBehavior enemyEyeCombatBehavior;
    private bool isPlayerDetected;
    private bool isDying;

    private void Awake()
    {
        enemyEyeCombatBehavior = GetComponent<EnemyEyeCombatBehavior>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
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
        if (health <= 0 && !isDying)
        {
            isDying = true;
            animator.SetTrigger("Die");
            OnDeath();
            return;
        }

        animator.SetBool("Is Attacking", enemyEyeCombatBehavior.isAttacking);
        animator.SetFloat("Look X", aiPath.desiredVelocity.x);
    }

    private void OnDeath()
    {
        audioSource.PlayOneShot(DeathAudioClip);
        collider.isTrigger = true;
        Invoke(nameof(OnDisable), 1.0f);
    }

    public override void OnDeltDamage(float damage, Player player = null)
    {
        audioSource.PlayOneShot(HitAudioClip);
        damage = Mathf.Abs(damage);
        health -= damage;
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
