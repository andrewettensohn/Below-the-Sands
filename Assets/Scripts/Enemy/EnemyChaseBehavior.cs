using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseBehavior : EnemyBehavior
{
    public bool canJump;

    private void Update()
    {
        if (!isBehaviorEnabled) return;
        
        enemy.AllowMovement();
        enemy.navMeshAgent.SetDestination(enemy.target.position);

        if(enemy.navMeshAgent.remainingDistance <= enemy.combatBehavior.attackRange)
        {
            enemy.combatBehavior.isBehaviorEnabled = true;
            this.isBehaviorEnabled = false;
        }
    }
}
