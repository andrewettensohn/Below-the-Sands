using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockingDoor : DamageableEnemy
{
    public GameObject Parent;
    public float health;

    public override void OnDeltDamage(float damage, Player player = null)
    {
        damage = Math.Abs(damage);
        health -= damage;

        if (health <= 0)
        {
            Parent.gameObject.SetActive(false);
        }
        else if(health > 0)
        {
            // animator.SetTrigger("Hit");
            // isStaggered = canBeStaggered;
            // Debug.Log(isStaggered);
        }
    }
}
