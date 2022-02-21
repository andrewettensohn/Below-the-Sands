using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerUI : MonoBehaviour
{
    public List<BlockIcon> blockIcons { get; private set; }
    private List<HealthHeart> hearts;
    private List<HealthPotSlot> healthPotSlots;
    private List<RelicSlot> relicSlots;
    private List<PrayerSlot> prayerSlots;

    private void Awake()
    {
        hearts = GetComponentsInChildren<HealthHeart>().ToList();
        blockIcons = GetComponentsInChildren<BlockIcon>().ToList();
        healthPotSlots = GetComponentsInChildren<HealthPotSlot>().ToList();
        relicSlots = GetComponentsInChildren<RelicSlot>().ToList();
        prayerSlots = GetComponentsInChildren<PrayerSlot>().ToList();
    }

    public void SyncHealthPotions()
    {
        int healthPotionsNeededToDisable = healthPotSlots.Count - PlayerInfo.instance.healthPotionCount;
        ChangeHealthPotionSlots(healthPotionsNeededToDisable, false);
    }

    public void ChangeHealthPotionSlots(int healthPotionSlotsNeededToChange, bool isActive)
    {
        int healthPotionSlotsChanged = 0;
        for (int i = 0; i < healthPotSlots.Count; i++)
        {
            if (healthPotSlots[i].isHealthPotSlotActive != isActive && healthPotionSlotsChanged < healthPotionSlotsNeededToChange)
            {
                healthPotSlots[i].ToggleActive(isActive);
                healthPotionSlotsChanged++;
            }
        }
    }

    public void SyncHearts()
    {
        int heartsNeededToChange = PlayerInfo.instance.fullHealth - PlayerInfo.instance.health;

        ChangeHealthHearts(heartsNeededToChange, false);
    }

    public void ChangeRelicSlots(int relicSlotsNeededToChange, bool isActive)
    {
        int relicSlotsChanged = 0;
        for (int i = 0; i < relicSlots.Count; i++)
        {
            if (relicSlots[i].isRelicSlotActive != isActive && relicSlotsChanged < relicSlotsNeededToChange)
            {
                relicSlots[i].ToggleActive(isActive);
                relicSlotsChanged++;
            }
        }
    }

    public void SyncRelicSlots()
    {
        int relicsNeededToChange = relicSlots.Count - PlayerInfo.instance.relicCount;
        ChangeRelicSlots(relicsNeededToChange, false);
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

    public void ChangePrayerSlots(int prayerSlotsToChange, bool isActive)
    {
        int prayerIconsChanged = 0;
        for (int i = prayerSlots.Count - 1; i > -1; i--)
        {
            if (prayerSlots[i].isPrayerSlotActive != isActive && prayerIconsChanged < prayerSlotsToChange)
            {
                prayerSlots[i].ToggleActive(isActive);
                prayerIconsChanged++;
            }
        }
    }

    public void SyncPrayerSlots()
    {
        int prayerSlotsNeededToChange = prayerSlots.Count - PlayerInfo.instance.prayerCount;
        ChangePrayerSlots(prayerSlotsNeededToChange, false);
    }
}
