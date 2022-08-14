using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseBehavior : EnemyBehavior
{
    public bool canJump;

    private void Update()
    {
        if (!isBehaviorEnabled) return;
        enemy.navMeshAgent.isStopped = false;
        enemy.navMeshAgent.SetDestination(enemy.target.position);
    }
}
