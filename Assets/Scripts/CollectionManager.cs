using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    public static HashSet<string> unlockedItems = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void UnlockItem(string id)
    {
        if (unlockedItems.Add(id))
        {
            Save();
        }
    }

    void Save()
    {
        PlayerPrefs.SetString("UnlockeddItems", string.Join(",", unlockedItems));
        PlayerPrefs.Save();
    }

    void Load()
    {
        string data = PlayerPrefs.GetString("UnlockeddItems", "");
        if(!string.IsNullOrEmpty(data))
        {
            foreach (string item in data.Split(','))
            {
                unlockedItems.Add(item);
            }
        }
    }

    public void clearPlayerPrefsForTesting()
    {
        PlayerPrefs.DeleteAll();
        unlockedItems.Clear();
    }
}
