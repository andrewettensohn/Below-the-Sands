using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Necromancer : Enemy
{
    public List<Transform> teleportPositions;
    public int maxHealth;

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
