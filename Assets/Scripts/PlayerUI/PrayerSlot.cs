using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerSlot : MonoBehaviour
{
    public bool isPrayerSlotActive { get; private set; } = true;

    public void ToggleActive(bool isActive)
    {
        isPrayerSlotActive = isActive;
        gameObject.SetActive(isActive);
    }
}
