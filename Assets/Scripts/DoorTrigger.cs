using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public LevelName levelName;
    public float horizontalPositionAfterLoad;
    public float verticalPositionAfterLoad;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        TransportPlayer(collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        TransportPlayer(collider);
    }

    private void TransportPlayer(Collider2D collider)
    {
        if (collider.name == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.instance.LoadScene(levelName.ToString(), new Vector2(horizontalPositionAfterLoad, verticalPositionAfterLoad));
        }
    }
}