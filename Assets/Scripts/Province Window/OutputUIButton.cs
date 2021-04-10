using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutputUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public BuildingUI buildingUI;
    public Image highlight;
    bool hovering;
    public Color hoverColor;
    public Color clicked;
    [Range(1, 2)] public float pitch;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public Resource resource;
    public Image outline;
    public Image icon;
    public TextMeshProUGUI amount;
    public int outputValue;
    public bool altMode;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
        {
            highlight.color = clicked;
            audioSource.PlayOneShot(audioClip);
            if (Input.GetKeyDown(KeyCode.Mouse0) && !altMode)
            {
                buildingUI.OpenResourceSelection(outputValue, true);
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1) && !altMode)
            {
                buildingUI.OpenResourceSelection(outputValue, false);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && altMode)
            {
                ProvinceManager.instance.selectedResource = buildingUI.provinceWindow.provinceTarget.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart].resourceOutput[outputValue];
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
        {
            highlight.color = hoverColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
        {
            hovering = true;
            highlight.color = hoverColor;
            highlight.gameObject.SetActive(true);
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
        {
            hovering = false;
            highlight.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && hovering && buildingUI.provinceWindow.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart].resourceOutputUI[outputValue].resourceIcon != null)
        {
            buildingUI.provinceWindow.hoveredOutput = this;
            buildingUI.provinceWindow.HoveringOutput();
        }
        else if (!Input.GetKey(KeyCode.LeftAlt))
        {
            ProvinceManager.instance.selectedResource = null;
            buildingUI.provinceWindow.hoveredOutput = null;
            buildingUI.provinceWindow.HoveringOutput();
        }
    }
}
