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

        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 0.735f, obstacleLayer);

        //Jump
        if (isJumping && groundHit.collider != null)
        {
            rigidbody.velocity += Vector2.up * jumpVelocity;
        }

        isGrounded = groundHit.collider != null;

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
            gameObject.SetActive(false);

            if (gameObject.name == "Ronin") GameManager.instance.GameOver();
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
