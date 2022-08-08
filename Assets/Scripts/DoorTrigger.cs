using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public string levelName;
    public float horizontalPositionAfterLoad;
    public float verticalPositionAfterLoad;
    public bool isLocked;
    public GameObject bossLockObject;
    public Sprite UnlockedSprite;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if(!bossLockObject.activeSelf)
        {
            isLocked = false;
            spriteRenderer.sprite = UnlockedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        TransportPlayer();
    }

    public void TransportPlayer()
    {
        if (isLocked) return;

        GameManager.instance.LoadScene(levelName.ToString(), new Vector2(horizontalPositionAfterLoad, verticalPositionAfterLoad));
    }
}
