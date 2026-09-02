using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Characters", menuName = "Scriptable Objects/SO_Characters")]
public class SO_Characters : ScriptableObject
{
    [Header("Character Info")]
    public DialogueMode characterName;
    public Sprite characterPortrait;

    public AudioClip[] dialogueSFXs;

    [Range(-3, 3)]
    public float minPitch = 0.3f;
    [Range(-3, 3)]
    public float maxPitch = 3f;


    [Range(1, 5)]
    public int frequencyLevel = 2;

}
