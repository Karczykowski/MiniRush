using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TapOnGreen : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color startColor = new Color(0.0f, 0.0f, 0.0f);
    [SerializeField] private Color clickColor = new Color(0.0f, 1.0f, 0.0f);
    protected float gameSpeed = 1.0f;

    [SerializeField] private GameObject[] images;
    [SerializeField] private float baseWaitTime = 5.0f;
    [SerializeField] private float waitTimeVariation = 0.3f;
    [SerializeField] private float reactionTime = 1.0f;
    [SerializeField] private float colorChangeIntensity = 0.2f;

    private enum State { Waiting, ReactionTime, Success, Fail }
    private State state = State.Waiting;
    private float effectiveWaitTime = 5f;
    private float minEffectiveTime = 1f;
    private float maxEffectiveTime = 1f;
    private Coroutine gameplayCoroutine;
    private Coroutine fadeImagesCoroutine;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            gameSpeed = GameManager.Instance.GameSpeed;
        }
        minEffectiveTime = (1f - waitTimeVariation) * baseWaitTime;
        maxEffectiveTime = (1f + waitTimeVariation) * baseWaitTime;
        effectiveWaitTime = Random.Range(minEffectiveTime, maxEffectiveTime) / gameSpeed;
        reactionTime = reactionTime / gameSpeed;

        gameplayCoroutine = StartCoroutine(Run());
    }

    void Update()
    {
        if(state == State.ReactionTime)
        {
            text.SetText("Klik!");
            background.color = clickColor;
        }

        // Color Rotation
        if(state == State.Waiting)
        {
            float pulse = (Mathf.Sin(Time.time * gameSpeed) + 1f) / 2f;
            float factor = 1f + (pulse - 0.5f) * 2f * colorChangeIntensity;
            background.color = startColor * factor;
        }

        bool press = Input.GetMouseButtonDown(0);

        if(Input.anyKeyDown &&
            !Input.GetMouseButtonDown(0) &&
            !Input.GetMouseButtonDown(1) &&
            !Input.GetMouseButtonDown(2))
        {
            CollectionManager.Instance.UnlockItem("tog2");
        }

        if (!press) return;

        if (state == State.Waiting)
        {
            CollectionManager.Instance.UnlockItem("tog1");
            FinishGame(false);
        }
        else if (state == State.ReactionTime)
        {
            FinishGame(true);
        }
    }

    private IEnumerator FadeImage()
    {
        yield return new WaitForSeconds(GameManager.Instance.imageFadeTime);

        if (images.Length == 0)
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
        state = State.ReactionTime;
        yield return new WaitForSeconds(reactionTime);
        FinishGame(false);
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
        if (win)
        {
            text.SetText("Wygrana!");
            state = State.Success;
            StartCoroutine(DelayedLevelUp());
            CollectionManager.Instance.UnlockItem("u6");
        }
        else
        {
            text.SetText("Przegrana");
            state = State.Fail;
            StartCoroutine(DelayedReturnToMenu());
        }
    }
}
