using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Resource")]
public class Resource : ScriptableObject
{
    public string Type;
    public Sprite icon;
}
