#pragma warning disable CS0414
using System.Collections;
using UnityEngine;
using TMPro;

public class Pong : MonoBehaviour
{
    protected float gameSpeed = 1.0f;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject boundry;

    [SerializeField] private GameObject[] images;
    [SerializeField] private float baseGameTime = 5.0f;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float ballSpeed = 3.0f;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ball;

    private enum State { Play, Success, Fail }
    private State state = State.Play;

    private float areaMinX, areaMaxX;
    float camHalfHeight;
    float camHalfWidth;
    private float effectiveWaitTime = 5f;
    private Coroutine gameplayCoroutine;
    private Coroutine fadeTextCoroutine;
    private Coroutine fadeImagesCoroutine;
    private int bounceCount = 0;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            gameSpeed = GameManager.Instance.GameSpeed;
        }

        boundry.AddComponent<BottomTrigger>().Init(this);

        effectiveWaitTime = baseGameTime / gameSpeed;

        camHalfHeight = Camera.main.orthographicSize;
        camHalfWidth = camHalfHeight * Camera.main.aspect;

        SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
        float playerHalfWidth = playerRenderer.bounds.size.x / 2;
        float playerHalfHeight = playerRenderer.bounds.size.y / 2;

        areaMinX = Camera.main.transform.position.x - camHalfWidth + playerHalfWidth;
        areaMaxX = Camera.main.transform.position.x + camHalfWidth - playerHalfWidth;

        gameplayCoroutine = StartCoroutine(Run());
        fadeImagesCoroutine = StartCoroutine(FadeImage());

        Vector2 dir = new Vector2(Random.value < 0.5f ? -1f : 1f, Random.Range(0.2f, 1f)).normalized;
        rb.linearVelocity = dir * gameSpeed;
        ball.AddComponent<BallCollisionProxy>().Init(this);
    }

    void Update()
    {
        if (state != State.Play)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = rb.linearVelocity.normalized * ballSpeed * gameSpeed;
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

    public void OnBounce()
    {
        bounceCount++;
        if (bounceCount >= 4)
        {
            CollectionManager.Instance.UnlockItem("p1");
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
        FinishGame(true);
    }

    private IEnumerator DelayedLevelUp()
    {
        yield return new WaitForSeconds(GameManager.Instance.winTimeDelay);
        GameManager.Instance.OnMiniGameWin();
    }

    private IEnumerator DelayedReturnToMenu()
    {
        yield return new WaitForSeconds(GameManager.Instance.loseTimeDelay);
        Debug.Log("Powrót do menu");
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
            if(bounceCount == 1)
            {
                CollectionManager.Instance.UnlockItem("p2");
            }
            StartCoroutine(DelayedLevelUp());
            CollectionManager.Instance.UnlockItem("u4");

        }
        else
        {
            Debug.Log("Przegrana!");
            text.SetText("Przegrana");
            state = State.Fail;
            StartCoroutine(DelayedReturnToMenu());
        }
    }

    private class BottomTrigger : MonoBehaviour
    {
        private Pong pongGame;
        public void Init(Pong game)
        {
            pongGame = game;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            pongGame.FinishGame(false);
        }
    }

    private class BallCollisionProxy : MonoBehaviour
    {
        private Pong pongGame;
        public void Init(Pong game)
        {
            pongGame = game;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                pongGame.OnBounce();
            }
        }
    }
}
