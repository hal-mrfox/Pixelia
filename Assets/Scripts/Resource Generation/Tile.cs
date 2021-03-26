using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public ResourceType resourceType;
    public new RectTransform transform;
    public Image image;

    public void Awake()
    {
        transform = (RectTransform)base.transform;
    }
}
