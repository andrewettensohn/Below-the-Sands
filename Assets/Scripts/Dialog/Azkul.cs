using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Azkul : MonoBehaviour
{
    public DialogUI dialogUI;
    private List<DialogBranch> dialogBranches;

    private void Start()
    {
        TextAsset dialogPackText = Resources.Load<TextAsset>("Text/AzkulDialogBranches");
        DialogPack pack = JsonUtility.FromJson<DialogPack>(dialogPackText.text);

        dialogBranches = pack.DialogBranches.ToList();

        dialogUI.gameObject.SetActive(false);
    }

    private void SetDialogValues()
    {
        if (GameManager.instance.azkulDialogStatus.IsNewGame)
        {
            LoadDialogBranch(dialogBranches.FirstOrDefault(x => x.BranchID == 100));
            GameManager.instance.azkulDialogStatus.IsNewGame = false;
        }
    }

    private void LoadDialogBranch(DialogBranch dialogBranch)
    {
        if (dialogBranch == null) return;

        dialogUI.dialogPrompt.dialog.text = dialogBranch.Prompt;

        if (dialogBranch.Responses.Count == 0) return;

        for (int i = 0; i < dialogUI.dialogFields.Count(); i++)
        {
            dialogUI.dialogFields[i].dialog.text = dialogBranch.Responses[i].Response != null ? dialogBranch.Responses[i].Response : string.Empty;
            dialogUI.dialogFields[i].nextBranchId = dialogBranch.Responses[i].NextBranchID != 0 ? dialogBranch.Responses[i].NextBranchID : 0;
        }
    }

    public void LoadDialogBranchFromID(int dialogBranchID)
    {
        DialogBranch dialogBranch = dialogBranches.FirstOrDefault(x => x.BranchID == dialogBranchID);

        if (dialogBranchID == 0 || dialogBranch == null)
        {
            dialogUI.gameObject.SetActive(false);
            GameManager.instance.isPlayerControlRestricted = false;
        }
        else
        {
            LoadDialogBranch(dialogBranch);
        }
    }

    public void OpenDialog()
    {
        dialogUI.gameObject.SetActive(true);
        SetDialogValues();
    }
}
