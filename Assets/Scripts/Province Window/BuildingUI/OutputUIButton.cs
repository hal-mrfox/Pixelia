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
        if (buildingUI.provinceWindow.targetCountry == CountryManager.instance.playerCountry)
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
        if (buildingUI.provinceWindow.targetCountry == CountryManager.instance.playerCountry)
        {
            highlight.color = hoverColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buildingUI.provinceWindow.targetCountry == CountryManager.instance.playerCountry)
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
        if (buildingUI.provinceWindow.targetCountry == CountryManager.instance.playerCountry)
        {
            hovering = false;
            highlight.gameObject.SetActive(false);
        }
    }
}
