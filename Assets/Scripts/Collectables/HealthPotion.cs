using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthPotion : MonoBehaviour
{
    public LevelName levelName;

    private void Start()
    {
        if (GameManager.instance.healthPotionAvailbility[levelName] == false)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            GameManager.instance.healthPotionAvailbility[levelName] = false;
            collision.collider.GetComponent<Player>().OnHealthPotionPickedUp();
            gameObject.SetActive(false);
        }
    }
}
