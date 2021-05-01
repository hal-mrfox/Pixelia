using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupOption : ButtonSound
{
    public WindowProvince provinceWindow;
    public BuildingType buildingType;
    public Image icon;
    public TextMeshProUGUI buildingName;
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (interactable)
        {
            if (buttonType == ButtonType.click)
            {
                audioSource.PlayOneShot(sound);
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    image.color = clicked;
                    provinceWindow.CreateBuilding(buildingType);
                }
            }
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (interactable)
        {
            if (hover)
            {
                image.color = hovering;
            }
            else
            {
                image.color = normal;
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
        if (lower)
        {
            audioSource.pitch = 1.7f;
            audioSource.PlayOneShot(sound);
        }
        else
        {
            audioSource.pitch = 2f;
            audioSource.PlayOneShot(sound);
        }

        if (image != null)
        {
            image.color = hovering;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!toggled)
        {
            if (image != null)
            {
                image.color = normal;
            }
        }
        else
        {
            if (image != null)
            {
                image.color = clicked;
            }
        }
        hover = false;
    }
}
