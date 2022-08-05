using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Acolyte : Enemy
{
    public List<GameObject> shieldGeneratorGameObjects;
    public GameObject magicShieldGameObject;

    private void FixedUpdate()
    {
        if(shieldGeneratorGameObjects != null && !shieldGeneratorGameObjects.Any(x => x.activeSelf))
        {
            magicShieldGameObject.SetActive(false);
        }
    }

    public override void OnDeltDamage(float damage, Player player = null)
    {
        bool isShieldGeneratorActive = shieldGeneratorGameObjects != null && shieldGeneratorGameObjects.Any(x => x.activeSelf);

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
