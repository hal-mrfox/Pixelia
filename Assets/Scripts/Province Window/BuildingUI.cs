using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using UnityEngine;
using TMPro;
using Pigeon;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public WindowProvince provinceWindow;
    public Image highlight;
    public AudioSource audioSource;
    public RectTransform rectTransform;
    public UnityEngine.UI.Button createBuildingButton;
    public bool active;

    bool hovering;

    [Space(10)]

    #region Output
    [Header("Output")]
    public ResourceUI[] resourceOutputUI;
    public OutputUI[] outputUI;

    [System.Serializable]
    public class OutputUI
    {
        public Image icon;
        public TextMeshProUGUI amount;
    }
    #endregion

    [Space(10)]

    #region Input
    [Header("Input")]
    public ResourceUI[] resourceInputUI;
    public InputUI[] inputUI;

    [System.Serializable]
    public class InputUI
    {
        public Image icon;
        public Image outline;
    }
    #endregion

    #region General
    [Space(10)]
    public TextMeshProUGUI textEfficiency;
    public TextMeshProUGUI buildingTypeName;
    public List<Graphic> popIcons;
    #region Colors
    [BoxGroup("Color")]
    public Color filled;
    [BoxGroup("Color")]
    public Color empty;
    #endregion
    #endregion

    [System.Serializable]
    public class ResourceUI
    {
        public Sprite resourceIcon;
        public int resourceCount;

        public ResourceUI(Sprite resourceIcon, int resourceCount)
        {
            this.resourceIcon = resourceIcon;
            this.resourceCount = resourceCount;
        }
    }

    public void Refresh(int holding, int building)
    {
        if (active)
        {
            createBuildingButton.gameObject.SetActive(false);
            #region Pops
            int pops = provinceWindow.provinceTarget.holdings[holding].buildings[building].pops.Count;
            for (int i = 0; i < popIcons.Count; i++)
            {
                if (i < pops)
                {
                    popIcons[i].color = filled;
                    popIcons[i].GetComponent<soundtest>().lower = false;
                }
                else
                {
                    popIcons[i].color = empty;
                    popIcons[i].GetComponent<soundtest>().lower = true;
                }
            }
            #endregion
            #region Efficiency
            textEfficiency.text = provinceWindow.provinceTarget.holdings[holding].buildings[building].efficiency.ToString("00") + "%";
            #endregion
            #region Name
            buildingTypeName.text = provinceWindow.provinceTarget.holdings[holding].buildings[building].buildingType.ToString();
            #endregion
            #region Resources
            #region Output
            for (int i = 0; i < provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceOutput.Count; i++)
            {
                resourceOutputUI[i] = new ResourceUI(provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceOutput[i].resource.icon, provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceOutput[i].resourceCount);
            }
            for (int i = 0; i < outputUI.Length; i++)
            {
                if (i < provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceOutput.Count)
                {
                    outputUI[i].icon.gameObject.SetActive(true);
                    outputUI[i].icon.sprite = provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceOutput[i].resource.icon;
                    outputUI[i].icon.SetNativeSize();
                    outputUI[i].amount.text = resourceOutputUI[i].resourceCount.ToString();
                }
                else
                {
                    outputUI[i].icon.gameObject.SetActive(false);
                    outputUI[i].icon.sprite = null;
                    outputUI[i].amount.text = null;
                }
            }
            #endregion
            #region Input
            for (int i = 0; i < provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput.Count; i++)
            {
                resourceInputUI[i] = new ResourceUI(provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput[i].resource.icon, provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput[i].resourceCount);
            }
            for (int i = 0; i < inputUI.Length; i++)
            {
                if (i < provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput.Count)
                {
                    inputUI[i].icon.gameObject.SetActive(true);
                    inputUI[i].icon.sprite = provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput[i].resource.icon;
                    inputUI[i].icon.SetNativeSize();
                    inputUI[i].outline.gameObject.SetActive(true);
                    inputUI[i].outline.sprite = provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput[i].resource.outline;
                    inputUI[i].outline.SetNativeSize();
                }
                else
                {
                    inputUI[i].icon.gameObject.SetActive(false);
                    inputUI[i].outline.gameObject.SetActive(false);
                    inputUI[i].icon.sprite = null;
                    inputUI[i].outline.sprite = null;
                }
            }
            #endregion
            #endregion
        }
        else
        {
            createBuildingButton.gameObject.SetActive(true);
            createBuildingButton.onClick.RemoveAllListeners();
            createBuildingButton.onClick.AddListener(() => provinceWindow.provinceTarget.CreateBuilding(holding, 0));
        }
    }

    public void Update()
    {
        if (active && hovering)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                audioSource.Play();
                rectTransform.SetAsLastSibling();
            }

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                highlight.gameObject.SetActive(true);
            }
            else
            {
                highlight.gameObject.SetActive(false);
            }
        }
        else
        {
            highlight.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
