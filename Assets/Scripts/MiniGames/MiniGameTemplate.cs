#pragma warning disable CS0414
using System.Collections;
using UnityEngine;
using TMPro;

public class MiniGameTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float baseGameTime = 5.0f;
    [SerializeField] private float gameSpeed = 1.0f;
    [SerializeField] private float loseTimeDelay = 2.0f;
    [SerializeField] private float winTimeDelay = 2.0f;

    private enum State { Play, Success, Fail }
    private State state = State.Play;

    private float effectiveWaitTime = 5f;
    private Coroutine gameplayCoroutine;

    void Start()
    {
        effectiveWaitTime = baseGameTime / gameSpeed;

        gameplayCoroutine = StartCoroutine(Run());
    }

    void Update()
    {
        if (state != State.Play)
        {
            return;
        }
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(effectiveWaitTime);
        FinishGame(false);
    }

    private IEnumerator DelayedLevelUp()
    {
        yield return new WaitForSeconds(winTimeDelay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(1, 6));
    }

    private IEnumerator DelayedReturnToMenu()
    {
        yield return new WaitForSeconds(loseTimeDelay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void FinishGame(bool win)
    {
        StopCoroutine(gameplayCoroutine);
        if (win)
        {
            Debug.Log("Wygrana!");
            text.SetText("Wygrana");
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
