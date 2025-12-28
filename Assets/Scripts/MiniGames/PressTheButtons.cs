using System.Collections;
using UnityEngine;
using TMPro;

public class PressTheButtons : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI winText;
    protected float gameSpeed;

    [SerializeField] private float baseGameTime = 5.0f;
    [SerializeField] private float numberOfKeys = 4;

    private enum State { Play, Success, Fail }
    private State state = State.Play;

    private float effectiveWaitTime = 5f;
    private float effectiveWaitTimeForKey = 1f;
    private int points;
    private float timer;
    private KeyCode currentKey = KeyCode.Q;
    private Coroutine gameplayCoroutine;

    private KeyCode[] keys = new KeyCode[]
    {
        KeyCode.Q, KeyCode.W, KeyCode.E
    };

    void Start()
    {
        if (GameManager.Instance != null)
        {
            gameSpeed = GameManager.Instance.GameSpeed;
        }
        effectiveWaitTime = baseGameTime / gameSpeed;
        effectiveWaitTimeForKey = effectiveWaitTime / numberOfKeys;
        points = 0;
        timer = effectiveWaitTimeForKey;
        currentKey = GetNewKey();

        gameplayCoroutine = StartCoroutine(Run());
        text.SetText($"{currentKey}");
    }

    void Update()
    {
        if (state != State.Play)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            FinishGame(false);
        }
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(currentKey))
            {
                Debug.Log("+1");
                timer = effectiveWaitTimeForKey;
                currentKey = GetNewKey();
                text.SetText($"{currentKey}");
                points++;

                if (points >= numberOfKeys)
                {
                    FinishGame(true);
                }
            }
            else
            {
                FinishGame(false);
            }
        }
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(effectiveWaitTime);
        Debug.Log("Przegrana!");
        winText.SetText("Przegrana");
        state = State.Fail;
    }

    private KeyCode GetNewKey()
    {
        KeyCode previousKey = currentKey;
        KeyCode newKey = keys[Random.Range(0, keys.Length)];
        while(newKey == previousKey)
        {
            newKey = keys[Random.Range(0, keys.Length)];
        }
        return newKey;
    }

    private IEnumerator DelayedLevelUp()
    {
        yield return new WaitForSeconds(GameManager.Instance.winTimeDelay);
        GameManager.Instance.OnMiniGameWin();
    }

    private IEnumerator DelayedReturnToMenu()
    {
        yield return new WaitForSeconds(GameManager.Instance.loseTimeDelay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void FinishGame(bool win)
    {
        StopCoroutine(gameplayCoroutine);
        if (win)
        {
            Debug.Log("Wygrana!");
            winText.SetText("Wygrana");
            state = State.Success;
            StartCoroutine(DelayedLevelUp());
        }
        else
        {
            Debug.Log("Przegrana!");
            winText.SetText("Przegrana");
            state = State.Fail;
            StartCoroutine(DelayedReturnToMenu());
        }
    }
}
