
using UnityEngine;

public class DialogueClickStarter : MonoBehaviour
{
    [SerializeField] private DialogueBubble dialogue;

    private bool started = false;

    void Update()
    {
        if (started) return;

        if (Input.GetMouseButtonDown(0))
        {
            started = true;
            dialogue.AllowDialogueStart();
        }
    }
}