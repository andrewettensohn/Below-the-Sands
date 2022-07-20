using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthPotion : MonoBehaviour
{
    public HealthPotionName healthPotionName;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Ronin")
        {
            collision.collider.GetComponent<Player>().OnHealthPotionPickedUp();
            gameObject.SetActive(false);
        }
    }
}
