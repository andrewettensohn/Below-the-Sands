using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritPowerup : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Ronin")
        {
            collision.collider.GetComponent<Player>().OnSpiritPowerupPickedUp();
            gameObject.SetActive(false);
        }
    }
}
