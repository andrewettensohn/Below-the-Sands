using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            PlayerInfo.instance.hasShield = true;
            collision.collider.GetComponent<Player>().ToggleShieldEquipped(true);
            gameObject.SetActive(false);
        }
    }
}
