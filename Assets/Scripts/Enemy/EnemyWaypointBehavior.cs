using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaypointBehavior : EnemyBehavior
{
    public LayerMask WaypointLayer;
    public float WaypointDetectionDistance;
    public bool isWaypointFound;
    private float xDirection;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isBehaviorEnabled == false) return;

        if (!isWaypointFound)
        {
            FindWaypoint();
        }
        else
        {
            enemy.movement.SetDirection(new Vector2(xDirection, 0f));
        }
    }

    private void FindWaypoint()
    {
        //Find Nearest waypoint
        RaycastHit2D waypointHit = Physics2D.CircleCast(transform.position, WaypointDetectionDistance, Vector2.zero, WaypointDetectionDistance, WaypointLayer);

        if (waypointHit.collider != null)
        {
            xDirection = waypointHit.collider.GetComponent<Transform>().position.x > 0 ? 1 : -1;
            isWaypointFound = true;
        }
    }
}
