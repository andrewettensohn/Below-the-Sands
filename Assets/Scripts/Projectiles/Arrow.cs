using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public LayerMask obstacleLayer;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;
    private new BoxCollider2D collider;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
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
        if (other.collider.name == "Player")
        {
            Player player = other.collider.GetComponent<Player>();

            if (player == null) return;

            player.OnDeltDamage(-1);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }

        rigidbody2d.AddForce(direction * force);
    }
}
