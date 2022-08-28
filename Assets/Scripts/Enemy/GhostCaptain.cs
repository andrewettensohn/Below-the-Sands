using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostCaptain : Enemy
{
    public GameObject arcaneBoltPrefab;
    public float arcaneBoltLaunchForce;
    public int arcaneLaunchesPerCycle;
    public float timeBetweenArcaneBlasts;
    private bool canBlast;
    private int numberOfBlastsThisCycle;
    private int damageTakeSinceLastArcaneBlast;

    protected override void Update()
    {
        if(isStaggered && !isStaggerTimerActive && !isCustomBeahviorRunning)
        {
            isStaggerTimerActive = true;
            StartCoroutine(HandleStaggerTimer());
        }

        if(canBlast)
        {
            Debug.Log("Launching blast");
            LaunchArcaneBolts();
        }

        DetermineLookDirection();
        Animate();
    }

    public override void OnDeltDamage(float damage, Player player = null)
    {
        if(isStaggered) return;

        damage = Math.Abs(damage);
        health -= damage;
        damageTakeSinceLastArcaneBlast += 1;

        if (health <= 0 && !isDying)
        {
            OnDeath();
        }
        else if(damageTakeSinceLastArcaneBlast >= 3)
        {
            damageTakeSinceLastArcaneBlast = 0;

            isCustomBeahviorRunning = true;
            isStaggered = true;

            DisableAllBehaviors();
            StopMovement();
            StartCoroutine(HandlePreArcaneBlastDelay());
        }
        else if(health > 0)
        {
            animator.SetTrigger("Hit");
            isStaggered = true;
        }

    }

    private void LaunchArcaneBolts()
    {
        if(!canBlast) return;

        canBlast = false;

        List<Vector2> launchDirections = new List<Vector2>
        { 
            Vector2.up, 
            Vector2.right,
            Vector2.left,
            Vector2.down,
            new Vector2(0.5f, 0.5f),
            new Vector2(-0.5f, 0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(-0.5f, -0.5f)
        };

        foreach(Vector2 launchDirection in launchDirections)
        {
            GameObject projectileGameObject = Instantiate(arcaneBoltPrefab, transform.position, Quaternion.identity);

            ArcaneBolt bolt = projectileGameObject.GetComponent<ArcaneBolt>();
            bolt.Launch(launchDirection, arcaneBoltLaunchForce);
        }

        numberOfBlastsThisCycle += 1;
        StartCoroutine(HandlePostArcaneBlastCycleTimer());
    }

    private IEnumerator HandlePreArcaneBlastDelay()
    {
        yield return new WaitForSeconds(staggerTime);

        animator.SetTrigger("Secondary Attack");
        StartCoroutine(HandleArcaneBlastAnimationDelay());
    }

    private IEnumerator HandleArcaneBlastAnimationDelay()
    {
        yield return new WaitForSeconds(1.8f);

        canBlast = true;
    }

    private IEnumerator HandlePostArcaneBlastCycleTimer()
    {
        yield return new WaitForSeconds(timeBetweenArcaneBlasts);

        if(numberOfBlastsThisCycle >= arcaneLaunchesPerCycle)
        {
            canBlast = false;
            StartCoroutine(HandlePostArcaneBlastTimer());
        }
        else
        {
            canBlast = true;
        }
    }

    protected virtual IEnumerator HandlePostArcaneBlastTimer()
    {
        yield return new WaitForSeconds(staggerTime);
        numberOfBlastsThisCycle = 0;
        isCustomBeahviorRunning = false;
        isStaggered = false;
        SetDefaultBehaviors();
        AllowMovement();
    }
}
