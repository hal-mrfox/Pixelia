using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopulationUICounterpart : ButtonSound
{
    public WindowProvince provinceWindow;
    public Population popCP;
    public enum UIType { job, home, list }
    public UIType uIType;

    #region Pop List Stuff
    [BoxGroup("Pop List")]
    public Image popTierUIStrip;
    [BoxGroup("Pop List")]
    public TextMeshProUGUI jobStatus;
    [BoxGroup("Pop List")]
    public TextMeshProUGUI homeStatus;
    [BoxGroup("Pop List")]
    public TextMeshProUGUI popNameText;
    #endregion

    bool clicking;
    GameObject previewPop;

    Vector2Int squareSize = new Vector2Int(10, 10);
    public override void OnPointerDown(PointerEventData eventData)
    {
        #region fuck shit
        if (buildingUI == null)
        {
            if (interactable)
            {
                if (buttonType == ButtonType.click)
                {
                    audioSource.PlayOneShot(sound);
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        image.color = clicked;
                    }
                    onClick?.Invoke();
                }
                else if (buttonType == ButtonType.toggle)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        toggled = !toggled;
                        if (toggled)
                        {
                            image.color = clicked;
                            buildingUI.ToggleBuilding(toggled);
                        }
                        else
                        {
                            image.color = hovering;
                            buildingUI.ToggleBuilding(toggled);
                        }
                        audioSource.PlayOneShot(sound);
                    }
                }
            }
        }
        else
        {
            if (interactable && buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
            {
                if (buttonType == ButtonType.click)
                {
                    audioSource.PlayOneShot(sound);
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        image.color = clicked;
                    }
                    onClick?.Invoke();
                }
                else if (buttonType == ButtonType.toggle)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        toggled = !toggled;
                        if (toggled)
                        {
                            image.color = clicked;
                            buildingUI.ToggleBuilding(toggled);
                        }
                        else
                        {
                            image.color = hovering;
                            buildingUI.ToggleBuilding(toggled);
                        }
                        audioSource.PlayOneShot(sound);
                    }
                }
            }
        }
        #endregion

        #region Drag and Drop
        if (Input.GetKeyDown(KeyCode.Mouse0) && popCP && uIType != UIType.list)
        {
            buildingUI.provinceWindow.movingPop = popCP;
            if (uIType == UIType.job)
            {
                provinceWindow.job = true;
            }
            else
            {
                provinceWindow.job = false;
            }
            clicking = true;

            previewPop = new GameObject("Preview Pop");

            if (GetComponent<Image>().sprite)
            {
                previewPop.AddComponent<Image>();
                previewPop.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
                previewPop.GetComponent<Image>().SetNativeSize();
                previewPop.GetComponent<RectTransform>().sizeDelta = new Vector2(previewPop.GetComponent<RectTransform>().sizeDelta.x + 2, previewPop.GetComponent<RectTransform>().sizeDelta.y + 2);
            }
            else
            {
                previewPop.AddComponent<Image>();
                previewPop.GetComponent<RectTransform>().sizeDelta = new Vector2(squareSize.x + 2, squareSize.y + 2);
            }
            previewPop.GetComponent<Image>().raycastTarget = false;
            previewPop.GetComponent<Image>().color = GetComponent<Image>().color;
            previewPop.transform.SetParent(provinceWindow.transform);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && popCP && uIType == UIType.list)
        {
            provinceWindow.movingPop = popCP;
            if (uIType == UIType.job)
            {
                provinceWindow.job = true;
            }
            else
            {
                provinceWindow.job = false;
            }
            clicking = true;

            previewPop = new GameObject("Preview Pop");

            previewPop.AddComponent<Image>();
            previewPop.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x + 2, GetComponent<RectTransform>().sizeDelta.y + 2);

            previewPop.GetComponent<Image>().raycastTarget = false;
            previewPop.GetComponent<Image>().color = new Color(GetComponent<PopulationUICounterpart>().normal.r, GetComponent<PopulationUICounterpart>().normal.g, GetComponent<PopulationUICounterpart>().normal.b, 0.5f);
            previewPop.transform.SetParent(provinceWindow.transform);
        }
        #endregion

        #region Unassign Pops
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (uIType == UIType.home)
            {
                popCP.home.housedPops.Remove(popCP);
                popCP.home = null;
                popCP = null;
            }

            if (uIType == UIType.job)
            {
                popCP.job.pops.Remove(popCP);
                popCP.transform.SetParent(popCP.provinceController.unemployed.transform);
                popCP.job = null;
                popCP = null;
            }

            provinceWindow.provinceTarget.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
        }
        #endregion
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        provinceWindow.DropPop();

        clicking = false;

        #region Drag and Drop
        Destroy(previewPop);
        #endregion
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (popCP)
        {
            provinceWindow.HighlightPop(popCP, uIType);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (popCP)
        {
            provinceWindow.UnHighlightPops(popCP, uIType);
        }
    }

    public void Update()
    {
        if (clicking && popCP)
        {
            previewPop.transform.position = Input.mousePosition;
        }
    }
}
