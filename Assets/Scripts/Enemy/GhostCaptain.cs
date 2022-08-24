using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostCaptain : Enemy
{
    //public GameObject arcaneBoltPrefab;
    public GameObject magicRiftPrefab;
    // public float arcaneBoltLaunchForce;
    // public int arcaneLaunchesPerCycle;
    // public float timeBetweenArcaneBlasts;
    public float postMagicRiftTimer;
    // private bool canBlast;
    // private int numberOfBlastsThisCycle;

    // public override void OnSuccessfulAttack()
    // {
    //     // numberOfBlastsThisCycle = 0;

    //     // animator.SetTrigger("Secondary Attack");
        
    //     // isCustomBeahviorRunning = true;
    //     // DisableAllBehaviors();
    //     // StopMovement();

    //     // StartCoroutine(HandlePostArcaneBlastCycleTimer());
    // }

    protected override void Update()
    {
        if(isStaggered && !isStaggerTimerActive)
        {
            isStaggerTimerActive = true;
            StartCoroutine(HandleStaggerTimer());
        }

        // if(canBlast)
        // {
        //     LaunchArcaneBolts();
        // }

        DetermineLookDirection();
        Animate();
    }

    // private void LaunchArcaneBolts()
    // {
    //     if(!canBlast) return;

    //     canBlast = false;

    //     List<Vector2> launchDirections = new List<Vector2>
    //     { 
    //         Vector2.up, 
    //         Vector2.right,
    //         Vector2.left,
    //         Vector2.down,
    //         new Vector2(0.5f, 0.5f),
    //         new Vector2(-0.5f, 0.5f),
    //         new Vector2(0.5f, -0.5f),
    //         new Vector2(-0.5f, -0.5f)
    //     };

    //     foreach(Vector2 launchDirection in launchDirections)
    //     {
    //         GameObject projectileGameObject = Instantiate(arcaneBoltPrefab, transform.position, Quaternion.identity);

    //         ArcaneBolt bolt = projectileGameObject.GetComponent<ArcaneBolt>();
    //         bolt.Launch(launchDirection, arcaneBoltLaunchForce);
    //     }

    //     numberOfBlastsThisCycle += 1;
    //     StartCoroutine(HandlePostArcaneBlastCycleTimer());
    // }

    // protected virtual IEnumerator HandlePostArcaneBlastCycleTimer()
    // {
    //     yield return new WaitForSeconds(timeBetweenArcaneBlasts);

    //     if(numberOfBlastsThisCycle >= arcaneLaunchesPerCycle)
    //     {
    //         canBlast = false;
    //         StartCoroutine(HandlePostArcaneBlastTimer());
    //     }
    //     else
    //     {
    //         canBlast = true;
    //     }
    // }

    protected virtual IEnumerator HandlePostArcaneBlastTimer()
    {
        yield return new WaitForSeconds(staggerTime);

        isCustomBeahviorRunning = false;
        SetDefaultBehaviors();
        AllowMovement();
    }

    protected virtual IEnumerator HandlePostRiftAttackTimer()
    {
        yield return new WaitForSeconds(postMagicRiftTimer);

        isCustomBeahviorRunning = false;
        SetDefaultBehaviors();
        AllowMovement();
    }

    public override void OnDeltDamage(float damage, Player player = null)
    {
        if(isStaggered) return;

        damage = Math.Abs(damage);
        health -= damage;

        if (health <= 0 && !isDying)
        {
            OnDeath();
        }
        else if(health > 0)
        {
            animator.SetTrigger("Hit");
            isStaggered = canBeStaggered;
            CreateRiftsAttack();
        }
    }

    private void CreateRiftsAttack()
    {
        isCustomBeahviorRunning = true;
        DisableAllBehaviors();
        StopMovement();

        List<Vector2> directions = new List<Vector2> 
        { 
            new Vector2(transform.position.x, transform.position.y + 1f),
            new Vector2(transform.position.x + 1.0f, transform.position.y),
            new Vector2(transform.position.x - 1.0f, transform.position.y),
        };

        foreach(Vector2 direction in directions)
        {
            GameObject projectileGameObject = Instantiate(magicRiftPrefab, direction, Quaternion.identity);

            MagicRift rift = projectileGameObject.GetComponent<MagicRift>();
            rift.StartLifeTimeTimer();
        }

        StartCoroutine(HandlePostRiftAttackTimer());
    }
}
