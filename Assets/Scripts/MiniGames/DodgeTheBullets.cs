#pragma warning disable CS0414
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DodgeTheBullets : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    protected float gameSpeed = 1.0f;

    [SerializeField] private GameObject[] images;
    [SerializeField] private float baseGameTime = 10.0f;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private float baseBulletSpawnDelay = 0.5f;
    [SerializeField] private GameObject player;

    private enum State { Play, Success, Fail }
    private State state = State.Play;

    private float areaMinX, areaMaxX, areaMinY, areaMaxY;
    float camHalfHeight;
    float camHalfWidth;
    private float effectiveWaitTime = 5f;
    private float effectiveBulletSpawnDelay = 0.5f;
    private float standTimer = 0f;
    private Vector3 lastPosition;
    private Coroutine gameplayCoroutine;
    private Coroutine bulletCoroutine;
    private Coroutine fadeTextCoroutine;
    private Coroutine fadeImagesCoroutine;
    private List<GameObject> bullets = new List<GameObject>();

    void Start()
    {
        if (GameManager.Instance != null)
        {
            gameSpeed = GameManager.Instance.GameSpeed;
        }
        player.AddComponent<PlayerTrigger>().Init(this);

        effectiveWaitTime = baseGameTime / gameSpeed;
        effectiveBulletSpawnDelay = baseBulletSpawnDelay / gameSpeed;

        camHalfHeight = Camera.main.orthographicSize;
        camHalfWidth = camHalfHeight * Camera.main.aspect;

        SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
        float playerHalfWidth = playerRenderer.bounds.size.x / 2;
        float playerHalfHeight = playerRenderer.bounds.size.y / 2;

        areaMinX = Camera.main.transform.position.x - camHalfWidth + playerHalfWidth;
        areaMaxX = Camera.main.transform.position.x + camHalfWidth - playerHalfWidth;
        areaMinY = Camera.main.transform.position.y - camHalfHeight + playerHalfHeight;
        areaMaxY = Camera.main.transform.position.y + camHalfHeight - playerHalfHeight;

        gameplayCoroutine = StartCoroutine(Run());
        bulletCoroutine = StartCoroutine(SpawnBullets());
        fadeImagesCoroutine = StartCoroutine(FadeImage());
    }

    void Update()
    {
        if (state != State.Play)
        {
            return;
        }

        Movement();
        CheckIdle();
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
        DestroyAllBullets();
    }

    private void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 position = player.transform.position;

        float beforeClampY = position.y;

        position += new Vector3(moveX, moveY, 0f).normalized * playerSpeed * gameSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, areaMinX, areaMaxX);
        position.y = Mathf.Clamp(position.y, areaMinY, areaMaxY);

        if (position.y != beforeClampY)
        {
            CollectionManager.Instance.UnlockItem("dtb1");
        }

        player.transform.position = position;
    }

    private void CheckIdle() //for achievement
    {
        Vector3 position = player.transform.position;

        if (position == lastPosition)
        {
            standTimer += Time.deltaTime;
            if (standTimer >= 3f)
            {
                CollectionManager.Instance.UnlockItem("dtb2");
            }
        }
        else
        {
            standTimer = 0f;
            lastPosition = position;
        }
    }

    private IEnumerator SpawnBullets()
    {
        while (state == State.Play)
        {
            SpawnBullet();
            yield return new WaitForSeconds(effectiveBulletSpawnDelay);
        }
    }

    private void SpawnBullet()
    {
        int side = Random.Range(0, 4);
        Vector2 spawnPoint = Vector2.zero;
        Vector2 direction = Vector2.zero;

        if (side == 0)
        {
            spawnPoint = new Vector2(-camHalfWidth, Random.Range(-camHalfHeight, camHalfHeight));
            direction = Vector2.right;
        }
        else if (side == 1)
        {
            spawnPoint = new Vector2(camHalfWidth, Random.Range(-camHalfHeight, camHalfHeight));
            direction = Vector2.left;
        }
        else if (side == 2)
        {
            spawnPoint = new Vector2(Random.Range(-camHalfHeight * 2, camHalfHeight * 2), camHalfHeight);
            direction = Vector2.down;
        }
        else if (side == 3)
        {
            spawnPoint = new Vector2(Random.Range(-camHalfHeight * 2, camHalfHeight * 2), -camHalfHeight);
            direction = Vector2.up;
        }

        GameObject bullet = Instantiate(bulletPrefab, spawnPoint, Quaternion.identity);

        bullet.GetComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed * gameSpeed;

        bullets.Add(bullet);
    }

    private void DestroyAllBullets()
    {
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        bullets.Clear();
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

    private void LoseGame()
    {
        FinishGame(false);
        DestroyAllBullets();
    }

    private void FinishGame(bool win)
    {
        StopCoroutine(gameplayCoroutine);
        StopCoroutine(fadeImagesCoroutine);
        if (win)
        {
            text.SetText("Wygrana!");
            state = State.Success;
            StartCoroutine(DelayedLevelUp());
            CollectionManager.Instance.UnlockItem("u2");
        }
        else
        {
            StopCoroutine(bulletCoroutine);
            text.SetText("Przegrana");
            state = State.Fail;
            StartCoroutine(DelayedReturnToMenu());
        }
    }

    private class PlayerTrigger : MonoBehaviour
    {
        private DodgeTheBullets miniGame;

        public void Init(DodgeTheBullets game)
        {
            miniGame = game;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bullet"))
            {
                miniGame.LoseGame();
            }
        }
    }
}

