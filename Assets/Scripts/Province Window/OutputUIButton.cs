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
    public bool hovering;
    public Color hoverColor;
    public Color clicked;
    [Range(1, 2)] public float pitch;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public Image outline;
    public Image icon;
    public TextMeshProUGUI amount;
    public int outputValue;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
        {
            highlight.color = clicked;
            audioSource.PlayOneShot(audioClip);
            if (Input.GetKeyDown(KeyCode.Mouse0) && !buildingUI.provinceWindow.altMode)
            {
                buildingUI.OpenResourceSelection(outputValue, true);
            }

            if (Input.GetKeyDown(KeyCode.Mouse1) && !buildingUI.provinceWindow.altMode)
            {
                buildingUI.OpenResourceSelection(outputValue, false);
                ProvinceManager.instance.selectedResource = null;
                buildingUI.provinceWindow.selectedOutput = null;
                buildingUI.provinceWindow.SelectingOutput();
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
        if (Input.GetKey(KeyCode.LeftAlt)
            && hovering
            && ProvinceManager.instance.selectedResource == null
            && System.Array.IndexOf(buildingUI.provinceWindow.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart].resourceOutputUI,
            buildingUI.provinceWindow.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart].resourceOutputUI[outputValue]) <
            buildingUI.provinceWindow.provinceTarget.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart].resourceOutput.Count)
        {
            buildingUI.provinceWindow.altMode = true;
            ProvinceManager.instance.hoveredResource = buildingUI.provinceWindow.provinceTarget.holdings[buildingUI.holdingCounterpart].buildings[buildingUI.buildingCounterpart].resourceOutput[outputValue];
            buildingUI.provinceWindow.hoveredOutput = this;
            buildingUI.provinceWindow.HoveringOutput();
        }
        else if (!Input.GetKey(KeyCode.LeftAlt))
        {
            buildingUI.provinceWindow.altMode = false;
            buildingUI.provinceWindow.hoveredOutputConnections.Clear();
            ProvinceManager.instance.selectedResource = null;
            ProvinceManager.instance.hoveredResource = null;
            ProvinceManager.instance.selectedResourceValue = 0;
            ProvinceManager.instance.selectedHoldingValue = 0;
            ProvinceManager.instance.selectedBuildingValue = 0;
            buildingUI.provinceWindow.selectedOutput = null;
            buildingUI.provinceWindow.hoveredOutput = null;
            buildingUI.provinceWindow.SelectingOutput();
            buildingUI.provinceWindow.HoveringOutput();
        }
    }
}
