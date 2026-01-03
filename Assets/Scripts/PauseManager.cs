using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] GameObject pauseMenu;
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

    public bool isPaused { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if(Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePause();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitToMenu()
    {
        Resume();
        GameManager.Instance.menuState = 0;
        SceneManager.LoadScene("MainMenu");
    }
    
}

