using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatacombLayerDoor : MonoBehaviour
{
    public Sprite LockedSprite;
    public Sprite UnlockedSprite;
    public LevelName levelName;
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
            levelName == LevelName.FirstLevelEntrance ||
            (levelName == LevelName.SecondLevelEntrance && GameManager.instance.milestones.HasOpenedSecondLayerToEntranceRoute);

        doorTrigger.isLocked = !isUnlocked;

        spriteRenderer.sprite = isUnlocked ? UnlockedSprite : LockedSprite;

        Vector3 scale = isUnlocked ? new Vector3(1.216326f, 1.297422f, 1) : new Vector3(1.030567f, 1.122422f, 1);
        transform.localScale = scale;
    }
}
