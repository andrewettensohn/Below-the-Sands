using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickCutscene : MonoBehaviour
{
    private TMP_Text tmpText;
    public string levelName;
    public float horizontalPositionAfterLoad;
    public float verticalPositionAfterLoad;
    public List<string> slideTextList;
    private int currentSlide;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        tmpText.text = slideTextList[currentSlide];
    }

    public void Continue()
    {
        if(currentSlide < slideTextList.Count - 1 )
        {
            currentSlide += 1;
            tmpText.text = slideTextList[currentSlide];
        }
        else if(currentSlide == slideTextList.Count - 1)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            GameManager.instance.LoadScene(levelName.ToString(), new Vector2(horizontalPositionAfterLoad, verticalPositionAfterLoad));
        }
    }
}
