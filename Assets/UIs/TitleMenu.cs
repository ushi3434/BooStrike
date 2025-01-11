using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    enum TITLE_SCREEN
    {
        TOP,
        GUIDE,
        CREDIT,
    }

    [SerializeField] private GameObject TitleUI;
    [SerializeField] private GameObject GuideUI;
    [SerializeField] private GameObject CreditUI;

    private TITLE_SCREEN shownScreen;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        switch (shownScreen)
        {
            case TITLE_SCREEN.GUIDE:
                GuideToTitle();
                break;

            case TITLE_SCREEN.CREDIT:
                CreditToTitle();
                break;

            default:
                break;
        }
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

    public void TitleToCredit()
    {
        TitleUI.SetActive(false);
        CreditUI.SetActive(true);
    }

    public void CreditToTitle()
    {
        CreditUI.SetActive(false);
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
