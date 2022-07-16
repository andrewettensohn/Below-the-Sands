using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallbackNode : MonoBehaviour
{
    public LayerMask EnemyLayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckIfWaypointBehaviorShouldEnd(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckIfWaypointBehaviorShouldEnd(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckIfWaypointBehaviorShouldEnd(collision);
    }

    private void CheckIfWaypointBehaviorShouldEnd(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (enemy.enemyWaypointBehavior != null)
            {
                enemy.enemyWaypointBehavior.BehaviorStop();
                enemy.SetDefaultBehaviors();
            }
        }
    }
}
