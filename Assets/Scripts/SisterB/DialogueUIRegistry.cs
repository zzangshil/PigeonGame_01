using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIRegistry : MonoBehaviour
{
    public static DialogueUIRegistry Instance;

    [System.Serializable]
    public class UIEntry
    {
        public string id;
        public Image image;
    }

    public List<UIEntry> uiElements = new List<UIEntry>();

    private Dictionary<string, Image> lookup;

    void Awake()
    {
        Instance = this;

        lookup = new Dictionary<string, Image>();

        foreach (var entry in uiElements)
        {
            if (entry.image != null && !lookup.ContainsKey(entry.id))
            {
                lookup.Add(entry.id, entry.image);
            }
        }
    }

    public Image Get(string id)
    {
        if (lookup == null) return null;

        lookup.TryGetValue(id, out Image img);
        return img;
    }
}