using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using UnityEngine;
using TMPro;
using System.Linq;
using Pigeon;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public WindowProvince provinceWindow;
    public BuildingType buildingType;
    public int holdingCounterpart;
    public int buildingCounterpart;
    public Image highlight;
    public AudioSource audioSource;
    public RectTransform rectTransform;
    public UnityEngine.UI.Button createBuildingButton;
    public bool active;

    bool hovering;

    #region Resource Selection

    [BoxGroup("Resource Selection")]
    public Image selectResourceWindow;

    [BoxGroup("Resource Selection")]
    public GridLayoutGroup resourceSelectionLayout;

    [BoxGroup("Resource Selection")]
    public List<OptionUI> resourceOptions;

    [BoxGroup("Resource Selection")]
    public Color grayblue;

    #endregion

    #region Output

    [BoxGroup("Output")]
    public ResourceUI[] resourceOutputUI;

    [BoxGroup("Output")]
    public OutputUIButton[] outputUI;

    #endregion 

    #region Input
    [BoxGroup("Input")]
    public ResourceUI[] resourceInputUI;

    [BoxGroup("Input")]
    public InputUI[] inputUI;

    [System.Serializable]
    public class InputUI
    {
        public Image icon;
        public Image outline;
    }

    [Space(5)]

    #endregion

    #region Storage

    [BoxGroup("Storage")]
    public ResourceUI[] resourceStorageUI;

    [BoxGroup("Storage")]
    public OutputUIButton[] storageUI;

    #endregion 

    #region General

    public TextMeshProUGUI textEfficiency;

    public TextMeshProUGUI buildingTypeName;

    public List<Graphic> popIcons;

    [Space(5)]

    #region Colors

    public Color filled;

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

    public void Awake()
    {
        provinceWindow = FindObjectOfType<WindowProvince>();
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
                    popIcons[i].GetComponent<ButtonSound>().lower = false;
                }
                else
                {
                    popIcons[i].color = empty;
                    popIcons[i].GetComponent<ButtonSound>().lower = true;
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
                    outputUI[i].icon.color = Color.white;
                    outputUI[i].amount.text = resourceOutputUI[i].resourceCount.ToString();
                }
                else
                {
                    outputUI[i].icon.gameObject.SetActive(true);
                    outputUI[i].icon.sprite = null;
                    outputUI[i].icon.color = grayblue;
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
            provinceWindow.selectBuildingWindow.SetActive(true);
            createBuildingButton.onClick.RemoveAllListeners();
            createBuildingButton.onClick.AddListener(() => provinceWindow.provinceTarget.CreateBuilding(holding, 0));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && selectResourceWindow == true)
        {
            selectResourceWindow.gameObject.SetActive(false);
        }
    }

    public void OpenResourceSelection(int outputValue, bool create)
    {
        if (create
            && 
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Count <= outputValue
            &&
            outputValue < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].buildingType].outputCapacity)
        {
            for (int i = 0; i < resourceOptions.Count; i++)
            {
                if (i < provinceWindow.provinceTarget.rawResources.Count && Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].creatableResources.Contains(provinceWindow.provinceTarget.rawResources[i].resource))
                {
                    resourceOptions[i].resource = provinceWindow.provinceTarget.rawResources[i].resource;
                    if (provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Count > 0)
                    {
                        for (int j = 0; j < provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Count; j++)
                        {
                            resourceOptions[i].GetComponent<Image>().sprite = resourceOptions[i].resource.icon;
                            resourceOptions[i].GetComponent<Image>().SetNativeSize();
                            resourceOptions[i].outputValue = outputValue;
                            if (provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput[j].resource == resourceOptions[i].resource)
                            {
                                resourceOptions[i].gameObject.SetActive(false);
                                break;
                            }
                            else
                            {
                                resourceOptions[i].gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        resourceOptions[i].GetComponent<Image>().sprite = resourceOptions[i].resource.icon;
                        resourceOptions[i].GetComponent<Image>().SetNativeSize();
                        resourceOptions[i].outputValue = outputValue;
                        resourceOptions[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    resourceOptions[i].gameObject.SetActive(false);
                }
                provinceWindow.provinceTarget.RefreshProvinceValues();
                provinceWindow.RefreshWindow();
                selectResourceWindow.gameObject.SetActive(true);
            }
        }
        else if (!create && outputValue < provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Count)
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Remove(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput[outputValue]);
            provinceWindow.provinceTarget.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
        }
    }

    public void SetResourceButtons(Resource resource, bool create, int outputValue)
    {
        if (create )
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Add(new Province.ProvinceResource(resource, 0));
            selectResourceWindow.gameObject.SetActive(false);
            provinceWindow.provinceTarget.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
        }
        else
        {
            selectResourceWindow.gameObject.SetActive(false);
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
