using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using UnityEngine;
using TMPro;
using System.Linq;
using Pigeon;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public WindowProvince provinceWindow;
    public BuildingType buildingType;
    public Building buildingCounterpart;
    public int holdingCounterpart;
    public int buildingCounterpartIndex;
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

    public bool hovering;

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

    #region BuildingTypes
    public Image housingUI;
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
    public InputUIButton[] inputUI;

    [BoxGroup("Input")]
    public TextMeshProUGUI[] inputText;

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

    #region Housing
    [BoxGroup("Housing Section")]
    public TextMeshProUGUI popGrowth;
    [BoxGroup("Housing Section")]
    public TextMeshProUGUI inhabitants;
    [BoxGroup("Housing Section")]
    public Image fillBar;
    [BoxGroup("Housing Section")]
    public Image fillBarNext;
    [BoxGroup("Housing Section")]
    public Image popTypeIcon;
    [BoxGroup("Housing Section")]
    public PopulationUICounterpart[] housedPopIcons;
    #endregion

    #region Highlights for moving pops
    [BoxGroup("Highlights for Moving Pops")]
    public Image housingHighlight;
    [BoxGroup("Highlights for Moving Pops")]
    public Image jobHighlight;
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

    public void Start()
    {
        Refresh(holdingCounterpart, buildingCounterpartIndex);
    }

    public void Refresh(int holding, int building)
    {
        if (active)
        {
            createBuildingButton.gameObject.SetActive(false);

            #region Housing
            if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isHousing)
            {
                housingUI.gameObject.SetActive(true);
            }
            else
            {
                housingUI.gameObject.SetActive(false);
            }

            //Inhabitants
            var buildingCounterpart = provinceWindow.provinceTarget.holdings[holding].buildings[building];

            inhabitants.text = buildingCounterpart.housedPops.Count.ToString() + "/" + Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingCounterpart.buildingType].housingCapacity;
            for (int i = 0; i < housedPopIcons.Length; i++)
            {
                if (i < buildingCounterpart.housedPops.Count)
                {
                    Color popColor = PopulationManager.instance.popTierDetails[(int)Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops[0]].popColor;
                    housedPopIcons[i].gameObject.SetActive(true);
                    housedPopIcons[i].GetComponent<Image>().color = popColor;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().normal = popColor;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().hovering = popColor;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().clicked = popColor;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().PopCP = buildingCounterpart.housedPops[i];
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().work = false;
                }
                else
                {
                    housedPopIcons[i].gameObject.SetActive(false);
                }
            }

            //PopGrowth
            popGrowth.text = "+" + buildingCounterpart.popGrowthMultiplier.ToString("0.00") + "%";

            //Fillbar
            fillBar.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, 56, buildingCounterpart.popGrowth / 100f), fillBar.rectTransform.sizeDelta.y);
            fillBarNext.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, 56, (buildingCounterpart.popGrowth + buildingCounterpart.popGrowthMultiplier) / 100), fillBar.rectTransform.sizeDelta.y);

            //PopTier color thing
            popTypeIcon.color = PopulationManager.instance.popTierDetails[(int)Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingCounterpart.buildingType].allowedPops[0]].popColor;
            #endregion
            #region Pops
            int pops = provinceWindow.provinceTarget.holdings[holding].buildings[building].pops.Count;
            for (int i = 0; i < popIcons.Count; i++)
            {
                if (i < pops)
                {
                    Color popColor = PopulationManager.instance.popTierDetails[(int)provinceWindow.provinceTarget.holdings[holding].buildings[building].pops[i].popTier].popColor;
                    popIcons[i].GetComponent<Image>().color = popColor;
                    popIcons[i].GetComponent<PopulationUICounterpart>().lower = false;
                    popIcons[i].GetComponent<PopulationUICounterpart>().PopCP = buildingCounterpart.pops[i];
                    popIcons[i].GetComponent<PopulationUICounterpart>().work = true;
                }
                else
                {
                    popIcons[i].color = empty;
                    popIcons[i].GetComponent<PopulationUICounterpart>().lower = true;
                }
            }
            #region Highlight
            for (int i = 0; i < popIcons.Count; i++)
            {
                popIcons[i].GetComponent<PopulationUICounterpart>().highlight.gameObject.SetActive(false);
            }
            for (int i = 0; i < housedPopIcons.Length; i++)
            {
                housedPopIcons[i].highlight.gameObject.SetActive(false);
            }
            #endregion
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
                    inputUI[i].resource = provinceWindow.provinceTarget.holdings[holding].buildings[building].resourceInput[i].resource;
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

    public void HighlightPop()
    {

    }

    public void ToggleBuilding(bool on)
    {
        if (on)
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].on = true;
            overlay.gameObject.SetActive(false);
        }
        else
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].on = false;
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
            if (i < Resources.Load<BuildingManager>("BuildingManager").buildings.Count)
            {
                provinceWindow.createBuildingOptions[i].buildingType = Resources.Load<BuildingManager>("BuildingManager").buildings[i].buildingType;
                provinceWindow.createBuildingOptions[i].buildingName.text = provinceWindow.createBuildingOptions[i].buildingType.ToString();
                provinceWindow.SetHoldingIndex(holdingCounterpart);
                provinceWindow.createBuildingOptions[i].gameObject.SetActive(true);
            }
            else
            {
                provinceWindow.createBuildingOptions[i].gameObject.SetActive(false);
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

    #region resource selection/buttons
    public void OpenResourceSelection(int outputValue, bool create)
    {
        if (create
            && 
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Count <= outputValue
            &&
            outputValue < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType].outputCapacity)
        {
            for (int i = 0; i < resourceOptions.Count; i++)
            {
                #region Raw Resource Gathering Holding Type
                if (provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.Mine
                    || provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.Logging
                    || provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.Farm)
                {
                    if (i < provinceWindow.provinceTarget.rawResources.Count && Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].creatableResources.Contains(provinceWindow.provinceTarget.rawResources[i].resource))
                    {
                        resourceOptions[i].resource = provinceWindow.provinceTarget.rawResources[i].resource;
                        if (provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Count > 0)
                        {
                            for (int j = 0; j < provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Count; j++)
                            {
                                resourceOptions[i].GetComponent<Image>().sprite = resourceOptions[i].resource.icon;
                                resourceOptions[i].GetComponent<Image>().SetNativeSize();
                                resourceOptions[i].outputValue = outputValue;
                                if (provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput[j].resource == resourceOptions[i].resource)
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
                else if (provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.Factory
                    || provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.GoodsFactory
                    || provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.Smeltery)
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
            }
            provinceWindow.provinceTarget.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
            selectResourceWindow.gameObject.SetActive(true);
        }
        else if (!create && outputValue < provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Count)
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Remove(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput[outputValue]);
            resourceOutputUI[outputValue] = null;
            provinceWindow.provinceTarget.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
        }
    }

    public void SetResourceButtons(Resource resource, bool create, int outputValue)
    {
        if (create)
        {
            provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Add(new Building.ProvinceBuildingResource(resource, 0, 0));
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
    #endregion

    public void Update()
    {
        if (active && hovering && provinceWindow.target == CountryManager.instance.playerCountry)
        {
            //Destroy Building
            //if (Input.GetKeyDown(KeyCode.Delete))
            //{
            //    provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings.Remove(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart]);
            //    
            //    provinceWindow.provinceTarget.RefreshProvinceValues();
            //    provinceWindow.RefreshWindow();
            //}

            #region Dev Pop Adding/Subtracting
            //if (Input.GetKeyDown(KeyCode.KeypadPlus) && provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Count < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].workerCapacity)
            //{
            //    provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].AddPop(Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops[0]);
            //    provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops[provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Count - 1].popTier = 0;
            //    provinceWindow.provinceTarget.RefreshProvinceValues();
            //    provinceWindow.RefreshWindow();
            //}
            //
            //if (Input.GetKeyDown(KeyCode.KeypadMinus) && provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Count > 0)
            //{
            //    provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Remove(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops[provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].pops.Count - 1]);
            //    provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart].DestroyPop();
            //    provinceWindow.provinceTarget.RefreshProvinceValues();
            //    provinceWindow.RefreshWindow();
            //}
            #endregion
        }
        else
        {
            highlight.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        if (provinceWindow.movingPop)
        {
            var movingPop = provinceWindow.movingPop;

            Color newGreen = new Color(CountryManager.instance.niceGreen.r, CountryManager.instance.niceGreen.g, CountryManager.instance.niceGreen.b, 0.5f);
            Color newRed = new Color(CountryManager.instance.niceRed.r, CountryManager.instance.niceRed.g, CountryManager.instance.niceRed.b, 0.5f);

            if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops.Contains(provinceWindow.movingPop.popTier) && buildingCounterpart.pops.Count < popIcons.Count && buildingCounterpart.housedPops.Count < housedPopIcons.Length)
            {
                jobHighlight.color = newGreen;
                housingHighlight.color = newGreen;
            }
            else
            {
                jobHighlight.color = newRed;
                housingHighlight.color = newRed;
            }
            jobHighlight.gameObject.SetActive(true);
            housingHighlight.gameObject.SetActive(true);

            //if (provinceWindow.job)
            //{
            //    jobHighlight.gameObject.SetActive(true);
            //    housingHighlight.gameObject.SetActive(false);
            //}
            //else
            //{
            //    housingHighlight.gameObject.SetActive(true);
            //    jobHighlight.gameObject.SetActive(false);
            //}
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        housingHighlight.gameObject.SetActive(false);
        jobHighlight.gameObject.SetActive(false);
    }

    public void MovePop()
    {
        if (provinceWindow.movingPop && hovering && active && Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops.Contains(provinceWindow.movingPop.popTier))
        {
            var movingPop = provinceWindow.movingPop;
            if (!Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isHousing)
            {
                movingPop.job.pops.Remove(movingPop);
                movingPop.job = buildingCounterpart;
                movingPop.job.pops.Add(movingPop);
            }
            else
            {
                movingPop.home.housedPops.Remove(movingPop);
                movingPop.home = buildingCounterpart;
                movingPop.home.housedPops.Add(movingPop);
                movingPop.popTier = Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops[0];
            }
        }
        provinceWindow.movingPop = null;

        provinceWindow.provinceTarget.RefreshProvinceValues();
        provinceWindow.RefreshWindow();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
