using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float GameSpeed { get; private set; } = 1.0f;
    public float speedIncrease = 0.1f;

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
    }
}
