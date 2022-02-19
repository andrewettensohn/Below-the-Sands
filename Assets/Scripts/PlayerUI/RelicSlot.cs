using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicSlot : MonoBehaviour
{
    public bool isRelicSlotActive { get; private set; } = true;

    public void ToggleActive(bool isActive)
    {
        isRelicSlotActive = isActive;
        gameObject.SetActive(isActive);
    }
}
