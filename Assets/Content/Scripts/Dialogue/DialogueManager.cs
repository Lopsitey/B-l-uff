using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [Header("Autoplay Delay")]
    public float autoplayDelay = 0f;
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
    private int lastClosedFrame = -1;
    private float lastClosedTime = -1f;
    private int lastAdvanceFrame = -1;

    public bool IsDialogueActive => dialogueActive;
    public bool WasDialogueActiveRecently => dialogueActive || Time.frameCount == lastClosedFrame || (Time.unscaledTime - lastClosedTime < 0.2f);

    private TMP_Text currentText;

    public void SetNewDialogue(List<DialogueLine> newDialogueLines)
    {
        if (newDialogueLines == null || newDialogueLines.Count == 0)
            return;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        dialogueLines = newDialogueLines;
        currentLineIndex = 0;

        StartDialogue();
    }

    public void StartDialogue()
    {
        if (DialogueBox != null)
            DialogueBox.SetActive(true);

        if (dialogueLines == null || dialogueLines.Count == 0)
        {
            Debug.LogWarning("No dialogue lines have been assigned.");
            if (DialogueBox != null)
                DialogueBox.SetActive(false);
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

        // Guard against multiple triggers on the exact same frame (e.g. UI Button + InputHandler)
        if (Time.frameCount == lastAdvanceFrame)
            return;
        lastAdvanceFrame = Time.frameCount;

        // If the text is currently typing,
        // clicking finishes the current line instead.
        if (isTyping)
        {
            FinishTyping();
            return;
        }

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
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

        if (line.character != null && Portrait != null)
        {
            Portrait.sprite = line.character.characterPortrait;
        }

        StartTyping(line.text);

        if (line.character != null)
        {
            switch (line.character.characterName)
            {
                case DialogueMode.Player:
                    if (playerPanel != null) playerPanel.SetActive(true);
                    break;

                default:
                    if (enemyPanel != null) enemyPanel.SetActive(true);
                    break;
            }
        }
    }

    private Coroutine autoAdvanceCoroutine;

    private void StartTyping(string text)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        typewriterCoroutine = StartCoroutine(TypeText(text));
        Debug.Log($"Started typing: {text}");
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        if (DialogueText != null)
        {
            DialogueText.text = text;

            // Force TMP to generate the text information immediately
            DialogueText.ForceMeshUpdate();

            DialogueText.maxVisibleCharacters = 0;

            int characterCount = DialogueText.textInfo.characterCount;

            yield return new WaitForSeconds(0.15f);
            for (int i = 0; i <= characterCount; i++)
            {
                DialogueText.maxVisibleCharacters = i;

                yield return new WaitForSeconds(
                    1f / charactersPerSecond
                );
            }
        }

        isTyping = false;
        typewriterCoroutine = null;

        if (dialogueLines != null && currentLineIndex < dialogueLines.Count)
        {
            DialogueLine line = dialogueLines[currentLineIndex];
            if (line.autoplayDelay > 0f)
            {
                autoAdvanceCoroutine = StartCoroutine(AutoAdvanceAfterDelay(line.autoplayDelay));
            }
        }
    }

    private void FinishTyping()
    {
        if (!isTyping)
            return;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (DialogueText != null)
        {
            DialogueText.maxVisibleCharacters =
                DialogueText.textInfo.characterCount;
        }

        isTyping = false;
    }

    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"Auto-advancing after {delay} seconds.");
        autoAdvanceCoroutine = null;
        NextLine();
    }

    private void HideAllPanels()
    {
        if (playerPanel != null) playerPanel.SetActive(false);
        if (enemyPanel != null) enemyPanel.SetActive(false);
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        isTyping = false;
        lastClosedFrame = Time.frameCount;
        lastClosedTime = Time.unscaledTime;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        HideAllPanels();

        if (DialogueBox != null)
            DialogueBox.SetActive(false);

        Debug.Log("Dialogue finished.");
    }

    private void ToggleDialogueBox() 
    {
        if (DialogueBox != null)
        {
            DialogueBox.SetActive(!DialogueBox.activeSelf);
        }
    }
}
