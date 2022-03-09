using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandedSwordPickup : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            PlayerInfo.instance.hasTwoHandSword = true;
            Player player = collision.collider.GetComponent<Player>();
            player.ToggleShieldEquipped(false);
            player.ToggleTwoHandSwordEquipped(true);
            gameObject.SetActive(false);
        }
    }
}
