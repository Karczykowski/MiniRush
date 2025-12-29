#pragma warning disable CS0414
using System.Collections;
using UnityEngine;
using TMPro;

public class MiniGameTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    protected float gameSpeed = 1.0f;

    [SerializeField] private GameObject[] images;
    [SerializeField] private float baseGameTime = 5.0f;

    private enum State { Play, Success, Fail }
    private State state = State.Play;

    private float effectiveWaitTime = 5f;
    private Coroutine gameplayCoroutine;
    private Coroutine fadeImagesCoroutine;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            gameSpeed = GameManager.Instance.GameSpeed;
        }
        effectiveWaitTime = baseGameTime / gameSpeed;

        gameplayCoroutine = StartCoroutine(Run());
        fadeImagesCoroutine = StartCoroutine(FadeImage());
    }

    void Update()
    {
        if (state != State.Play)
        {
            return;
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

    private IEnumerator FadeText()
    {
        yield return new WaitForSeconds(GameManager.Instance.textFadeTime);
        text.SetText("");
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(effectiveWaitTime);
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void FinishGame(bool win)
    {
        StopCoroutine(gameplayCoroutine);
        StopCoroutine(fadeImagesCoroutine);
        if (win)
        {
            Debug.Log("Wygrana!");
            text.SetText("Przyœpieszamy!");
            state = State.Success;
            StartCoroutine(DelayedLevelUp());
        }
        else
        {
            Debug.Log("Przegrana!");
            text.SetText("Przegrana");
            state = State.Fail;
            StartCoroutine(DelayedReturnToMenu());
        }
    }
}
