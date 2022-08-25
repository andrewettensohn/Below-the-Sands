using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    Slider slider;

    public void Awake()
    {
        gameObject.SetActive(false);
        slider = GetComponentInChildren<Slider>();
    }

    public void Start()
    {
        if (slider != null)
        {
            slider.value = AudioListener.volume;
        }
    }

    public void OpenOptionsMenu()
    {
        gameObject.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        gameObject.SetActive(false);
    }

    public void OnWindowMode()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    public void OnBorderlessWindowMode()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void OnAudioChanged()
    {
        AudioListener.volume = slider.value;
    }
}
