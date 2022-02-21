using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogBranch
{
    public int BranchID;
    public string Prompt;
    public List<DialogResponse> Responses;
}
