#pragma warning disable CS0414
using System.Collections;
using UnityEngine;
using TMPro;

public class MiniGameTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float baseGameTime = 5.0f;
    [SerializeField] private float gameSpeed = 1.0f;

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

    private void FinishGame(bool win)
    {
        if (win)
        {
            Debug.Log("Wygrana!");
            text.SetText("Wygrana");
            state = State.Success;
            UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(0, 5));
        }
        else
        {
            Debug.Log("Przegrana!");
            text.SetText("Przegrana");
            state = State.Fail;
        }
    }
}
