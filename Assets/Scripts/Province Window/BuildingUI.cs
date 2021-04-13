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
    public Image overlay;
    public ButtonSound toggleBuilding;
    #region Sounds
    [BoxGroup("Sounds")]
    public AudioSource audioSource;
    [BoxGroup("Sounds")]
    public AudioSource resourceBasedSource;
    #endregion
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

    #region Input and Output

    #region Output

    [BoxGroup("Output")]
    public ResourceUI[] resourceOutputUI;

    [BoxGroup("Output")]
    public OutputUIButton[] outputUI;

    [BoxGroup("Output")]
    public Image[] backgroundUI;

    #endregion 

    #region Input
    [BoxGroup("Input")]
    public ResourceUI[] resourceInputUI;

    [BoxGroup("Input")]
    public InputUI[] inputUI;

    [BoxGroup("Input")]
    public TextMeshProUGUI[] inputText;

    [System.Serializable]
    public class InputUI
    {
        public Image icon;
        public Image outline;
    }

    [Space(5)]

    #endregion

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

    //public void SetConnection()
    //{
    //    provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].connections.Add(new Province.ProvinceHolding.ProvinceBuilding.Connection());
    //}

    public void Start()
    {
        Refresh(holdingCounterpart, buildingCounterpart);
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
            textEfficiency.text = provinceWindow.provinceTarget.holdings[holding].buildings[building].efficiency.ToString("0") + "%";
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
                    backgroundUI[i].gameObject.SetActive(true);
                    outputUI[i].icon.sprite = provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceOutput[i].resource.icon;
                    outputUI[i].icon.SetNativeSize();
                    outputUI[i].icon.color = Color.white;
                    outputUI[i].amount.text = resourceOutputUI[i].resourceCount.ToString();
                }
                else if (i < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceWindow.provinceTarget.holdings[holding].buildings[building].buildingType].outputCapacity)
                {
                    outputUI[i].icon.gameObject.SetActive(true);
                    backgroundUI[i].gameObject.SetActive(true);
                    outputUI[i].icon.sprite = null;
                    outputUI[i].icon.GetComponent<RectTransform>().sizeDelta = new Vector2(16, 16);
                    outputUI[i].icon.color = grayblue;
                    outputUI[i].amount.text = null;
                }
                else
                {
                    outputUI[i].icon.gameObject.SetActive(false);
                    backgroundUI[i].gameObject.SetActive(false);
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
                    inputText[i].text = provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput[i].resourceNeedsCount.ToString();
                    inputText[i].gameObject.SetActive(true);

                    if (provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput[i].resourceCount < provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput[i].resourceNeedsCount)
                    {
                        inputUI[i].outline.color = CountryManager.instance.niceRed;
                    }
                    else
                    {
                        inputUI[i].outline.color = CountryManager.instance.niceGreen;
                    }

                    inputUI[i].outline.SetNativeSize();
                }
                else
                {
                    inputText[i].gameObject.SetActive(false);
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
        }
    }

    public void ToggleBuilding(bool on)
    {
        if (on)
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].on = true;
            overlay.gameObject.SetActive(false);
        }
        else
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].on = false;
            overlay.gameObject.SetActive(true);
        }
        provinceWindow.provinceTarget.RefreshProvinceValues();
        provinceWindow.RefreshWindow();
    }

    public void OpenCreationWindow()
    {
        provinceWindow.createBuildingWindow.gameObject.SetActive(true);
        for (int i = 0; i < provinceWindow.createBuildingOptions.Length; i++)
        {
            if (i >= Resources.Load<HoldingManager>("HoldingManager").holdings[(int)provinceWindow.provinceTarget.holdings[holdingCounterpart].holdingType].holdables.Length)
            {
                provinceWindow.createBuildingOptions[i].gameObject.SetActive(false);
            }
            else
            {
                provinceWindow.createBuildingOptions[i].gameObject.SetActive(true);
                provinceWindow.createBuildingOptions[i].buildingType = Resources.Load<HoldingManager>("HoldingManager").holdings[(int)provinceWindow.provinceTarget.holdings[holdingCounterpart].holdingType].holdables[i];
                provinceWindow.createBuildingOptions[i].icon.sprite = provinceWindow.buildingIcons[(int)Resources.Load<HoldingManager>("HoldingManager").holdings[(int)provinceWindow.provinceTarget.holdings[holdingCounterpart].holdingType].holdables[i]];
                provinceWindow.SetHoldingIndex(holdingCounterpart);
            }
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
                #region Raw Resource Gathering Holding Type
                if (provinceWindow.provinceTarget.holdings[holdingCounterpart].holdingType == HoldingType.RawResourceGathering)
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
                }
                #endregion
                #region Manufactory Holding Type
                else if (provinceWindow.provinceTarget.holdings[holdingCounterpart].holdingType == HoldingType.Manufactury)
                {
                    if (i < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].creatableResources.Length)
                    {
                        resourceOptions[i].resource = Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].creatableResources[i];
                        resourceOptions[i].GetComponent<Image>().sprite = resourceOptions[i].resource.icon;
                        resourceOptions[i].GetComponent<Image>().SetNativeSize();
                        resourceOptions[i].outputValue = outputValue;
                        resourceOptions[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        resourceOptions[i].gameObject.SetActive(false);
                    }
                }
                #endregion

                provinceWindow.provinceTarget.RefreshProvinceValues();
                provinceWindow.RefreshWindow();
                selectResourceWindow.gameObject.SetActive(true);
            }
        }
        else if (!create && outputValue < provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Count)
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Remove(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput[outputValue]);
            resourceOutputUI[outputValue] = null;
            provinceWindow.provinceTarget.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
        }
    }

    public void SetResourceButtons(Resource resource, bool create, int outputValue)
    {
        if (create)
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput.Add(new Province.ProvinceResource(resource, 0, 0));
            selectResourceWindow.gameObject.SetActive(false);
            provinceWindow.provinceTarget.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
            //resourceBasedSource.PlayOneShot(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].resourceOutput[outputValue].resource.sound);
        }
        else
        {
            selectResourceWindow.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (active && hovering && provinceWindow.target == CountryManager.instance.playerCountry)
        {
            #region Connections Mode (Alt Mode)
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
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
            #endregion

            //Destroy Building
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings.Remove(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart]);
                
                provinceWindow.provinceTarget.RefreshProvinceValues();
                provinceWindow.RefreshWindow();
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus) && provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Count < 18)
            {
                provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Add(new Population());
                provinceWindow.provinceTarget.RefreshProvinceValues();
                provinceWindow.RefreshWindow();
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus) && provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Count > 0)
            {
                provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Remove(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops[provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Count - 1]);
                provinceWindow.provinceTarget.RefreshProvinceValues();
                provinceWindow.RefreshWindow();
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
