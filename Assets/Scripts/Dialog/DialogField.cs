using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogField : MonoBehaviour
{
    public TMP_Text dialog;
    public Azkul azkul;
    public int nextBranchId;
    private Button button;

    private void Awake()
    {
        dialog = GetComponentInChildren<TMP_Text>();
    }

    public void GetNextDialogBranch()
    {
        azkul.LoadDialogBranchFromID(nextBranchId);
    }
}
