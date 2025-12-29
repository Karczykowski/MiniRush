using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public GameObject tooltipObject;
    public TextMeshProUGUI tooltipText;

    public Vector2 offset = new Vector2(15f, -15f);
    private bool isActive;

    private void Awake()
    {
        Instance = this;
        tooltipObject.SetActive(false);
        isActive = false;
    }

    private void Update()
    {
        if (isActive)
        {
            Vector2 position = (Vector2)Input.mousePosition + offset;
            tooltipObject.transform.position = position;
        }
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        isActive = true;
        tooltipObject.SetActive(true);
    }

    public void Hide()
    {
        isActive = false;
        tooltipObject.SetActive(false);
    }
}
