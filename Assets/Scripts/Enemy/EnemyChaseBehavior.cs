using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseBehavior : EnemyBehavior
{
    public bool canJump;

    private void Update()
    {
        if (!isBehaviorEnabled) return;
        // if (canJump)
        // {
        //     enemy.navMeshAgent.SetDestination(enemy.target.position);
        // }
        // else
        // {
        //     enemy.navMeshAgent.SetDestination(new Vector2(enemy.target.position.x, 0.0f));
        // }
        enemy.navMeshAgent.SetDestination(enemy.target.position);
    }
}
