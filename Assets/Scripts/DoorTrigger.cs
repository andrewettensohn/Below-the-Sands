using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public LevelName levelName;
    public float horizontalPositionAfterLoad;
    public float verticalPositionAfterLoad;
    public bool isLocked;
    public bool isSecondLevelToCatacombEntrance;
    public bool isThirdLevelToCatacombEntrance;

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
        if (isLocked) return;

        if (collider.name == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            if (isSecondLevelToCatacombEntrance) GameManager.instance.milestones.HasOpenedSecondLayerToEntranceRoute = true;
            if (isThirdLevelToCatacombEntrance) GameManager.instance.milestones.HasOpenedThirdLayerToEntranceRoute = true;

            GameManager.instance.LoadScene(levelName.ToString(), new Vector2(horizontalPositionAfterLoad, verticalPositionAfterLoad));
        }
    }
}
