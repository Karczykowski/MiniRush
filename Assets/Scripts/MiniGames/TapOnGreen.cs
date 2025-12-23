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

    [SerializeField] private float baseWaitTime = 5.0f;
    [SerializeField] private float waitTimeVariation = 0.3f;
    [SerializeField] private float gameSpeed = 1.0f;
    [SerializeField] private float reactionTime = 1.0f;
    [SerializeField] private float colorChangeIntensity = 0.2f;

    private enum State { Waiting, ReactionTime, Success, Fail }
    private State state = State.Waiting;
    private float effectiveWaitTime = 5f;
    private float minEffectiveTime = 1f;
    private float maxEffectiveTime = 1f;
    private Coroutine gameplayCoroutine;

    void Start()
    {
        minEffectiveTime = (1f - waitTimeVariation) * baseWaitTime;
        maxEffectiveTime = (1f + waitTimeVariation) * baseWaitTime;
        effectiveWaitTime = Random.Range(minEffectiveTime, maxEffectiveTime) / gameSpeed;
        Debug.Log("Effective wait time for this round: " + effectiveWaitTime);
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

        if (!press) return;

        if (state == State.Waiting)
        {
            Debug.Log("Przegrana!");
            text.SetText("Przegrana");
            state = State.Fail;
            UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(0, 5));
            StopCoroutine(gameplayCoroutine);
        }
        else if (state == State.ReactionTime)
        {
            FinishGame(true);
            StopCoroutine(gameplayCoroutine);
        }
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(effectiveWaitTime);
        state = State.ReactionTime;
        yield return new WaitForSeconds(reactionTime);
        FinishGame(false);
    }

    private void FinishGame(bool win)
    {
        if(win)
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
