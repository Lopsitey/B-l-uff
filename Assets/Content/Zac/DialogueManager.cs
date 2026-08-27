using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum DialogueMode
{
    Player,
    NPC1,
    NPC2,
    NPC3,
    Death
}


[Serializable]
public class DialogueLine
{
    [Header("Speaker")]
    public SO_Characters character;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string text;
}



public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private List<DialogueLine> dialogueLines;

    [Header("Typewriter")]
    [SerializeField] private float charactersPerSecond = 40f;

    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] private GameObject DialogueBox;
    [SerializeField] private Image Portrait;
    [SerializeField] private TMP_Text DialogueText;

    private int currentLineIndex;
    private Coroutine typewriterCoroutine;

    private bool isTyping;
    private bool dialogueActive;

    private TMP_Text currentText;

    private void Start()
    {
        ToggleDialogueBox();
        StartDialogue();
    }

    public void StartDialogue()
    {

        ToggleDialogueBox();

        if (dialogueLines == null || dialogueLines.Count == 0)
        {
            Debug.LogWarning("No dialogue lines have been assigned.");
            return;
        }

        currentLineIndex = 0;
        dialogueActive = true;

        ShowCurrentLine();
    }

    public void NextLine()
    {
        if (!dialogueActive)
            return;

        // If the text is currently typing,
        // clicking finishes the current line instead.
        if (isTyping)
        {
            FinishTyping();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex >= dialogueLines.Count)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        DialogueLine line = dialogueLines[currentLineIndex];

        HideAllPanels();

        Portrait.sprite = line.character.characterPortrait;
        
        StartTyping(line.text);

        switch (line.character.characterName)
        {
            case DialogueMode.Player:
                playerPanel.SetActive(true);
                break;

            default:
                enemyPanel.SetActive(true);
                break;

        }

    }

    private void StartTyping(string text)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        typewriterCoroutine = StartCoroutine(TypeText(text));
        Debug.Log($"Started typing: {text}");
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        DialogueText.text = text;

        // Force TMP to generate the text information immediately
        DialogueText.ForceMeshUpdate();

        DialogueText.maxVisibleCharacters = 0;

        int characterCount = DialogueText.textInfo.characterCount;

        for (int i = 0; i <= characterCount; i++)
        {
            DialogueText.maxVisibleCharacters = i;

            yield return new WaitForSeconds(
                1f / charactersPerSecond
            );
        }

        isTyping = false;
        typewriterCoroutine = null;
    }

    private void FinishTyping()
    {
        if (!isTyping || currentText == null)
            return;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        currentText.maxVisibleCharacters =
            currentText.textInfo.characterCount;

        isTyping = false;
    }

    private void HideAllPanels()
    {
        playerPanel.SetActive(false);
        enemyPanel.SetActive(false);
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        isTyping = false;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        HideAllPanels();

        ToggleDialogueBox();

        Debug.Log("Dialogue finished.");
    }

    private void ToggleDialogueBox() 
    {
        DialogueBox.SetActive(!DialogueBox.activeSelf);
    }
}
