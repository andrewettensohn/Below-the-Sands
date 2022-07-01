using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallbackNode : MonoBehaviour
{
    public LayerMask EnemyLayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered");
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (enemy.enemyWaypointBehavior != null)
            {
                Debug.Log("Changing");
                enemy.enemyWaypointBehavior.isBehaviorEnabled = false;
                enemy.enemyWaypointBehavior.isWaypointFound = false;
                enemy.SetDefaultBehaviors();
            }
        }
    }
}
