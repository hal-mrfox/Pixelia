using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class soundtest : MonoBehaviour, IPointerEnterHandler
{
    public AudioSource dwink;

    public void OnPointerEnter(PointerEventData eventData)
    {
        dwink.Play();
    }
}
