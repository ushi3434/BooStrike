using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private GameObject TitleUI;
    [SerializeField] private GameObject GuideUI;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;

    }

    public void TitleToGuide()
    {
        TitleUI.SetActive(false);
        GuideUI.SetActive(true);

    }

    public void GuideToTitle()
    {
        GuideUI.SetActive(false);
        TitleUI.SetActive(true);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
