using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    public string text;

    public AudioClip voiceClip;
    public bool stopPreviousSound = true;
    public Sprite emotionSprite;

    [Header("UI Objects (by ID)")]
    public List<string> uiObjectIDs;   // IDs registered in DialogueUIRegistry
    public bool objectsInteractable = false;
}