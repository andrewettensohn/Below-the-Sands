using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prayer : MonoBehaviour
{
    public LevelName levelName;

    private void Start()
    {
        if (GameManager.instance.prayerAvailbility[levelName] == false)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            GameManager.instance.prayerAvailbility[levelName] = false;
            collision.collider.GetComponent<Player>().OnPrayerPickedUp();
            gameObject.SetActive(false);
        }
    }
}
