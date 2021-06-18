using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class InputUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public BuildingUI buildingUI;
    public Image icon;
    public Image outline;
    public Image highlight;
    public Image connectionHighlight;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI needsText;
    public bool hovering;
    public AudioSource audioSource;
    [Range(1, 2)]public float pitch;
    public AudioClip audioClip;
    public Resource resource;
    public int inputValue;

    public Color red;
    public Color green;

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
