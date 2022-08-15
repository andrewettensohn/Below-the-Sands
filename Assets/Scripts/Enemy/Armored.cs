using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armored : Enemy
{
    public GameObject arcaneBoltPrefab;
    public float arcaneBoltLaunchForce;
    public int arcaneLaunchesPerCycle;
    public float timeBetweenArcaneBlasts;
    public float postArcaneBlastTimer;
    private bool canBlast;
    private int numberOfBlastsThisCycle;

    public override void OnSuccessfulAttack()
    {
        numberOfBlastsThisCycle = 0;
        
        isCustomBeahviorRunning = true;
        DisableAllBehaviors();
        StopMovement();

        canBlast = true;
    }

    protected override void Update()
    {
        if(isStaggered && !isStaggerTimerActive)
        {
            isStaggerTimerActive = true;
            StartCoroutine(HandleStaggerTimer());
        }

        if(canBlast)
        {
            LaunchArcaneBolts();
        }

        DetermineLookDirection();
        HandleBehaviors();
        Animate();
    }

    private void LaunchArcaneBolts()
    {
        if(!canBlast) return;

        canBlast = false;

        animator.SetBool("Is Blasting", true);

        List<Vector2> launchDirections = new List<Vector2> { Vector2.up, Vector2.right, Vector2.left, Vector2.up, new Vector2(0.5f, 0.5f), new Vector2(-0.5f, 0.5f) };

        foreach(Vector2 launchDirection in launchDirections)
        {
            GameObject projectileGameObject = Instantiate(arcaneBoltPrefab, transform.position, Quaternion.identity);

            ArcaneBolt bolt = projectileGameObject.GetComponent<ArcaneBolt>();
            bolt.Launch(launchDirection, arcaneBoltLaunchForce);
        }

        numberOfBlastsThisCycle += 1;
        StartCoroutine(HandlePostArcaneBlastCycleTimer());
    }

    protected virtual IEnumerator HandlePostArcaneBlastCycleTimer()
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
        yield return new WaitForSeconds(postArcaneBlastTimer);

        animator.SetBool("Is Blasting", false);
        isCustomBeahviorRunning = false;
        SetDefaultBehaviors();
        AllowMovement();
    }
}
