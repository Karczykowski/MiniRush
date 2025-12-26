using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject collectionCanvas;
    [SerializeField] GameObject creditsCanvas;

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(1, 7));
    }

    public void EnableMainCanvas()
    {
        mainCanvas.SetActive(true);
        collectionCanvas.SetActive(false);
        creditsCanvas.SetActive(false);

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

    public void ExitGame()
    {
        Application.Quit();
    }
}
 