using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float GameSpeed { get; private set; } = 1.0f;
    public float speedIncrease = 0.1f;
    public float textFadeTime = 1.0f;
    public float imageFadeTime = 1.0f;
    public float loseTimeDelay = 2.0f;
    public float winTimeDelay = 2.0f;
    int lastSceneIndex = -1;

    private void Awake()
    {
        if  (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  
    }

    public void OnMiniGameWin()
    {
        GameSpeed += speedIncrease;
        LoadRandomMiniGame();
    }

    public void LoadRandomMiniGame()
    {
        int nextSceneIndex;
        do
        {
            nextSceneIndex = Random.Range(1, SceneManager.sceneCountInBuildSettings);
        } while (nextSceneIndex == lastSceneIndex);

        lastSceneIndex = nextSceneIndex;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
