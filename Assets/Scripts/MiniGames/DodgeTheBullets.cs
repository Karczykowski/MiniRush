#pragma warning disable CS0414
using System.Collections;
using UnityEngine;
using TMPro;

public class DodgeTheBullets : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float baseGameTime = 10.0f;
    [SerializeField] private float gameSpeed = 1.0f;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private float bulletSpawnDelay = 0.5f;
    [SerializeField] private GameObject player;

    private enum State { Play, Success, Fail }
    private State state = State.Play;

    private float areaMinX, areaMaxX, areaMinY, areaMaxY;
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
            yield return new WaitForSeconds(bulletSpawnDelay);
        }
    }

    private void SpawnBullet()
    {
        float camHeight = camHalfHeight * 2f;
        float camWidth = camHalfWidth * 2f;
        Debug.Log("Spawned Bullet");
    }
}
