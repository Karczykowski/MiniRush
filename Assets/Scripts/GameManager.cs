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
    public int currentChapter = 1;
    public int gamesWonInChapter = 0;

    public int[] chapter1Games = new int[]
    {
        1, 2 , 3
    };

    public int[] chapter2Games = new int[]
    {
        2, 3 , 4
    };

    public int[] chapter3Games = new int[]
    {
        3, 4 , 5
    };

    public int[] chapter4Games = new int[]
    {
        4, 5 , 6
    };

    public int[] chapter5Games = new int[]
    {
        5, 6 , 1
    };

    public float[] chapterSpeeds = new float[]
    {
        1f, 1.1f, 1.2f, 1.3f, 1.4f
    };

    public int[] chaptersLengths = new int[]
    {
        5, 6, 7, 8, 10
    };

    public bool[] areChaptersUnlocked = new bool[]
    {
        true, false, false, false, false
    };

    public int menuState = 0;
    // 1 - chapter selection
    // 2 - win game

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
        gamesWonInChapter++;
        if(gamesWonInChapter >= chaptersLengths[currentChapter - 1])
        {
            GoToNextChapter();
        }
        GameSpeed += speedIncrease;
        LoadRandomMiniGame();
    }

    public void GoToNextChapter()
    {
        SetGameSpeed(chapterSpeeds[currentChapter - 1]);
        currentChapter++;
        if(areChaptersUnlocked[currentChapter - 1] == false)
        {
            areChaptersUnlocked[currentChapter - 1] = true;
        }
        gamesWonInChapter = 0;
        if (currentChapter > chaptersLengths.Length)
        {
            SceneManager.LoadScene("MainMenu");
            menuState = 2;
            return;
        }
        SetChapter(currentChapter);
    }

    public void SetGameSpeed(float speed)
    {
        GameSpeed = speed;
    }

    public void SetChapter(int chapter)
    {
        currentChapter = chapter;
        GameSpeed = chapterSpeeds[chapter - 1];
    }

    public void LoadRandomMiniGame()
    {
        int nextSceneIndex;
        do
        {
            nextSceneIndex = GetRandomGameIndexFromChapter(currentChapter);
        } while (nextSceneIndex == lastSceneIndex);

        lastSceneIndex = nextSceneIndex;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public int GetRandomGameIndexFromChapter(int chapter)
    {
        int index = 0;
        switch (chapter)
        {
            case 1:
                index = chapter1Games[Random.Range(0, GameManager.Instance.chapter1Games.Length)];
                break;

            case 2:
                index = chapter2Games[Random.Range(0, GameManager.Instance.chapter2Games.Length)];
                break;

            case 3:
                index = chapter3Games[Random.Range(0, GameManager.Instance.chapter3Games.Length)];
                break;

            case 4:
                index = chapter4Games[Random.Range(0, GameManager.Instance.chapter4Games.Length)];
                break;

            case 5:
                index = chapter5Games[Random.Range(0, GameManager.Instance.chapter5Games.Length)];
                break;

            default:
                Debug.Log("Blad, brak podanego poziomu");
                break;
        }
        return index;
    }
}
