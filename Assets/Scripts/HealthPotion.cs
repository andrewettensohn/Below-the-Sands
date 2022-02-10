using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthPotion : MonoBehaviour
{
    public string potionName;

    private void Start()
    {
        if (GameManager.instance.healthPotionAvailbility[potionName] == false)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            GameManager.instance.healthPotionAvailbility[potionName] = false;
            collision.collider.GetComponent<Player>().OnHealthPotionPickedUp();
            gameObject.SetActive(false);
        }
    }
}
