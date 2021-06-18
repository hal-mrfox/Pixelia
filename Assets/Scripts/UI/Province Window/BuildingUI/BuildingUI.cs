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
    public Image notActive;
    public bool fullyInactive;
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
    public Image militaryUI;
    #endregion

    #region Input & Output & Needs

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

    #region Needs
    [BoxGroup("Needs")]
    public InputUIButton[] needsUI;
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

    public List<PopulationUICounterpart> jobPopIcons;

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
    public TextMeshProUGUI popGrowthNumber;
    [BoxGroup("Housing Section")]
    public TextMeshProUGUI inhabitants;
    [BoxGroup("Housing Section")]
    public Image fillBar;
    [BoxGroup("Housing Section")]
    public Image fillBarNext;
    [BoxGroup("Housing Section")]
    public Image fillBarBackground;
    [BoxGroup("Housing Section")]
    public Image popTypeIcon;
    [BoxGroup("Housing Section")]
    public PopulationUICounterpart[] housedPopIcons;
    #endregion

    #region Military

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
        if (active && !fullyInactive)
        {
            createBuildingButton.gameObject.SetActive(false);
            notActive.gameObject.SetActive(false);

            #region Housing
            if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isHousing)
            {
                housingUI.gameObject.SetActive(true);
            }
            else
            {
                housingUI.gameObject.SetActive(false);
            }

            if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isMilitary)
            {
                militaryUI.gameObject.SetActive(true);
            }
            else
            {
                militaryUI.gameObject.SetActive(false);
            }

            //Inhabitants
            var buildingCounterpart = provinceWindow.target.holdings[holding].buildings[building];

            inhabitants.text = buildingCounterpart.housedPops.Count.ToString() + "/" + Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingCounterpart.buildingType].housingCapacity;
            for (int i = 0; i < housedPopIcons.Length; i++)
            {
                if (i < buildingCounterpart.housedPops.Count)
                {
                    Color popColor = PopulationManager.instance.popTierDetails[(int)Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops[0]].popColor;
                    housedPopIcons[i].gameObject.SetActive(true);
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().image.color = popColor;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().normal = popColor;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().hovering = popColor;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().clicked = popColor;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().popCP = buildingCounterpart.housedPops[i];
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().uIType = PopulationUICounterpart.UIType.home;
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().provinceWindow = provinceWindow;
                }
                else
                {
                    housedPopIcons[i].GetComponent<PopulationUICounterpart>().popCP = null;
                    housedPopIcons[i].gameObject.SetActive(false);
                }
            }

            //PopGrowth
            popGrowth.text = "+" + buildingCounterpart.popGrowthMultiplier.ToString("0.00") + "%";
            popGrowthNumber.text = buildingCounterpart.popGrowth.ToString("0.00") +"/" + 100;

            //Fillbar
            fillBar.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, fillBarBackground.rectTransform.sizeDelta.x, buildingCounterpart.popGrowth / 100f), fillBar.rectTransform.sizeDelta.y);
            fillBarNext.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, fillBarBackground.rectTransform.sizeDelta.x, (buildingCounterpart.popGrowth + buildingCounterpart.popGrowthMultiplier) / 100), fillBar.rectTransform.sizeDelta.y);

            //PopTier color thing
            popTypeIcon.color = PopulationManager.instance.popTierDetails[(int)Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingCounterpart.buildingType].allowedPops[0]].popColor;
            #endregion
            #region Pops
            int pops = provinceWindow.target.holdings[holding].buildings[building].workingPops.Count;
            for (int i = 0; i < jobPopIcons.Count; i++)
            {
                if (i < pops)
                {
                    Color popColor = PopulationManager.instance.popTierDetails[(int)provinceWindow.target.holdings[holding].buildings[building].workingPops[i].popTier].popColor;
                    jobPopIcons[i].GetComponent<Image>().color = popColor;
                    jobPopIcons[i].GetComponent<PopulationUICounterpart>().lower = false;
                    jobPopIcons[i].GetComponent<PopulationUICounterpart>().popCP = buildingCounterpart.workingPops[i];
                    jobPopIcons[i].GetComponent<PopulationUICounterpart>().uIType = PopulationUICounterpart.UIType.job;
                    jobPopIcons[i].GetComponent<PopulationUICounterpart>().provinceWindow = provinceWindow;
                }
                else
                {
                    jobPopIcons[i].GetComponent<PopulationUICounterpart>().popCP = null;
                    jobPopIcons[i].GetComponent<Image>().color = empty;
                    jobPopIcons[i].GetComponent<PopulationUICounterpart>().lower = true;
                }
            }

            for (int i = 0; i < housedPopIcons.Length; i++)
            {
                if (i < buildingCounterpart.housedPops.Count)
                {
                    housedPopIcons[i].Refresh();
                }
            }
            #region Highlight
            DeActivatePopHighlights();
            #endregion
            #endregion
            #region Efficiency
            textEfficiency.text = provinceWindow.target.holdings[holding].buildings[building].efficiency.ToString("0") + "%";
            #endregion
            #region Name
            buildingTypeName.text = provinceWindow.target.holdings[holding].buildings[building].buildingType.ToString();
            #endregion
            #region Resources
            #region Output
            for (int i = 0; i < provinceWindow.target.holdings[holding].buildings[building].resourceOutput.Count; i++)
            {
                resourceOutputUI[i] = new ResourceUI(provinceWindow.target.holdings[holding].buildings[building].resourceOutput[i].resource.icon, provinceWindow.target.holdings[holding].buildings[building].resourceOutput[i].resourceCount);
            }
            for (int i = 0; i < outputUI.Length; i++)
            {
                if (i < provinceWindow.target.holdings[holding].buildings[building].resourceOutput.Count)
                {
                    outputUI[i].icon.gameObject.SetActive(true);
                    backgroundUI[i].gameObject.SetActive(true);
                    outputUI[i].icon.sprite = provinceWindow.target.holdings[holding].buildings[building].resourceOutput[i].resource.icon;
                    outputUI[i].icon.SetNativeSize();
                    outputUI[i].icon.color = Color.white;
                    outputUI[i].amount.text = "+" + resourceOutputUI[i].resourceCount.ToString();
                    outputUI[i].resourceName.text = provinceWindow.target.holdings[holding].buildings[building].resourceOutput[i].resource.name;
                }
                else if (i < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceWindow.target.holdings[holding].buildings[building].buildingType].outputCapacity)
                {
                    outputUI[i].icon.gameObject.SetActive(true);
                    backgroundUI[i].gameObject.SetActive(true);
                    outputUI[i].icon.sprite = null;
                    outputUI[i].icon.GetComponent<RectTransform>().sizeDelta = new Vector2(16, 16);
                    outputUI[i].icon.color = grayblue;
                    outputUI[i].amount.text = null;
                    outputUI[i].resourceName.text = null;
                }
                else
                {
                    outputUI[i].icon.gameObject.SetActive(false);
                    backgroundUI[i].gameObject.SetActive(false);
                }
            }
            #endregion
            #region Input
            for (int i = 0; i < provinceWindow.target.holdings[holding].buildings[building].resourceInput.Count; i++)
            {
                resourceInputUI[i] = new ResourceUI(provinceWindow.target.holdings[holding].buildings[building].resourceInput[i].resource.icon, provinceWindow.target.holdings[holding].buildings[building].resourceInput[i].resourceCount);
            }
            for (int i = 0; i < inputUI.Length; i++)
            {
                if (i < provinceWindow.target.holdings[holding].buildings[building].resourceInput.Count)
                {
                    inputUI[i].icon.gameObject.SetActive(true);
                    inputUI[i].icon.sprite = provinceWindow.target.holdings[holding].buildings[building].resourceInput[i].resource.icon;
                    inputUI[i].icon.SetNativeSize();
                    inputUI[i].resource = provinceWindow.target.holdings[holding].buildings[building].resourceInput[i].resource;
                    inputUI[i].outline.gameObject.SetActive(true);
                    inputUI[i].outline.sprite = provinceWindow.target.holdings[holding].buildings[building].resourceInput[i].resource.outline;
                    inputText[i].text = provinceWindow.target.holdings[holding].buildings[building].resourceInput[i].resourceNeedsCount.ToString();
                    inputText[i].gameObject.SetActive(true);

                    if (provinceWindow.target.holdings[holding].buildings[building].resourceInput[i].resourceCount < provinceWindow.target.holdings[holding].buildings[building].resourceInput[i].resourceNeedsCount)
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
        else if (!active && !fullyInactive)
        {
            createBuildingButton.gameObject.SetActive(true);
            notActive.gameObject.SetActive(false);
        }
        else if (!active && fullyInactive)
        {
            createBuildingButton.gameObject.SetActive(false);
            notActive.gameObject.SetActive(true);
        }
    }

    public void DeActivatePopHighlights()
    {
        for (int i = 0; i < jobPopIcons.Count; i++)
        {
            jobPopIcons[i].GetComponent<PopulationUICounterpart>().highlight.gameObject.SetActive(false);
        }
        for (int i = 0; i < housedPopIcons.Length; i++)
        {
            housedPopIcons[i].GetComponent<PopulationUICounterpart>().highlight.gameObject.SetActive(false);
        }
    }

    public void ToggleBuilding(bool on)
    {
        if (on)
        {
            provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].on = true;
            overlay.gameObject.SetActive(false);
        }
        else
        {
            provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].on = false;
            overlay.gameObject.SetActive(true);
        }
        provinceWindow.target.RefreshProvinceValues();
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
        if (create && buildingCounterpart.resourceOutput.Count <= outputValue && outputValue < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].outputCapacity)
        {
            for (int i = 0; i < resourceOptions.Count; i++)
            {
                #region Raw Resource Gathering
                if (buildingType == BuildingType.Mine || buildingType == BuildingType.Logging || buildingType == BuildingType.Farm)
                {
                    if (i < buildingCounterpart.holding.rawResources.Count
                        && Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].creatableResources.Contains(buildingCounterpart.holding.rawResources[i].resource)
                        && buildingCounterpart.holding.rawResources[i].amount > 0)
                    {
                        resourceOptions[i].resource = buildingCounterpart.holding.rawResources[i].resource;
                        if (provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Count > 0)
                        {
                            for (int j = 0; j < buildingCounterpart.resourceOutput.Count; j++)
                            {
                                resourceOptions[i].GetComponent<Image>().sprite = resourceOptions[i].resource.icon;
                                resourceOptions[i].GetComponent<Image>().SetNativeSize();
                                resourceOptions[i].outputValue = outputValue;
                                if (provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput[j].resource == resourceOptions[i].resource)
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
                else if (provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.Factory
                    || provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.GoodsFactory
                    || provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].buildingType == BuildingType.Smeltery)
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
            provinceWindow.target.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
            selectResourceWindow.gameObject.SetActive(true);
        }
        else if (!create && outputValue < provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Count)
        {
            provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Remove(provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput[outputValue]);
            resourceOutputUI[outputValue] = null;
            provinceWindow.target.RefreshProvinceValues();
            provinceWindow.RefreshWindow();
        }
    }

    public void SetResourceButtons(Resource resource, bool create, int outputValue)
    {
        if (create)
        {
            provinceWindow.target.holdings[holdingCounterpart].buildings[buildingCounterpartIndex].resourceOutput.Add(new Building.ProvinceBuildingResource(resource, 0, 0));
            selectResourceWindow.gameObject.SetActive(false);
            provinceWindow.target.RefreshProvinceValues();
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
        //if (active && hovering && provinceWindow.targetCountry == CountryManager.instance.playerCountry)
        //{
        //    //Destroy Building
        //    //if (Input.GetKeyDown(KeyCode.Delete))
        //    //{
        //    //    provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings.Remove(provinceWindow.provinceTarget.holdings[holdingCounterpart].buildings[buildingCounterpart]);
        //    //    
        //    //    provinceWindow.provinceTarget.RefreshProvinceValues();
        //    //    provinceWindow.RefreshWindow();
        //    //}
        //
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
        //}
        //else
        //{
        //    //highlight.gameObject.SetActive(false);
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        if (provinceWindow.movingPop && active)
        {
            var movingPop = provinceWindow.movingPop;

            Color newGreen = new Color(CountryManager.instance.niceGreen.r, CountryManager.instance.niceGreen.g, CountryManager.instance.niceGreen.b, 0.5f);
            Color newRed = new Color(CountryManager.instance.niceRed.r, CountryManager.instance.niceRed.g, CountryManager.instance.niceRed.b, 0.5f);

            if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops.Contains(provinceWindow.movingPop.popTier)
                && buildingCounterpart.workingPops.Count < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].workerCapacity
                || buildingCounterpart.housedPops.Count < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].housingCapacity)
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

    public void CreateUnit()
    {
        buildingCounterpart.CreateUnit();
    }

    public void MovePop()
    {
        var resourcesBuilding = Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType];
        if (provinceWindow.movingPop && hovering && active && resourcesBuilding.allowedPops.Contains(provinceWindow.movingPop.popTier) && buildingCounterpart.workingPops.Count < resourcesBuilding.workerCapacity || buildingCounterpart.housedPops.Count < resourcesBuilding.housingCapacity)
        {
            var movingPop = provinceWindow.movingPop;
            if (!resourcesBuilding.isHousing)
            {
                movingPop.workingHolding.pops.Remove(movingPop);
                if (movingPop.job)
                {
                    movingPop.job.workingPops.Remove(movingPop);
                    movingPop.job.holding.pops.Remove(movingPop);
                }
                movingPop.job = buildingCounterpart;
                movingPop.job.workingPops.Add(movingPop);
                movingPop.transform.SetParent(buildingCounterpart.transform);

                if (!movingPop.job.holding.pops.Contains(movingPop))
                {
                    movingPop.job.holding.pops.Add(movingPop);
                }
            }
            else
            {
                if (movingPop.home)
                {
                    movingPop.home.housedPops.Remove(movingPop);
                }
                movingPop.home = buildingCounterpart;
                movingPop.home.housedPops.Add(movingPop);
                movingPop.popTier = resourcesBuilding.allowedPops[0];
            }
            movingPop.workingHolding.RefreshValues();
        }
        provinceWindow.movingPop = null;

        provinceWindow.target.RefreshProvinceValues();
        provinceWindow.RefreshWindow();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void DestroyBuilding()
    {
        buildingCounterpart.DestroyBuilding();

        provinceWindow.target.RefreshProvinceValues();
        provinceWindow.RefreshWindow();
    }
}
