using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatacombLayerDoor : MonoBehaviour
{
    public Sprite LockedSprite;
    public Sprite UnlockedSprite;
    private SpriteRenderer spriteRenderer;
    private DoorTrigger doorTrigger;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorTrigger = GetComponent<DoorTrigger>();
    }

    private void Start()
    {
        bool isUnlocked =
            doorTrigger.levelName == LevelName.FirstLevelEntrance ||
            (doorTrigger.levelName == LevelName.SecondLevelGreatHall && GameManager.instance.milestones.HasOpenedSecondLayerToEntranceRoute) ||
            (doorTrigger.levelName == LevelName.ThirdLevelEntrance && GameManager.instance.milestones.HasOpenedThirdLayerToEntranceRoute);

        doorTrigger.isLocked = !isUnlocked;

        spriteRenderer.sprite = isUnlocked ? UnlockedSprite : LockedSprite;

        Vector3 scale = isUnlocked ? new Vector3(1.216326f, 1.297422f, 1) : new Vector3(1.030567f, 1.122422f, 1);
        transform.localScale = scale;
    }
}
