using System.Collections.Generic;
using UnityEngine;

public class CollectionMenu : MonoBehaviour
{
    public List<CollectionIcon> icons = new List<CollectionIcon>();
    private HashSet<string> unlockedItems = new HashSet<string>();

    private void Awake()
    {
        if(icons.Count == 0)
        {
            icons.AddRange(GetComponentsInChildren<CollectionIcon>(true));
        }
    }

    private void Start()
    {
        RefreshUI();
    }

    public void UnlockItem(string id)
    {
        unlockedItems.Add(id);
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
            icon.SetUnlocked(unlockedItems.Contains(icon.id));
        }
    }
}
