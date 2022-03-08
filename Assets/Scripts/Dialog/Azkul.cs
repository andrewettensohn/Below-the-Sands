using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Azkul : MonoBehaviour
{
    public DialogUI dialogUI;

    private void Start()
    {
        dialogUI.gameObject.SetActive(false);
    }

    private List<DialogBranch> GetDialogBranches()
    {
        TextAsset dialogPackText = Resources.Load<TextAsset>("Text/AzkulDialogBranches");
        DialogPack pack = JsonUtility.FromJson<DialogPack>(dialogPackText.text);
        return pack.DialogBranches;
    }

    private void SetDialogValues()
    {
        if (GameManager.instance.milestones.IsFistVisitToCatacomb)
        {
            LoadDialogBranch(GetDialogBranches().FirstOrDefault(x => x.BranchID == 100));
            GameManager.instance.milestones.IsFistVisitToCatacomb = false;
        }
        else if (!GameManager.instance.milestones.HasCompletedThirdLayer)
        {
            LoadDialogBranch(GetDynamicMainBranch());
        }
    }

    private DialogBranch GetDynamicMainBranch()
    {
        DialogBranch dynamicMainBranch = GetDialogBranches().FirstOrDefault(x => x.BranchID == 200);

        dynamicMainBranch.Responses = dynamicMainBranch.Responses.Select(x => new DialogResponse { Response = x.Response, NextBranchID = x.NextBranchID }).ToList();

        List<DialogResponse> filteredResponses = new List<DialogResponse>();

        Milestones milestones = GameManager.instance.milestones;

        foreach (DialogResponse response in dynamicMainBranch.Responses)
        {
            if (response.NextBranchID == 210 && !milestones.HasFinishedAzkulQuest && PlayerInfo.instance.relicCount >= 3)
            {
                filteredResponses.Add(response);
            }
            else if (response.NextBranchID == 220 && milestones.HasFinishedAzkulQuest && !milestones.HasAskedAzkulForSupplies)
            {
                filteredResponses.Add(response);
            }
            else if (response.NextBranchID == 230 && milestones.HasFinishedAzkulQuest && milestones.HasAskedAzkulForSupplies && PlayerInfo.instance.relicCount > 0)
            {
                filteredResponses.Add(response);
            }
            else if (response.NextBranchID == 240 && !milestones.HasFinishedAzkulQuest && !milestones.HasAskedAzkulForSupplies)
            {
                filteredResponses.Add(response);
            }
            else if (response.NextBranchID == 0)
            {
                filteredResponses.Add(response);
            }
        }

        dynamicMainBranch.Responses = filteredResponses;
        return dynamicMainBranch;
    }

    private void ResetDialogFields()
    {
        for (int i = 0; i < dialogUI.dialogFields.Count(); i++)
        {
            dialogUI.dialogFields[i].dialog.text = string.Empty;
            dialogUI.dialogFields[i].nextBranchId = 0;
        }
    }

    private void LoadDialogBranch(DialogBranch dialogBranch)
    {
        ResetDialogFields();

        if (dialogBranch == null) return;

        dialogUI.dialogPrompt.dialog.text = dialogBranch.Prompt;

        if (dialogBranch.Responses.Count == 0) return;

        for (int i = 0; i < dialogBranch.Responses.Count; i++)
        {
            dialogUI.dialogFields[i].dialog.text = dialogBranch.Responses[i].Response != null ? dialogBranch.Responses[i].Response : string.Empty;
            dialogUI.dialogFields[i].nextBranchId = dialogBranch.Responses[i].NextBranchID != 0 ? dialogBranch.Responses[i].NextBranchID : 0;
        }
    }

    private void CheckForMilestoneFromBranchId(int branchID)
    {
        if (branchID == 210)
        {
            GameManager.instance.milestones.HasFinishedAzkulQuest = true;
            PlayerInfo.instance.relicCount -= 3;
        }
        else if (branchID == 220 || branchID == 240)
        {
            GameManager.instance.milestones.HasAskedAzkulForSupplies = true;
        }
        else if (branchID == 230)
        {
            PlayerInfo.instance.healthPotionCount += PlayerInfo.instance.relicCount;
            PlayerInfo.instance.relicCount = 0;
        }

        SyncPlayerUI();
    }

    private void SyncPlayerUI()
    {
        PlayerUI ui = GameObject.Find("Player").GetComponent<Player>().playerUI;

        ui.SyncHealthPotCount();
        ui.SyncRelicCount();
    }

    public void LoadDialogBranchFromID(int dialogBranchID)
    {
        DialogBranch dialogBranch = null;

        if (dialogBranchID == 200)
        {
            dialogBranch = GetDynamicMainBranch();
        }
        else
        {
            dialogBranch = GetDialogBranches().FirstOrDefault(x => x.BranchID == dialogBranchID);
        }

        if (dialogBranchID == 0 || dialogBranch == null)
        {
            dialogUI.gameObject.SetActive(false);
            GameManager.instance.isPlayerControlRestricted = false;
        }
        else
        {
            CheckForMilestoneFromBranchId(dialogBranchID);
            LoadDialogBranch(dialogBranch);
        }
    }

    public void OpenDialog()
    {
        dialogUI.gameObject.SetActive(true);
        SetDialogValues();
    }
}
