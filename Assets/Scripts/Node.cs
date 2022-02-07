using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Vector2> availableDirections { get; private set; }
    public LayerMask obstacleLayer;

    private void Start()
    {
        availableDirections = new List<Vector2>();

        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(new Vector2(-1.0f, 0.5f)); // up left diagonal
        CheckAvailableDirection(new Vector2(1.0f, 0.5f)); // up right diagonal
        CheckAvailableDirection(new Vector2(-0.5f, -1.0f)); // down left diagonal
        CheckAvailableDirection(new Vector2(0.5f, -1.0f)); // down right diagonal
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.0f, obstacleLayer);
        if (hit.collider == null)
        {
            availableDirections.Add(direction);
        }
    }
}
