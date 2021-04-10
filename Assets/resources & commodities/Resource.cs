using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Resource")]
public class Resource : ScriptableObject
{
    public ResourceType Type;
    public Sprite icon;
    public Sprite outline;
    [Range(0, 1)]public float acquisitionDifficulty;
    public AudioClip sound;
}
