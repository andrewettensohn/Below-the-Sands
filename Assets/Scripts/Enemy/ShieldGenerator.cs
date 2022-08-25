using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGenerator : DamageableEnemy
{
    public float health;

    public override void OnDeltDamage(float damage, Player player = null)
    {
        health -= damage;

        if(health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
