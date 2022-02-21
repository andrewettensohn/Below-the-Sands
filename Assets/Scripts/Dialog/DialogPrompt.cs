using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogPrompt : MonoBehaviour
{
    public TMP_Text dialog;

    private void Awake()
    {
        dialog = GetComponent<TMP_Text>();
    }
}
