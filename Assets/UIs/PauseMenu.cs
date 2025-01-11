using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject guideUI;

    public static bool gameisPaused = false;
    public static bool showGuide = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (showGuide)
            {
                GuideToPause();
            }
            else
            {
                if (gameisPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }

        }
    }

    public void Resume()
    {
        //カーソル開放
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameisPaused = false;
    }

    public void Pause()
    {
        //カーソルロック
        Cursor.lockState = CursorLockMode.None;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameisPaused = true;
    }

    public void PauseToGuide()
    {
        pauseMenuUI.SetActive(false);
        guideUI.SetActive(true);
        showGuide = true;
    }

    public void GuideToPause()
    {
        guideUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        showGuide = false;
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public bool GetGameIsPaused()
    {
        return gameisPaused;
    }

}