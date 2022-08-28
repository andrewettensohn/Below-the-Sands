using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlligatorWarrior : Enemy
{
    public AudioClip laughAudioClip;

    public override void OnSuccessfulAttack()
    {
        audioSource.PlayOneShot(laughAudioClip);
        DisableAllBehaviors();
        enemyWaypointBehavior.isBehaviorEnabled = true;
    }
}
