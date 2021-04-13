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
    public bool hovering;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public Resource resource;
    [Range(1, 2)]public float pitch;

    public Color red;
    public Color green;

    public int InputValue;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) 
            && buildingUI.provinceWindow.altMode 
            && ProvinceManager.instance.selectedResource.resource == resource
            && ProvinceManager.instance.selectedResourceBuilding != buildingUI.provinceWindow.provinceTarget.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart])
        {
            audioSource.PlayOneShot(audioClip);
            buildingUI.provinceWindow.provinceTarget.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart].connections.Add(new Province.ProvinceResource(ProvinceManager.instance.selectedResource.resource, ProvinceManager.instance.selectedResource.resourceCount, 0));
            buildingUI.provinceWindow.provinceTarget.RefreshProvinceValues();
            buildingUI.provinceWindow.RefreshWindow();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip);
        if (ProvinceManager.instance.selectedResource != null)
        {
            highlight.gameObject.SetActive(true);
            if (ProvinceManager.instance.selectedResource.resource == resource
                && ProvinceManager.instance.selectedResourceBuilding != buildingUI.provinceWindow.provinceTarget.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart])
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
