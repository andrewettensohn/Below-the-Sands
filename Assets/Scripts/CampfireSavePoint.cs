using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireSavePoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Game Saved");
        GameManager.instance.SaveProgress(transform.position);
    }
}