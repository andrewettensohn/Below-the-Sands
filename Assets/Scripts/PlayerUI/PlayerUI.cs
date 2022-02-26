using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public List<BlockIcon> blockIcons { get; private set; }
    public GameObject healthPotObject;
    public GameObject relicObject;
    public GameObject prayerObject;
    private TMP_Text healthPotCount;
    private TMP_Text relicCount;
    private TMP_Text prayerCount;
    private List<HealthHeart> hearts;

    private void Awake()
    {
        hearts = GetComponentsInChildren<HealthHeart>().ToList();
        blockIcons = GetComponentsInChildren<BlockIcon>().ToList();
        healthPotCount = healthPotObject.GetComponent<TMP_Text>();
        relicCount = relicObject.GetComponent<TMP_Text>();
        prayerCount = prayerObject.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        SyncHealthPotCount();
        SyncRelicCount();
        SyncPrayerCount();
    }

    public void SyncHealthPotCount() => healthPotCount.text = PlayerInfo.instance.healthPotionCount.ToString();

    public void SyncRelicCount() => relicCount.text = PlayerInfo.instance.relicCount.ToString();
    public void SyncPrayerCount() => prayerCount.text = PlayerInfo.instance.prayerCount.ToString();

    public void SyncHearts()
    {
        int heartsNeededToChange = PlayerInfo.instance.fullHealth - PlayerInfo.instance.health;

        ChangeHealthHearts(heartsNeededToChange, false);
    }

    public void ChangeHealthHearts(int heartsNeededToChange, bool isActive)
    {
        int heartsChanged = 0;
        for (int i = hearts.Count - 1; i > -1; i--)
        {
            if (heartsChanged >= heartsNeededToChange) break;

            if (hearts[i].isHealthy == true && isActive == false)
            {
                hearts[i].SetUnhealthy();
                heartsChanged++;
            }
            else if (hearts[i].isHealthy == false && isActive == true)
            {
                hearts[i].SetHealthy();
                heartsChanged++;
            }
        }
    }

    public void ResetBlockIcons()
    {
        foreach (BlockIcon blockIcon in blockIcons)
        {
            blockIcon.SetBlocking();
        }
    }

    public void SetBlockIconsToBroken(int blockIconsToChange)
    {
        int blockIconsChanged = 0;
        for (int i = blockIcons.Count - 1; i > -1; i--)
        {
            if (blockIcons[i].isBlockIconActive && blockIcons[i].isBlocking && blockIconsChanged < blockIconsToChange)
            {
                blockIcons[i].SetBroken();
                blockIconsChanged++;
            }
        }
    }

    public void ToggleBlockIconsActive(int blockIconsToChange, bool isActive)
    {
        int blockIconsChanged = 0;
        for (int i = blockIcons.Count - 1; i > -1; i--)
        {
            if (blockIcons[i].isBlockIconActive != isActive && blockIconsChanged < blockIconsToChange)
            {
                blockIcons[i].ToggleActive(isActive);
                blockIconsChanged++;
            }
        }
    }
}
