#pragma warning disable CS0414
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DodgeTheBullets : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float baseGameTime = 10.0f;
    [SerializeField] private float gameSpeed = 1.0f;
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
    private Coroutine gameplayCoroutine;
    private List<GameObject> bullets = new List<GameObject>();

    void Start()
    {
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
        StartCoroutine(SpawnBullets());
    }

    void Update()
    {
        if (state != State.Play)
        {
            return;
        }

        Movement();
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(effectiveWaitTime);
        Debug.Log("Wygrana!");
        text.SetText("Wygrana");
        state = State.Success;
        DestroyAllBullets();
    }

    private void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 position = player.transform.position;
        position += new Vector3(moveX, moveY, 0f).normalized * playerSpeed * gameSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, areaMinX, areaMaxX);
        position.y = Mathf.Clamp(position.y, areaMinY, areaMaxY);

        player.transform.position = position;
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
            spawnPoint = new Vector2(Random.Range(-camHalfHeight, camHalfHeight), camHalfHeight);
            direction = Vector2.down;
        }
        else if (side == 3)
        {
            spawnPoint = new Vector2(Random.Range(-camHalfHeight, camHalfHeight), -camHalfHeight);
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
                Debug.Log("Przegrana!");
                miniGame.text.SetText("Przegrana");
                miniGame.state = State.Success;
                miniGame.DestroyAllBullets();
            }
        }
    }
}

