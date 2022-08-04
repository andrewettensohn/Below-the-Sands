using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Acolyte : Enemy
{
    public GameObject shieldGeneratorGameObject;

    public override void OnDeltDamage(float damage, Player player = null)
    {
        bool isShieldGeneratorActive = shieldGeneratorGameObject != null && shieldGeneratorGameObject.activeSelf;

        if (combatBehavior.isBlocking || isShieldGeneratorActive)
        {
            audioSource.PlayOneShot(BlockAudioClip);
            return;
        }

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
    }
}
