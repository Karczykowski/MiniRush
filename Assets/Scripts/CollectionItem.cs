using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CollectionIcon : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public string id;
    public Image iconImage;
    [SerializeField] private float lockedTransparency = 0.25f;
    public bool isUnlocked;
    [TextArea]
    public string tooltipDescription;

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        Color color = iconImage.color;
        color.a = unlocked ? 1.0f : lockedTransparency;
        iconImage.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Instance.Show(tooltipDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.Hide();
    }
}
