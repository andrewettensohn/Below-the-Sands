using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == GameManager.instance.playerCharacterName)
        {
            Debug.Log("Slow");
            Player player = collision.GetComponent<Player>();
            player.movement.speed = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == GameManager.instance.playerCharacterName)
        {
            Player player = collision.GetComponent<Player>();
            player.movement.speed = player.defaultSpeed;
        }
    }
}
