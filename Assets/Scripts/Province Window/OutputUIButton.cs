using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutputUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public BuildingUI buildingUI;
    public Image highlight;
    public Color hovering;
    public Color clicked;
    [Range(1, 2)] public float pitch;
    public AudioSource sound;

    public Resource resource;
    public Image icon;
    public TextMeshProUGUI amount;
    public int outputValue;


    public void OnPointerDown(PointerEventData eventData)
    {
        highlight.color = clicked;
        sound.Play();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            buildingUI.OpenResourceSelection(outputValue, true);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            buildingUI.OpenResourceSelection(outputValue, false);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        highlight.color = hovering;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlight.color = hovering;
        highlight.gameObject.SetActive(true);
        sound.pitch = pitch;
        sound.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.gameObject.SetActive(false);
    }
}
