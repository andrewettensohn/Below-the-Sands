using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcher : DamageableEnemy
{
    [Range(1, 10)]
    public float health;

    [Range(1, 20)]
    public float detectionRange;

    [Range(1, 5)]
    public float shootDelayTime;

    [Range(300, 2000)]
    public int arrowLaunchForce;

    public GameObject arrowPrefab;

    public new CircleCollider2D collider { get; private set; }
    public float lookDirection { get; private set; }
    public LayerMask playerLayer;
    private Animator animator;
    private new Rigidbody2D rigidbody;
    private bool isAttacking;
    private bool canShoot { get; set; } = true;


    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        animator.SetFloat("Look X", lookDirection);

        if (!isAttacking)
        {
            Sentry();
            animator.SetBool("Is Attacking", false);
        }
        else if (health > 0)
        {
            Shoot();
            animator.SetBool("Is Attacking", true);
        }
    }

    private void Sentry()
    {
        HandleSentryForDirection(0.1f);
        HandleSentryForDirection(-0.1f);
    }

    private void Shoot()
    {
        RaycastHit2D hit = GetPlayerHit(lookDirection);

        if (hit.collider == null)
        {
            isAttacking = false;
            return;
        }

        if (canShoot)
        {
            canShoot = false;

            GameObject arrowGameObject = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

            Arrow arrow = arrowGameObject.GetComponent<Arrow>();
            arrow.Launch(new Vector2(lookDirection, 0), arrowLaunchForce);

            StartCoroutine(HandleShootDelayTimer());
        }
    }

    private void HandleSentryForDirection(float direction)
    {
        RaycastHit2D hit = GetPlayerHit(direction);

        if (hit.collider != null)
        {
            isAttacking = true;
            lookDirection = direction;
        }
    }

    private RaycastHit2D GetPlayerHit(float direction)
    {
        return Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, new Vector2(direction, 0.0f), detectionRange, playerLayer);
    }

    private IEnumerator HandleShootDelayTimer()
    {
        yield return new WaitForSeconds(shootDelayTime);

        canShoot = true;
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
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
