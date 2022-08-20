using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseBehavior : EnemyBehavior
{

    private void FixedUpdate()
    {
        if (!isBehaviorEnabled) return;

        Debug.Log("Chase is on");
        
        enemy.SetDestinationToPlayer();

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
        else if(enemy.isArcher) // the archer has the ability to shoot through nodes so they need extra logic to determine if a player is within range
        {
            //Check if the player is in range of the current look direction
            Collider2D[] playerHits = Physics2D.OverlapCircleAll(enemy.attackPoint.position, enemy.combatBehavior.attackRange, enemy.playerLayer);

            if(playerHits.Length > 0)
            {
                TransitionToCombatBehavior();
                return;
            }

            bool isFacingRight = transform.rotation == Quaternion.identity;
            bool isFacingLeft = transform.rotation != Quaternion.identity;
            float oppositeDirection = isFacingLeft ? enemy.shootingRange : -enemy.shootingRange;

            Vector3 reversedAttackPoint = new Vector2(enemy.attackPoint.position.x + oppositeDirection, enemy.attackPoint.position.y);
        
            // Check if the player would be in range if the enemy turned around
            Collider2D[] turnedAroundPlayerHits = Physics2D.OverlapCircleAll(reversedAttackPoint, enemy.combatBehavior.attackRange, enemy.playerLayer);

            if(turnedAroundPlayerHits.Length > 0)
            {
                //Turn around
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
