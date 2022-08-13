using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == GameManager.instance.playerCharacterName)
        {
            Player player = collision.GetComponent<Player>();
            player.OnDeltDamage(-1, true);
        }
    }
}
