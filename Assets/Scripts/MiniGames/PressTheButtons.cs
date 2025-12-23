using System.Collections;
using UnityEngine;
using TMPro;

public class PressTheButtons : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float baseGameTime = 5.0f;
    [SerializeField] private float gameSpeed = 1.0f;
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
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T
    };

    void Start()
    {
        effectiveWaitTime = baseGameTime / gameSpeed;
        effectiveWaitTimeForKey = effectiveWaitTime / numberOfKeys;
        points = 0;
        timer = effectiveWaitTimeForKey;
        currentKey = GetNewKey();

        gameplayCoroutine = StartCoroutine(Run());
        text.SetText($"Klawisz: {currentKey}");
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
            StopCoroutine(gameplayCoroutine);
        }
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(currentKey))
            {
                Debug.Log("+1");
                timer = effectiveWaitTimeForKey;
                currentKey = GetNewKey();
                text.SetText($"Klawisz: {currentKey}");
                points++;

                if (points >= numberOfKeys)
                {
                    FinishGame(true);
                    StopCoroutine(gameplayCoroutine);
                }
            }
            else
            {
                FinishGame(false);
                StopCoroutine(gameplayCoroutine);
            }
        }
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(effectiveWaitTime);
        Debug.Log("Przegrana!");
        text.SetText("Przegrana");
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

    private void FinishGame(bool win)
    {
        if (win)
        {
            Debug.Log("Wygrana!");
            text.SetText("Wygrana");
            state = State.Success;
            //UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(0, 5));
        }
        else
        {
            Debug.Log("Przegrana!");
            text.SetText("Przegrana");
            state = State.Fail;
            //UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(0, 5));
        }
    }
}
