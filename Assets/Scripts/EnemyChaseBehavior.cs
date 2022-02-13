using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseBehavior : EnemyBehavior
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Chase(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Chase(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Chase(other);
    }

    private void Chase(Collider2D other)
    {
        // 10 is the node layer
        if (!isBehaviorEnabled || other.gameObject.layer != 10) return;

        Node node = other.GetComponent<Node>();

        if (node != null)
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            foreach (Vector2 availableDirection in node.availableDirections)
            {
                float distance;

                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);

                float xDifference = (Mathf.Abs(newPosition.x) - Mathf.Abs(enemy.target.position.x));

                if (xDifference <= 0.5 && xDifference >= -0.5)
                {
                    distance = float.MaxValue;
                }
                else
                {
                    distance = (enemy.target.position - newPosition).sqrMagnitude;
                }

                if (distance < minDistance)
                {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

            if (!enemy.combatBehavior.isInCombat && enemy.health > 0)
            {
                enemy.movement.SetDirection(direction);
            }
            else
            {
                enemy.movement.SetDirection(Vector2.zero);
            }
        }
    }
}
