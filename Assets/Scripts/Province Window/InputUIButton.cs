using System.Collections;
using System.Collections.Generic;
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
        if (Input.GetKeyDown(KeyCode.Mouse0) && buildingUI.provinceWindow.altMode && ProvinceManager.instance.selectedResource.resource == resource)
        {
            buildingUI.provinceWindow.provinceTarget.RefreshProvinceValues();
            buildingUI.provinceWindow.RefreshWindow();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip);
        if (ProvinceManager.instance.selectedResource != null)
        {
            highlight.gameObject.SetActive(true);

            if (ProvinceManager.instance.selectedResource.resource == resource)
            {
                highlight.color = green;
            }
            else
            {
                highlight.color = red;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
