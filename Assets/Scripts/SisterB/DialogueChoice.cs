using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string text;                   // The choice text
    public DialogueNode nextNode;  // The node to go to if chosen
}