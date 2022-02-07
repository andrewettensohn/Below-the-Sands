using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonChaseBehavior : SkeletonBehavior
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
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);

                float distance = (skeleton.target.position - newPosition).sqrMagnitude;

                if (distance < minDistance)
                {
                    direction = availableDirection;
                    minDistance = distance;
                }
            }

            if (!skeleton.combatBehavior.isInCombat && skeleton.health > 0)
            {
                skeleton.movement.SetDirection(direction);
            }
            else
            {
                skeleton.movement.SetDirection(Vector2.zero);
            }
        }
    }
}
