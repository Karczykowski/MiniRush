using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] string gamesScene;
    [SerializeField] string freePlayScene;
    [SerializeField] string collectionScene;
    [SerializeField] string optionsScene;
    public void StartGame()
    {
        SceneManager.LoadScene(gamesScene);
    }

    public void FreePlay()
    {
        SceneManager.LoadScene(freePlayScene);
    }

    public void Collection()
    {
        SceneManager.LoadScene(collectionScene);
    }

    public void Options()
    {
        SceneManager.LoadScene(optionsScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
