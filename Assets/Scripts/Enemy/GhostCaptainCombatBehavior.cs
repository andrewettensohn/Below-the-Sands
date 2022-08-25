using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCaptainCombatBehavior : EnemyCombatBehavior
{
    public GameObject arcaneBoltPrefab;
    public float arcaneBoltLaunchForce;
    public int arcaneLaunchesPerCycle;
    public float timeBetweenArcaneBlasts;
    private bool canBlast;
    private int numberOfBlastsThisCycle;

    protected override void Update()
    {
        if(canBlast)
        {
            Debug.Log("Launching blast");
            LaunchArcaneBolts();
        }

        base.Update();
    }

    protected override void OnAttack()
    {
        canAttack = false;
        isAttacking = true;

        enemy.audioSource.PlayOneShot(enemy.AttackingAudioClip);
        enemy.animator.SetTrigger("Attack");

        enemy.isCustomBeahviorRunning = true;
        enemy.DisableAllBehaviors();
        enemy.StopMovement();

        StartCoroutine(HandlePostArcaneBlastCycleTimer());
    }

    protected override IEnumerator HandlePostAttackDelayTimer()
    {
        yield return new WaitForSeconds(primaryAttackDealDamageDelay);

        if (enemy.health > 0)
        {
            HandleAttack(primaryAttackHasKnockBack);

            isAttacking = false;
            canLeavePostAttackOpening = true;   
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

    private IEnumerator HandlePostArcaneBlastTimer()
    {
        yield return new WaitForSeconds(enemy.staggerTime);

        enemy.isCustomBeahviorRunning = false;
        enemy.SetDefaultBehaviors();
        enemy.AllowMovement();

        numberOfBlastsThisCycle = 0;

        isAttacking = false;
        canLeavePostAttackOpening = true;  
    }
}
