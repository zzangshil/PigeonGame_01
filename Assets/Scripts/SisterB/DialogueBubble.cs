using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueBubble : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Character")]
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite characterDefaultSprite;

    [Header("Dialogue")]
    [SerializeField] private DialogueNode startingNode;
    [SerializeField] private float typeSpeed = 0.03f;

    [Header("Choices")]
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Transform choiceContainer;

    [Header("Audio")]
    [SerializeField] private AudioSource dialogueAudio;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float dialogueVolume = 1f;

    [Range(1f, 3f)]
    [SerializeField] private float dialogueGain = 1.5f; // 🔊 Loudness boost

    private DialogueNode currentNode;
    private int lineIndex;
    private bool isTyping;
    private bool waitingForChoice;
    private Coroutine typingRoutine;

    private bool canStartDialogue = false;
    private bool dialogueStarted = false;

    public event System.Action OnDialogueEnded;

    void Start()
    {
        InitializeUI();
        ApplyAudioVolume();
    }

    void Update()
    {
        if (!canStartDialogue)
            return;

        if (!dialogueStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dialogueStarted = true;
                StartDialogue(startingNode);
            }
            return;
        }

        if (currentNode == null || waitingForChoice)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
                SkipTyping();
            else
                AdvanceLine();
        }
    }

    void InitializeUI()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (choiceContainer)
            choiceContainer.gameObject.SetActive(false);

        if (characterImage && characterDefaultSprite)
            characterImage.sprite = characterDefaultSprite;
    }

    public void AllowDialogueStart()
    {
        canStartDialogue = true;
    }

    public void StartDialogue(DialogueNode node)
    {
        if (!node)
        {
            EndDialogue();
            return;
        }

        currentNode = node;
        lineIndex = 0;
        waitingForChoice = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        ShowLine();
    }

    void ShowLine()
    {
        if (lineIndex >= currentNode.lines.Count)
        {
            ShowChoices();
            return;
        }

        DialogueLine line = currentNode.lines[lineIndex];

        // UI object handling (unchanged)
        HandleLineObjects(line);

        if (dialogueAudio && line.voiceClip)
        {
            dialogueAudio.Stop();
            dialogueAudio.clip = line.voiceClip;
            ApplyAudioVolume();
            dialogueAudio.Play();
        }

        if (characterImage)
            characterImage.sprite = line.emotionSprite != null
                ? line.emotionSprite
                : characterDefaultSprite;

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeText(line.text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    void SkipTyping()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        dialogueText.text = currentNode.lines[lineIndex].text;
        isTyping = false;
    }

    void AdvanceLine()
    {
        lineIndex++;
        ShowLine();
    }

    void ShowChoices()
    {
        if (currentNode.choices == null || currentNode.choices.Count == 0)
        {
            EndDialogue();
            return;
        }

        waitingForChoice = true;
        choiceContainer.gameObject.SetActive(true);

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        foreach (DialogueChoice choice in currentNode.choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
            ChoiceButton choiceBtn = btn.GetComponent<ChoiceButton>();
            if (choiceBtn != null)
                choiceBtn.Setup(this, choice);
        }
    }

    public void OnChoiceSelected(DialogueChoice choice)
    {
        waitingForChoice = false;
        choiceContainer.gameObject.SetActive(false);

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        if (choice.nextNode != null)
            StartDialogue(choice.nextNode);
        else
            EndDialogue();
    }

    void EndDialogue()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        currentNode = null;
        OnDialogueEnded?.Invoke();
    }

    // 🔊 Apply volume + gain (core loudness logic)
    void ApplyAudioVolume()
    {
        if (dialogueAudio == null)
            return;

        dialogueAudio.volume = Mathf.Clamp01(dialogueVolume) * dialogueGain;
    }

    // 🔄 Live Inspector updates
    void OnValidate()
    {
        ApplyAudioVolume();
    }

    // ✅ Unchanged UI object logic
    void HandleLineObjects(DialogueLine line)
    {
        if (DialogueUIRegistry.Instance == null || line.uiObjectIDs == null)
            return;

        foreach (string id in line.uiObjectIDs)
        {
            Image img = DialogueUIRegistry.Instance.Get(id);
            if (img == null) continue;

            img.gameObject.SetActive(true);

            Color c = img.color;
            c.a = 1f;
            img.color = c;

            CanvasGroup cg = img.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1f;
                cg.interactable = line.objectsInteractable;
                cg.blocksRaycasts = line.objectsInteractable;
            }

            Button btn = img.GetComponent<Button>();
            if (btn != null)
                btn.interactable = line.objectsInteractable;
        }
    }
}