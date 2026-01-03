using System.Collections;
using UnityEngine;
using TMPro;

public class PressTheButtons : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI winText;
    protected float gameSpeed;

    [SerializeField] private GameObject[] images;
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
    private Coroutine fadeImagesCoroutine;

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
        fadeImagesCoroutine = StartCoroutine(FadeImage());
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CollectionManager.Instance.UnlockItem("ptb2");
            }
            if (Input.GetKeyDown(currentKey))
            {
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
                if(points == numberOfKeys - 1)
                {
                    CollectionManager.Instance.UnlockItem("ptb1");
                }
                FinishGame(false);
            }
        }
    }

    private IEnumerator FadeImage()
    {
        yield return new WaitForSeconds(GameManager.Instance.imageFadeTime);

        if(images.Length == 0)
        {
            yield break;
        }

        foreach (GameObject image in images)
        {
            if (image != null)
            {
                image.SetActive(false);
            }
        }
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(effectiveWaitTime);
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
        GameManager.Instance.menuState = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void FinishGame(bool win)
    {
        StopCoroutine(gameplayCoroutine);
        StopCoroutine(fadeImagesCoroutine);
        if (win)
        {
            winText.SetText("Wygrana!");
            state = State.Success;
            StartCoroutine(DelayedLevelUp());
            CollectionManager.Instance.UnlockItem("u5");
        }
        else
        {
            winText.SetText("Przegrana");
            state = State.Fail;
            StartCoroutine(DelayedReturnToMenu());
        }
    }
}
