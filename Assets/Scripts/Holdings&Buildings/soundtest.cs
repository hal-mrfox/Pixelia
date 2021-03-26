using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class soundtest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource sound;
    public Image selectionSquare;
    public bool lower;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (lower)
        {
            sound.pitch = 1.6f;
            sound.Play();
        }
        else
        {
            sound.pitch = 2f;
            sound.Play();
        }

        if (selectionSquare != null)
        {
            selectionSquare.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectionSquare != null)
        {
            selectionSquare.gameObject.SetActive(false);
        }
    }
}
