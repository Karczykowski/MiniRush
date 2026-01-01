using System.Collections.Generic;
using UnityEngine;

public class CollectionMenu : MonoBehaviour
{
    public List<CollectionIcon> icons = new List<CollectionIcon>();

    private void Awake()
    {
        if(icons.Count == 0)
        {
            icons.AddRange(GetComponentsInChildren<CollectionIcon>(true));
        }
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach(var icon in icons)
        {
            if (icon == null)
            {
                continue;
            }
            icon.SetUnlocked(CollectionManager.unlockedItems.Contains(icon.id));
        }
    }
}
