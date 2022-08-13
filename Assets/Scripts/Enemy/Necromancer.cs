using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Necromancer : Enemy
{
    public List<Transform> teleportPositions;
    public int maxHealth;

    protected override void HandleBehaviors()
    {
        if (health <= 0)
        {
            chase.isBehaviorEnabled = false;
            combatBehavior.isBehaviorEnabled = false;
            StopMovement();
            return;
        }

        if (enemyWaypointBehavior?.isBehaviorEnabled == true)
        {
            chase.isBehaviorEnabled = false;
            combatBehavior.isBehaviorEnabled = false;
            sentry.isBehaviorEnabled = false;
            AllowMovement();
        }
        else if (playerDetected)
        {
            combatBehavior.HandleCombat();
            chase.isBehaviorEnabled = true;
            AllowMovement();
        }
        else
        {
            chase.isBehaviorEnabled = false;
            StopMovement();
        }
    }


    public override void OnDeltDamage(float damage, Player player = null)
    {
        damage = Math.Abs(damage);
        health -= damage;

        if (health <= 0 && !isDying)
        {
            OnDeath();
        }
        else if(health <= 0)
        {
            animator.SetTrigger("Hit");
            isStaggered = canBeStaggered;
        }

        if(health % 10 == 0)
        {
            Teleport();
        }
    }

    private void Teleport()
    {
         System.Random random = new System.Random();
         int index = random.Next(teleportPositions.Count);
         transform.position = teleportPositions[index].position;
    }
}
