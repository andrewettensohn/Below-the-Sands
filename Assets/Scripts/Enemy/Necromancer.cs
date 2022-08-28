using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Necromancer : Enemy
{
    public List<Transform> teleportPositions;
    public GameObject teleportOrbPrefab;
    private int teleportsDone = 0;
    private bool isTeleporting;
    private float damageTakenForTeleport = 0;
    private bool isEndingMusicPlaying;

    public override void OnDeltDamage(float damage, Player player = null)
    {
        if(isTeleporting || isDying || isStaggered) return;

        damage = Math.Abs(damage);
        health -= damage;
        damageTakenForTeleport += damage;

        Debug.Log($"Necromancer health = {health}");

        if (health <= 0 && !isDying)
        {
            OnDeath();
        }
        else if(health > 0)
        {
            animator.SetTrigger("Hit");
            isStaggered = canBeStaggered;
        }

        if(health <= 10 && !isEndingMusicPlaying)
        {
            isEndingMusicPlaying = true;
            GameManager.instance.SwitchMusicTrack(GameManager.instance.musicTracks.EndingTrack);
        }

        // If the player does 2 damage then this step might be skipped
        if(damageTakenForTeleport >= 10)
        {
            damageTakenForTeleport = 0;
            Debug.Log("Teleport");
            Teleport();
        }
    }

    protected override void OnDeath()
    {
        isDying = true;
        animator.SetTrigger("Die");
        StopMovement();
        DisableAllBehaviors();

        Invoke(nameof(OnDisable), 1.3f);

        GameObject demonObject = GameObject.Find("Demon");

        if(demonObject != null)
        {
            Enemy demon = demonObject.GetComponent<Enemy>();
            demon.OnDeltDamage(demon.health);
        }

        GameManager.instance.PlayEndGameCutscene();
    }

    private void Teleport()
    {
        isTeleporting = true;

        Vector2 teleportLocationPos = teleportPositions[teleportsDone].position;

        animator.SetTrigger("Teleport");

        GameObject orbGameOjbect = Instantiate(teleportOrbPrefab, transform.position, Quaternion.identity);
        TeleportOrb orb = orbGameOjbect.GetComponent<TeleportOrb>();
        orb.Launch(teleportLocationPos, 5);

        DisableAllBehaviors();

        StartCoroutine(HandleTeleportTimer(teleportLocationPos));
    }

    private IEnumerator HandleTeleportTimer(Vector2 teleportLocationPos)
    {
        yield return new WaitForSeconds(1f);

        navMeshAgent.enabled = false;
        transform.position = teleportLocationPos;
        teleportsDone += 1;

        StartCoroutine(HandleTeleportCoolDown());
    }

    private IEnumerator HandleTeleportCoolDown()
    {
        yield return new WaitForSeconds(staggerTime);

        navMeshAgent.enabled = true;
        SetDefaultBehaviors();
        isTeleporting = false;
    }
}
