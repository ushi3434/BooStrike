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
        STAGESELECT,
    }

    [SerializeField] private GameObject TitleUI;
    [SerializeField] private GameObject GuideUI;
    [SerializeField] private GameObject CreditUI;
    [SerializeField] private GameObject StageSelectUI;

    private TITLE_SCREEN shownScreen;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (shownScreen)
            {
                case TITLE_SCREEN.GUIDE:
                    GuideToTitle();
                    break;

                case TITLE_SCREEN.CREDIT:
                    CreditToTitle();
                    break;

                case TITLE_SCREEN.STAGESELECT:

                    break;

                default:
                    break;
            }
        }
    }

    public void TitleToGuide()
    {
        TitleUI.SetActive(false);
        GuideUI.SetActive(true);
        shownScreen = TITLE_SCREEN.GUIDE;

    }

    public void GuideToTitle()
    {
        GuideUI.SetActive(false);
        TitleUI.SetActive(true);
        shownScreen = TITLE_SCREEN.TOP;

    }

    public void TitleToCredit()
    {
        TitleUI.SetActive(false);
        CreditUI.SetActive(true);
        shownScreen = TITLE_SCREEN.CREDIT;

    }

    public void CreditToTitle()
    {
        CreditUI.SetActive(false);
        TitleUI.SetActive(true);
        shownScreen = TITLE_SCREEN.TOP;

    }

    public void GameStart()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void TitleToSelect()
    {
        TitleUI.SetActive(false);
        StageSelectUI.SetActive(true);
        shownScreen = TITLE_SCREEN.STAGESELECT;

    }

    public void SelectToTitle()
    {
        StageSelectUI.SetActive(false);
        TitleUI.SetActive(true);
        shownScreen = TITLE_SCREEN.TOP;

    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
