using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private Canvas canvas;

    public void Awake()
    {
        canvas = GetComponent<Canvas>();
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
            PauseGame();
        }
    }

    public void LoadMainMenu() => GameManager.instance.LoadMainMenu();

    public void PauseGame()
    {
        GameManager.instance.isGamePaused = true;
        canvas.enabled = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        GameManager.instance.isGamePaused = false;
        Time.timeScale = 1;
        canvas.enabled = false;
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
