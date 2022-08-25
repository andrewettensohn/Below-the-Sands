using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class MenuController : MonoBehaviour
{
    private Canvas canvas;
    private List<Button> buttons;
    private TMP_Text menuTitle;

    public void Awake()
    {
        canvas = GetComponent<Canvas>();
        buttons = GetComponentsInChildren<Button>().ToList();
        menuTitle = GameObject.Find("MenuTitle").GetComponent<TMP_Text>();
    }

    public void Start()
    {
        canvas.enabled = false;
        GameManager.instance.isGamePaused = false;
    }

    public void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (GameManager.instance.isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            IsPauseMode(true);
            PauseGame();
        }
    }

    public void LoadMainMenu()
    {
        ResumeGame();
        GameManager.instance.LoadMainMenu();
    }

    public void PauseGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.isGamePaused = true;
        canvas.enabled = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.instance.isGamePaused = false;
        Time.timeScale = 1;
        canvas.enabled = false;
    }

    public void LoadCampfire()
    {
        GameManager.instance.LoadProgress();
        ResumeGame();
    }

    // Will set game over mode if disabled
    public void IsPauseMode(bool isEnabled)
    {
        Button button = buttons.FirstOrDefault(x => x.name == "ResumeButton");

        button.gameObject.SetActive(isEnabled);

        menuTitle.text = isEnabled ? "Paused" : "Game Over";
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
