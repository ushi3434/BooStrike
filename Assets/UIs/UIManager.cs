using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public enum SCREEN
    {
        GAME,
        PAUSE,
        GUIDE,
        DEAD,
        GOAL,
    };

    [SerializeField] AudioManager audioManager;

    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject guideUI;
    [SerializeField] GameObject deadMenuUI;
    [SerializeField] GameObject goalMenuUI;

    public static SCREEN showScreen;

    private void Start()
    {
        showScreen = SCREEN.GAME;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (showScreen)
            {
                case SCREEN.GAME:
                    Pause();
                    break;

                case SCREEN.PAUSE:
                    Resume();
                    break;

                case SCREEN.GUIDE:
                    GuideToPause();
                    break;

                default:
                    break;
            }

        }
    }

    public void Resume()
    {
        //BGM音量大きくする
        audioManager.BGM.volume = 0.2f;

        //カーソル開放
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        showScreen = SCREEN.GAME;
    }

    public void Pause()
    {
        //BGM音量小さくする
        audioManager.BGM.volume = 0.1f;

        //カーソルロック
        Cursor.lockState = CursorLockMode.None;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        showScreen = SCREEN.PAUSE;
    }

    public void PauseToGuide()
    {
        pauseMenuUI.SetActive(false);
        guideUI.SetActive(true);
        showScreen = SCREEN.GUIDE;
    }

    public void GuideToPause()
    {
        guideUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        showScreen = SCREEN.PAUSE;
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void ShowDeadMenu()
    {
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;

        audioManager.StopBGM();

        //効果音
        audioManager.PlayDeadSound();

        deadMenuUI.SetActive(true);

        deadMenuUI.GetComponent<ShakeUI>().StartShake();
        showScreen = SCREEN.DEAD;
    }

    public void ShowGoalMenu()
    {
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
        goalMenuUI.SetActive(true);

        goalMenuUI.GetComponent<ShakeUI>().StartShake();
        showScreen = SCREEN.GOAL;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public SCREEN GetShownScreen()
    {
        return showScreen;
    }
}