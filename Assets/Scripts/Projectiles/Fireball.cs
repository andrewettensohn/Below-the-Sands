using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private new BoxCollider2D collider;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        bool isEnemy = other.collider.TryGetComponent<DamageableEnemy>(out DamageableEnemy enemy);

        if(isEnemy)
        {
            enemy.OnDeltDamage(-1);
            Destroy(gameObject);
        }
        else if(other.collider.name != GameManager.instance.playerCharacterName)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        animator.SetFloat("X Direction", direction.x);

        rigidbody2d.AddForce(direction * force);
    }
}
