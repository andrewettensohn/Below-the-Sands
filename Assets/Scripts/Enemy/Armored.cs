using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Armored : Enemy
{

    public override void OnDeltDamage(float damage)
    {
        damage = Math.Abs(damage);
        health -= damage;

        if (health <= 0)
        {
            OnDeath();
        }
        else
        {
            audioSource.PlayOneShot(DeathAudioClip);
            animator.SetTrigger("Hit");
            isStaggered = canBeStaggered;
            enemyWaypointBehavior.isBehaviorEnabled = true;
        }
    }
}
