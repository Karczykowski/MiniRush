using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject collectionCanvas;
    [SerializeField] GameObject creditsCanvas;
    [SerializeField] GameObject chapterCanvas;
    [SerializeField] GameObject winCanvas;
    [SerializeField] List<GameObject> chapterButtons = new List<GameObject>();

    private void OnEnable()
    {
        switch(GameManager.Instance.menuState)
        {
            case 0:
                EnableMainCanvas();
                break;
            case 1:
                EnableChapterCanvas();
                break;
            case 2:
                EnableWinCanvas();
                break;
        }
    }

    public void EnableMainCanvas()
    {
        mainCanvas.SetActive(true);
        collectionCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        chapterCanvas.SetActive(false);
    }

    public void EnableCollectionCanvas()
    {
        mainCanvas.SetActive(false);
        collectionCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
    }

    public void EnableCreditsCanvas()
    {
        mainCanvas.SetActive(false);
        collectionCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void EnableChapterCanvas()
    {
        mainCanvas.SetActive(false);
        chapterCanvas.SetActive(true);
        SetChapterButton();
    }

    public void EnableWinCanvas()
    {
        mainCanvas.SetActive(false);
        winCanvas.SetActive(true);
    }

    public void StartGame(int chapter)
    {
        GameManager.Instance.SetChapter(chapter);
        GameManager.Instance.LoadRandomMiniGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetChapterButton()
    {
        int buttonIndex = 0;
        foreach(GameObject button in chapterButtons)
        {
            if (GameManager.Instance.areChaptersUnlocked[buttonIndex])
            {
                button.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            else
            {
                button.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
            buttonIndex++;
        }
    }
}
 