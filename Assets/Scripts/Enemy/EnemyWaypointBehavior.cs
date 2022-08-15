using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaypointBehavior : EnemyBehavior
{
    public Transform singleWaypointLocation;
    public LayerMask WaypointLayer;
    public float WaypointDetectionDistance;
    public bool isWaypointFound;
    private Vector3 waypointPos;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isBehaviorEnabled == false) return;

        enemy.AllowMovement();

        if(singleWaypointLocation != null)
        {
            isWaypointFound = true;
            waypointPos = singleWaypointLocation.position;
        }

        enemy.navMeshAgent.stoppingDistance = 0;

        if (!isWaypointFound)
        {
            FindWaypoint();
        }
        else
        {
            enemy.navMeshAgent.SetDestination(waypointPos);
        }
    }

    public void BehaviorStop()
    {
        enemy.enemyWaypointBehavior.isBehaviorEnabled = false;
        enemy.enemyWaypointBehavior.isWaypointFound = false;
        enemy.navMeshAgent.stoppingDistance = enemy.combatBehavior.attackRange;
        enemy.navMeshAgent.SetDestination(enemy.target.position);

        enemy.sentry.isBehaviorEnabled = true;
    }

    private void FindWaypoint()
    {
        //Find Nearest waypoint
        RaycastHit2D waypointHit = Physics2D.CircleCast(transform.position, WaypointDetectionDistance, Vector2.zero, WaypointDetectionDistance, WaypointLayer);

        if (waypointHit.collider != null)
        {
            waypointPos = waypointHit.collider.GetComponent<Transform>().position;
            isWaypointFound = true;
        }
    }
}
