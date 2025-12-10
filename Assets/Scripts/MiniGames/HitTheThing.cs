using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitTheThing : MonoBehaviour
{
    [SerializeField] private GameObject dartboardGameObject;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float baseGameTime = 5.0f;
    [SerializeField] private float gameSpeed = 1.0f;
    [SerializeField] private float dartboardSpeed = 1250.0f;

    private enum State { Play, Success, Fail }
    private State state = State.Play;
    private Vector2 direction;

    private float areaMinX, areaMaxX, areaMinY, areaMaxY;
    private float effectiveWaitTime = 5f;
    private Coroutine gameplayCoroutine;

    void Start()
    {
        effectiveWaitTime = baseGameTime / gameSpeed;

        direction = Random.insideUnitCircle.normalized;

        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        SpriteRenderer dartboardSpriteRenderer = dartboardGameObject.GetComponent<SpriteRenderer>();
        float dartboardHalfWidth = dartboardSpriteRenderer.bounds.size.x / 2;
        float dartboardHalfHeight = dartboardSpriteRenderer.bounds.size.y / 2;

        areaMinX = Camera.main.transform.position.x - camHalfWidth + dartboardHalfWidth;
        areaMaxX = Camera.main.transform.position.x + camHalfWidth - dartboardHalfWidth;
        areaMinY = Camera.main.transform.position.y - camHalfHeight + dartboardHalfHeight;
        areaMaxY = Camera.main.transform.position.y + camHalfHeight - dartboardHalfHeight;

        gameplayCoroutine = StartCoroutine(Run());
    }

    void Update()
    {
        if(state != State.Play)
        {
            return;
        }

        Vector3 position = dartboardGameObject.transform.position;
        position += (Vector3)(dartboardSpeed * direction * Time.deltaTime * gameSpeed);

        if (position.x < areaMinX)
        {
            position.x = areaMinX;
            direction.x = -direction.x;
        }
        else if (position.x > areaMaxX)
        {
            position.x = areaMaxX;
            direction.x = -direction.x;
        }
        if (position.y < areaMinY)
        {
            position.y = areaMinY;
            direction.y = -direction.y;
        }
        else if (position.y > areaMaxY)
        {
            position.y = areaMaxY;
            direction.y = -direction.y;
        }

        dartboardGameObject.transform.position = position;

        if (Input.GetMouseButtonDown(0))
        {
            Shoot(Input.mousePosition);
        }
    }

    public void Shoot(Vector2 screenPos)
    {
        if (state != State.Play) {
            return;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point2D = new Vector2(worldPos.x, worldPos.y);

        Collider2D hit = Physics2D.OverlapPoint(point2D);

        if (hit == null)
        {
            FinishGame(false);
            StopCoroutine(gameplayCoroutine);
        }
        else if(hit.gameObject == dartboardGameObject)
        {
            FinishGame(true);
            StopCoroutine(gameplayCoroutine);
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
