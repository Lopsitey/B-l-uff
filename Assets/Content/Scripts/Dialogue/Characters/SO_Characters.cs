using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Characters", menuName = "Scriptable Objects/SO_Characters")]
public class SO_Characters : ScriptableObject
{
    [Header("Character Info")]
    public DialogueMode characterName;
    public Sprite characterPortrait;

    [SerializeField] private List<DialogueLine> dialogueLines;

}
