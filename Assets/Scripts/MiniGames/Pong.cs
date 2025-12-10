#pragma warning disable CS0414
using System.Collections;
using UnityEngine;
using TMPro;

public class Pong : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float baseGameTime = 5.0f;
    [SerializeField] private float gameSpeed = 1.0f;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private GameObject player;

    private enum State { Play, Success, Fail }
    private State state = State.Play;

    private float areaMinX, areaMaxX;
    float camHalfHeight;
    float camHalfWidth;
    private float effectiveWaitTime = 5f;
    private Coroutine gameplayCoroutine;

    void Start()
    {
        effectiveWaitTime = baseGameTime / gameSpeed;

        camHalfHeight = Camera.main.orthographicSize;
        camHalfWidth = camHalfHeight * Camera.main.aspect;

        SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
        float playerHalfWidth = playerRenderer.bounds.size.x / 2;
        float playerHalfHeight = playerRenderer.bounds.size.y / 2;

        areaMinX = Camera.main.transform.position.x - camHalfWidth + playerHalfWidth;
        areaMaxX = Camera.main.transform.position.x + camHalfWidth - playerHalfWidth;

        gameplayCoroutine = StartCoroutine(Run());
    }

    void Update()
    {
        if (state != State.Play)
        {
            return;
        }

        Movement();
    }

    private void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        Vector3 position = player.transform.position;
        position += new Vector3(moveX, 0f, 0f).normalized * playerSpeed * gameSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, areaMinX, areaMaxX);

        player.transform.position = position;
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(effectiveWaitTime);
        FinishGame(true);
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
