using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndgameCutscene : MonoBehaviour
{
    private TMP_Text tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        RunSlideshow();
    }

    private void RunSlideshow()
    {
        int slideLength = 8;

        string firstSlideText = "The paladin slayed the demon and retrieved the scroll from our Lady Saint Rachel's tomb...";
        StartCoroutine(DisplaySlide(0, firstSlideText));

        string secondSlideText = "And that text spoke of the crusades of old and of the madness of the Xorian warlords...";
        StartCoroutine(DisplaySlide(slideLength, secondSlideText));

        string thirdSlideText = "And it foretold the expansion of the sands and the return of the demons to forge a new domain from the scattered Xorians...";
        StartCoroutine(DisplaySlide(slideLength * 2, thirdSlideText));

        string fourthSlideText = "So the paladin began his journey across the sands once again.";
        StartCoroutine(DisplaySlide(slideLength * 3, fourthSlideText));

        StartCoroutine(EndCutscene(slideLength * 4));
    }

    private IEnumerator DisplaySlide(float secondsBeforeDisplay, string text)
    {
        yield return new WaitForSeconds(secondsBeforeDisplay);
        tmpText.text = text;
    }

    private IEnumerator EndCutscene(float secondsBeforeEnd)
    {
        yield return new WaitForSeconds(secondsBeforeEnd);
        GameManager.instance.LoadMainMenu();
    }

}
