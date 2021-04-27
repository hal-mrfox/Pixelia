using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class RawResourceUI : MonoBehaviour
{
    public Image icon;

    public void Refresh(Sprite resource, int quantity)
    {
        icon.sprite = resource;
        icon.SetNativeSize();
    }
}
