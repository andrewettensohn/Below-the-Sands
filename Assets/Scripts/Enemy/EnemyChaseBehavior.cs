using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseBehavior : EnemyBehavior
{

    private void FixedUpdate()
    {
        if (!isBehaviorEnabled) return;
        Debug.Log("Chase is on");
        
        enemy.navMeshAgent.SetDestination(enemy.target.position);

        if(enemy.navMeshAgent.remainingDistance == float.PositiveInfinity)
        {
            enemy.StopMovement();
        }
        else
        {
            enemy.AllowMovement();
        }

        if(enemy.navMeshAgent.remainingDistance <= enemy.combatBehavior.attackRange && !enemy.isArcher)
        {
            TransitionToCombatBehavior();
        }
        else if(enemy.isArcher)
        {
            Collider2D[] playerHits = Physics2D.OverlapCircleAll(enemy.attackPoint.position, enemy.combatBehavior.attackRange, enemy.playerLayer);

            Debug.Log($"Attack point {enemy.attackPoint.position}");

            if(playerHits.Length > 0)
            {
                Debug.Log("Player is present in current look direction");
                TransitionToCombatBehavior();
                return;
            }

            bool isFacingRight = transform.rotation == Quaternion.identity;
            bool isFacingLeft = transform.rotation != Quaternion.identity;
            float oppositeDirection = isFacingLeft ? enemy.shootingRange : -enemy.shootingRange;

            Vector3 reversedAttackPoint = new Vector2(enemy.attackPoint.position.x + oppositeDirection, enemy.attackPoint.position.y);
            
            Debug.Log($"Reversed attack point {reversedAttackPoint}");

            Collider2D[] turnedAroundPlayerHits = Physics2D.OverlapCircleAll(reversedAttackPoint, enemy.combatBehavior.attackRange, enemy.playerLayer);

            if(turnedAroundPlayerHits.Length > 0)
            {
                Debug.Log("Player is present in turned look direction");
                if (isFacingLeft)
                {
                    transform.rotation = Quaternion.identity;
                }
                else if (isFacingRight)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                TransitionToCombatBehavior();

                return;
            }
        }
    }

    
    public void TransitionToCombatBehavior()
    {
        this.isBehaviorEnabled = false;
        enemy.combatBehavior.isBehaviorEnabled = true;
    }
}
