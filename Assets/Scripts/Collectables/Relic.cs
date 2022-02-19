using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relic : MonoBehaviour
{

    public LevelName levelName;

    private void Start()
    {
        if (GameManager.instance.relicAvailbility[levelName] == false)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            GameManager.instance.relicAvailbility[levelName] = false;
            collision.collider.GetComponent<Player>().OnRelicPickedUp();
            gameObject.SetActive(false);
        }
    }
}
