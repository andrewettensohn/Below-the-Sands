using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSlideCutscene : MonoBehaviour
{
    public List<string> slideTexts;
    public int slideDisplayLength;
    private TMP_Text tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        RunSlideshow();
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            EndCutscene();
        }
    }

    private void RunSlideshow()
    {
        for (int i = 0; i < slideTexts.Count; i++)
        {
            if (i == 0)
            {
                StartCoroutine(DisplaySlide(0, slideTexts[i]));
            }
            else
            {
                StartCoroutine(DisplaySlide(slideDisplayLength * i, slideTexts[i]));
            }
        }

        StartCoroutine(EndCutsceneDelay(slideDisplayLength * slideTexts.Count));
    }

    private IEnumerator DisplaySlide(float secondsBeforeDisplay, string text)
    {
        yield return new WaitForSeconds(secondsBeforeDisplay);
        tmpText.text = text;
    }

    private IEnumerator EndCutsceneDelay(float secondsBeforeEnd)
    {
        yield return new WaitForSeconds(secondsBeforeEnd);
        EndCutscene();
    }

    private void EndCutscene()
    {
        if (GameManager.instance.isIntroCutscenePlaying)
        {
            GameManager.instance.isIntroCutscenePlaying = false;
            GameManager.instance.LoadScene(LevelName.CatacombEntrance.ToString(), new Vector2(-10.5f, 0.6f));
        }
        else if (GameManager.instance.isEndGameCutscenePlaying)
        {
            GameManager.instance.isEndGameCutscenePlaying = false;
            GameManager.instance.LoadMainMenu();
        }
    }

}
