using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Range(1, 10)]
    public float speed;

    [Range(1, 5)]
    public float fallingMultiplier;

    [Range(1, 10)]
    public float jumpVelocity;

    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 lookDirection = new Vector2(1, 0);
    public LayerMask obstacleLayer;

    public bool canMove = true;
    public bool isJumping;
    public bool isGrounded;
    public bool fallenIntoAbyss;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        isGrounded = true;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        //Jump
        if (isGrounded == true && isJumping)
        {
            rigidbody.velocity += Vector2.up * jumpVelocity;
            isGrounded = false;
        }

        // Set Look
        if (!Mathf.Approximately(direction.x, 0.0f))
        {
            lookDirection.Set(direction.x, 0.0f);
            lookDirection.Normalize();
        }

        if (!canMove) return;

        // If the object is falling
        if (rigidbody.velocity.y < 0)
        {
            rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallingMultiplier - 1) * Time.fixedDeltaTime;
        }

        // Determine velocity
        rigidbody.velocity = new Vector2(direction.x * speed, rigidbody.velocity.y);

        //Destroy the object if it falls off the map
        if (rigidbody.position.y < -15.0f)
        {
            // TODO: Let objects read this and handle whether they are active
            gameObject.SetActive(false);
            fallenIntoAbyss = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Platform")
        {
            isGrounded = true;
        }
    }

    private bool Occupied(Vector2 direction)
    {
        // If no collider is hit then there is no obstacle in that direction
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.5f, obstacleLayer);
        return hit.collider != null;
    }

    public void SetDirection(Vector2 newDirection, bool forced = false)
    {
        direction = newDirection;
    }
}
