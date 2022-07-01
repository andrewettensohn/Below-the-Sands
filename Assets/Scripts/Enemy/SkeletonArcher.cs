using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcher : DamageableEnemy
{
    [Range(1, 10)]
    public float health;

    [Range(1, 100)]
    public float detectionRange;

    [Range(1, 5)]
    public float shootDelayTime;

    [Range(300, 5000)]
    public int arrowLaunchForce;

    public bool freeFire;

    public GameObject arrowPrefab;

    public new CircleCollider2D collider { get; private set; }
    public float lookDirection;
    public LayerMask playerLayer;
    public AudioClip DeathAudioClip;
    public AudioClip AttackingAudioClip;
    private Animator animator;
    private new Rigidbody2D rigidbody;
    private AudioSource audioSource;
    private bool isDying;
    private bool isPlayerDetected;
    private bool canShoot;
    private bool isWaitingToShoot;
    private bool hasDelayedShot;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        animator.SetFloat("Look X", lookDirection);

        if (freeFire)
        {
            isPlayerDetected = true;
        }

        if (isPlayerDetected && health > 0)
        {
            HandleCombat();
        }
        else
        {
            Sentry();
        }
    }

    private void Sentry()
    {
        HandleSentryForDirection(0.1f);
        HandleSentryForDirection(-0.1f);
    }

    private void HandleCombat()
    {
        RaycastHit2D hit = GetPlayerHit(lookDirection);

        if (hit.collider == null && !freeFire)
        {
            isPlayerDetected = false;
            hasDelayedShot = false;
        }

        if (!hasDelayedShot && !isWaitingToShoot)
        {
            hasDelayedShot = true;
            StartCoroutine(HandleShootDelayTimer());
        }

        if (canShoot && !isWaitingToShoot)
        {
            canShoot = false;
            FireArrow();
            StartCoroutine(HandleShootDelayTimer());
        }
    }

    private void HandleSentryForDirection(float direction)
    {
        RaycastHit2D hit = GetPlayerHit(direction);

        if (hit.collider != null)
        {
            isPlayerDetected = true;
            lookDirection = direction;
        }
    }

    private RaycastHit2D GetPlayerHit(float direction)
    {
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.4f, 0.0f, new Vector2(direction, 0.0f), detectionRange, playerLayer);
    }

    private void FireArrow()
    {
        animator.SetTrigger("Is Attacking");

        GameObject arrowGameObject = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        Arrow arrow = arrowGameObject.GetComponent<Arrow>();
        arrow.Launch(new Vector2(lookDirection, 0), arrowLaunchForce);
        audioSource.PlayOneShot(AttackingAudioClip);
    }

    private IEnumerator HandleShootDelayTimer()
    {
        isWaitingToShoot = true;
        yield return new WaitForSeconds(shootDelayTime);
        isWaitingToShoot = false;
        canShoot = true;
    }

    public override void OnDeltDamage(float damage)
    {
        damage = Mathf.Abs(damage);

        health -= damage;

        if (health <= 0 && !isDying)
        {
            isDying = true;
            animator.SetTrigger("Die");
            audioSource.PlayOneShot(DeathAudioClip);
            collider.isTrigger = true;
            rigidbody.gravityScale = 0;
            Invoke(nameof(OnDisable), 1.3f);
        }
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
