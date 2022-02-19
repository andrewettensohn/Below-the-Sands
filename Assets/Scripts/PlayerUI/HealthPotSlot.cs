using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotSlot : MonoBehaviour
{
    public bool isHealthPotSlotActive { get; private set; } = true;

    public void ToggleActive(bool isActive)
    {
        isHealthPotSlotActive = isActive;
        gameObject.SetActive(isActive);
    }
}
