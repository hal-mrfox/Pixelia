﻿using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class WindowProvince : InteractableWindow
{
    #region Main
    [BoxGroup("Main")]
    public Country target;
    [BoxGroup("Main")]
    public Province provinceTarget;
    [BoxGroup("Main")]
    public TextMeshProUGUI provinceName;
    #endregion
    #region General
    [BoxGroup("General")]
    public TextMeshProUGUI taxText;
    [BoxGroup("General")]
    public TextMeshProUGUI unrestText;
    [BoxGroup("General")]
    public TextMeshProUGUI supplyLimitText;
    [BoxGroup("General")]
    public TextMeshProUGUI holdingCapacity;
    #endregion
    #region Demographics
    [BoxGroup("Demographics")]
    public TextMeshProUGUI popsCount;
    [BoxGroup("Demographics")]
    public TextMeshProUGUI religionText;
    [BoxGroup("Demographics")]
    public TextMeshProUGUI cultureText;
    [BoxGroup("Demographics")]
    public TextMeshProUGUI ideologyText;
    [BoxGroup("Demographics")]
    public TextMeshProUGUI nationalityText;
    [BoxGroup("Demographics")]
    public Image popListUI;
    [BoxGroup("Demographics")]
    public List<PopulationUICounterpart> popList;
    [BoxGroup("Demographics")]
    public PopulationUICounterpart popListPrefab;
    [BoxGroup("Demographics")]
    public GridLayoutGroup popListGridLayout;
    bool isPopListOpen = false;
    #endregion
    #region Commodities & Resources
    [BoxGroup("Commodities & Resources")]
    public Commodity[] commodities;
    [BoxGroup("Commodities & Resources")]
    public RawResourceUI[] rawResources;
    #endregion
    public Image capitolIcon;
    public Image backBar;
    bool isPlayer;
    //move to province
    public int overPopulation;
    public int snapDistance;
    //Sounds
    public AudioSource audioSource;

    #region Improvements

    #region Create Holding
    [BoxGroup("Holding Creation")]
    public CreateImprovement createHolding;
    #endregion

    #region Create Building
    [BoxGroup("Building Creation")]
    public Image createBuildingWindow;
    [BoxGroup("Building Creation")]
    public PopupOption[] createBuildingOptions;
    #endregion

    [System.Serializable]
    public class CreateImprovement
    {
        public Image window;
        public ButtonSound[] options;
    }

    int holdingIndex;

    public Population movingPop;
    public bool job;
    #endregion

    #region Building Hover UI
    public bool altMode;
    public OutputUIButton hoveredOutput;
    public List<InputUIButton> hoveredOutputConnections;
    public OutputUIButton selectedOutput;

    public void SelectingOutput()
    {
        for (int i = 0; i < holdings.Length; i++)
        {
            for (int j = 0; j < holdings[i].buildings.Length; j++)
            {
                for (int k = 0; k < holdings[i].buildings[j].outputUI.Length; k++)
                {
                    if (selectedOutput == holdings[i].buildings[j].outputUI[k])
                    {
                        holdings[i].buildings[j].outputUI[k].outline.gameObject.SetActive(true);
                    }
                    else
                    {
                        holdings[i].buildings[j].outputUI[k].outline.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void HoveringOutput()
    {
        for (int i = 0; i < holdings.Length; i++)
        {
            for (int j = 0; j < holdings[i].buildings.Length; j++)
            {
                for (int k = 0; k < holdings[i].buildings[j].outputUI.Length; k++)
                {
                    if (hoveredOutput == holdings[i].buildings[j].outputUI[k])
                    {
                        holdings[i].buildings[j].outputUI[k].outline.gameObject.SetActive(true);
                    }
                    else
                    {
                        holdings[i].buildings[j].outputUI[k].outline.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    #endregion

    #region old building making stuff
    public GameObject createBuildingMarker;
    public GameObject selectBuildingWindow;
    public int selectedBuildingType;
    public BuildingInfoWindow buildingInfoWindow;
    //destroying buildings stuff
    public GameObject destroyBuildingConfirmation;
    //local
    bool markBuildingSpot;
    #endregion

    [Space(20)]
    public HoldingUI[] holdings;
    const float HoldingHeight = 85.1764f;
    const float HoldingPadding = 5f;

    public void Scroll(float value)
    {
        float maxHeight = HoldingHeight * holdings.Length;

        for (int i = 0; i < holdings.Length; i++)
        {
            RectTransform transform = (RectTransform)holdings[i].transform;
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, -(transform.rect.height + HoldingPadding) * i + Mathf.Lerp(0, maxHeight / 2, value));
        }
    }

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        CountryManager.instance.openWindows.Add(this);
        createBuildingWindow.gameObject.SetActive(false);
        OnClicked();
    }

    //Set every value on enable here!
    public void OnClicked()
    {
        IfPlayer();
        RefreshWindow();
    }

    #region Highlighting Pops
    public void HighlightPop(Population highlightedPop, PopulationUICounterpart.UIType type)
    {
        #region list
        var listPopIconsGameObject = popList[provinceTarget.pops.IndexOf(highlightedPop)].highlight.gameObject;
        #endregion

        if (type == PopulationUICounterpart.UIType.home)
        {
            if (highlightedPop.job)
            {
                GameObject job = GetJob();
                job.SetActive(true);
            }

            listPopIconsGameObject.SetActive(true);
        }
        else if (type == PopulationUICounterpart.UIType.job)
        {
            if (highlightedPop.home)
            {
                GameObject home = GetHome();
                home.SetActive(true);
            }

            listPopIconsGameObject.SetActive(true);
        }
        else
        {
            if (highlightedPop.job)
            {
                GameObject job = GetJob();
                job.SetActive(true);
            }

            if (highlightedPop.home)
            {
                GameObject home = GetHome();
                home.SetActive(true);
            }
        }

        #region Job
        GameObject GetJob()
        {
            int jobHolding = provinceTarget.holdings.IndexOf(highlightedPop.job.holding);
            int jobBuilding = provinceTarget.holdings[jobHolding].buildings.IndexOf(highlightedPop.job);
            int jobPop = provinceTarget.holdings[jobHolding].buildings[jobBuilding].pops.IndexOf(highlightedPop);
            var jobPopIconsGameObject = holdings[jobHolding].buildings[jobBuilding].jobPopIcons[jobPop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

            return jobPopIconsGameObject;
        }
        #endregion

        #region Home
        GameObject GetHome()
        {
            int homeHolding = provinceTarget.holdings.IndexOf(highlightedPop.home.holding);
            int homeBuilding = provinceTarget.holdings[homeHolding].buildings.IndexOf(highlightedPop.home);
            int homePop = provinceTarget.holdings[homeHolding].buildings[homeBuilding].housedPops.IndexOf(highlightedPop);
            var homePopIconsGameObject = holdings[homeHolding].buildings[homeBuilding].housedPopIcons[homePop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

            return homePopIconsGameObject;
        }
        #endregion
    }

    public void UnHighlightPops(Population highlightedPop, PopulationUICounterpart.UIType type)
    {
        var listPopIconsGameObject = popList[provinceTarget.pops.IndexOf(highlightedPop)].highlight.gameObject;

        if (type == PopulationUICounterpart.UIType.home)
        {
            if (highlightedPop.job)
            {
                GameObject job = GetJob();
                job.SetActive(false);
            }

            listPopIconsGameObject.SetActive(false);
        }
        else if (type == PopulationUICounterpart.UIType.job)
        {
            if (highlightedPop.home)
            {
                GameObject home = GetHome();
                home.SetActive(false);
            }

            listPopIconsGameObject.SetActive(false);
        }
        else
        {
            if (highlightedPop.job)
            {
                GameObject job = GetJob();
                job.SetActive(false);
            }

            if (highlightedPop.home)
            {
                GameObject home = GetHome();
                home.SetActive(false);
            }
        }


        #region Job
        GameObject GetJob()
        {
            int jobHolding = provinceTarget.holdings.IndexOf(highlightedPop.job.holding);
            int jobBuilding = provinceTarget.holdings[jobHolding].buildings.IndexOf(highlightedPop.job);
            int jobPop = provinceTarget.holdings[jobHolding].buildings[jobBuilding].pops.IndexOf(highlightedPop);
            var jobPopIconsGameObject = holdings[jobHolding].buildings[jobBuilding].jobPopIcons[jobPop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

            return jobPopIconsGameObject;
        }
        #endregion

        #region Home
        GameObject GetHome()
        {
            int homeHolding = provinceTarget.holdings.IndexOf(highlightedPop.home.holding);
            int homeBuilding = provinceTarget.holdings[homeHolding].buildings.IndexOf(highlightedPop.home);
            int homePop = provinceTarget.holdings[homeHolding].buildings[homeBuilding].housedPops.IndexOf(highlightedPop);
            var homePopIconsGameObject = holdings[homeHolding].buildings[homeBuilding].housedPopIcons[homePop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

            return homePopIconsGameObject;
        }
        #endregion
    }
    #endregion

    public void DropPop()
    {
        for (int i = 0; i < holdings.Length; i++)
        {
            for (int j = 0; j < holdings[i].buildings.Length; j++)
            {
                var house = holdings[i].buildings[j].housingHighlight;
                var job = holdings[i].buildings[j].jobHighlight;

                if (house.gameObject.activeSelf || job.gameObject.activeSelf)
                {
                    holdings[i].buildings[j].MovePop();
                    house.gameObject.SetActive(false);
                    job.gameObject.SetActive(false);

                    goto end;
                }
            }
        }

    end:

        return;
    }

    public void RefreshWindow()
    {
        #region Commodities & Raw Resources
        for (int i = 0; i < commodities.Length; i++)
        {
            commodities[i].gameObject.SetActive(false);
            if (i < provinceTarget.storedResources.Count)
            {
                int resourceOutputValue = 0;
                //shows output number as "+000"
                for (int j = 0; j < provinceTarget.holdings.Count; j++)
                {
                    for (int k = 0; k < provinceTarget.holdings[j].buildings.Count; k++)
                    {
                        //adding
                        for (int h = 0; h < provinceTarget.holdings[j].buildings[k].resourceOutput.Count; h++)
                        {
                            if (provinceTarget.storedResources[i].resource == provinceTarget.holdings[j].buildings[k].resourceOutput[h].resource)
                            {
                                resourceOutputValue += provinceTarget.holdings[j].buildings[k].resourceOutput[h].resourceCount;
                            }
                        }

                        //subtracting
                        for (int o = 0; o < provinceTarget.holdings[j].buildings[k].resourceInput.Count; o++)
                        {
                            if (provinceTarget.storedResources[i].resource == provinceTarget.holdings[j].buildings[k].resourceInput[o].resource
                                && provinceTarget.holdings[j].buildings[k].resourceOutput[0].resourceCount != 0)
                            {
                                resourceOutputValue -= provinceTarget.holdings[j].buildings[k].resourceInput[o].resourceNeedsCount;
                            }
                        }
                    }
                }
                commodities[i].Refresh(provinceTarget.storedResources[i].resource.icon, provinceTarget.storedResources[i].resourceCount, resourceOutputValue);
                commodities[i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < rawResources.Length; i++)
        {
            rawResources[i].gameObject.SetActive(false);
            if (i < provinceTarget.rawResources.Count)
            {
                rawResources[i].Refresh(provinceTarget.rawResources[i].resource.icon, provinceTarget.rawResources[i].quality);
                rawResources[i].gameObject.SetActive(true);
            }
        }
        #endregion
        #region Name and Colors
        provinceName.text = provinceTarget.name;
        if (isPlayer)
        {
            backBar.color = CountryManager.instance.green;
        }
        else
        {
            backBar.color = CountryManager.instance.red;
        }
        //set capitol icon active if it is capitol
        capitolIcon.gameObject.SetActive(provinceTarget == CountryManager.instance.playerCountry.capitalProvince);
        #endregion
        #region Demographics
        popsCount.text = provinceTarget.pops.Count.ToString();
        //province religion, culture, and ideology text
        //religionText.text = provinceTarget.religion.religionName;
        //cultureText.text = provinceTarget.culture.cultureName;
        //ideologyText.text = provinceTarget.ideology.ideologyName;
        if (provinceTarget.religion == CountryManager.instance.playerCountry.religion)
        {
            religionText.color = CountryManager.instance.green;
        }
        else
        {
            religionText.color = CountryManager.instance.red;
        }

        if (provinceTarget.culture == CountryManager.instance.playerCountry.culture)
        {
            cultureText.color = CountryManager.instance.green;
        }
        else
        {
            cultureText.color = CountryManager.instance.red;
        }

        if (provinceTarget.ideology == CountryManager.instance.playerCountry.ideology)
        {
            ideologyText.color = CountryManager.instance.green;
        }
        else
        {
            ideologyText.color = CountryManager.instance.red;
        }

        if (provinceTarget.pops.Count == 0)
        {
            religionText.text = "-";
            religionText.color = CountryManager.instance.yellow;
            cultureText.text = "-";
            cultureText.color = CountryManager.instance.yellow;
            ideologyText.text = "-";
            ideologyText.color = CountryManager.instance.yellow;
        }

        #endregion
        #region Holdings and Buildings
        for (int i = 0; i < holdings.Length; i++)
        {
            holdings[i].gameObject.SetActive(false);
            if (i < provinceTarget.holdings.Count)
            {
                holdings[i].gameObject.SetActive(true);
                holdings[i].holdingCounterpart = i;
                holdings[i].Refresh(true);

                for (int j = 0; j < holdings[i].buildings.Length; j++)
                {
                    //setting buildings active
                    holdings[i].buildings[j].gameObject.SetActive(false);
                    if (j < provinceTarget.holdings[i].buildings.Count)
                    {
                        holdings[i].buildings[j].gameObject.SetActive(true);
                        holdings[i].buildings[j].active = true;
                        holdings[i].buildings[j].holdingCounterpart = i;
                        holdings[i].buildings[j].buildingCounterpartIndex = j;
                        holdings[i].buildings[j].buildingCounterpart = provinceTarget.holdings[i].buildings[j];
                        holdings[i].buildings[j].buildingType = provinceTarget.holdings[i].buildings[j].buildingType;
                        holdings[i].buildings[j].Refresh(i, j);
                    }
                    if (j == provinceTarget.holdings[i].buildings.Count && provinceTarget.owner == CountryManager.instance.playerCountry)
                    {
                        holdings[i].buildings[j].holdingCounterpart = i;
                        holdings[i].buildings[j].gameObject.SetActive(true);
                        holdings[i].buildings[j].active = false;
                        holdings[i].buildings[j].Refresh(i, j);
                    }
                }
            }
            //show create holding ui (not a real holding)
            if (i == provinceTarget.holdings.Count && provinceTarget.owner == CountryManager.instance.playerCountry)
            {
                holdings[i].gameObject.SetActive(true);
                holdings[i].Refresh(false);
                for (int j = 0; j < holdings[i].buildings.Length; j++)
                {
                    holdings[i].buildings[j].gameObject.SetActive(false);
                }
            }
        }
        #endregion
        #region PopList
        for (int i = 0; i < popList.Count; i++)
        {
            Destroy(popList[i].gameObject);
        }
        popList.Clear();

        for (int i = 0; i < provinceTarget.pops.Count; i++)
        {
            popList.Add(Instantiate(popListPrefab, popListGridLayout.transform));
        }

        for (int i = 0; i < popList.Count; i++)
        {
            popList[i].popCP = provinceTarget.pops[i];
            popList[i].provinceWindow = this;
            popList[i].uIType = PopulationUICounterpart.UIType.list;
            popList[i].popTierUIStrip.color = PopulationManager.instance.popTierDetails[(int)popList[i].popCP.popTier].popColor;
            popList[i].popNameText.text = provinceTarget.pops[i].name;

            if (provinceTarget.pops[i].job)
            {
                popList[i].jobStatus.text = provinceTarget.pops[i].job.name;
            }
            else
            {
                popList[i].jobStatus.text = "unemployed";
            }

            if (provinceTarget.pops[i].home)
            {
                popList[i].homeStatus.text = provinceTarget.pops[i].home.name;
            }
            else
            {
                popList[i].homeStatus.text = "homeless";
            }
        }
        #endregion

        target.CalculateResources();
    }

    public void TogglePopList()
    {
        isPopListOpen = !isPopListOpen;

        if (isPopListOpen)
        {
            popListUI.gameObject.SetActive(true);
        }
        else
        {
            popListUI.gameObject.SetActive(false);
        }
    }

    public void SetHoldingIndex(int holdingIndex)
    {
        this.holdingIndex = holdingIndex;
    }

    public void CreateBuilding(BuildingType buildingType)
    {
        provinceTarget.CreateBuilding(holdingIndex, buildingType);
        createBuildingWindow.gameObject.SetActive(false);
        provinceTarget.RefreshProvinceValues();
        RefreshWindow();
    }

    #region old building stuff to get rid of
    public void BuildingButton(int buildingNumber)
    {
        if (buildingNumber < provinceTarget.holdings.Count && isPlayer)
        {
            buildingInfoWindow.gameObject.SetActive(true);
            //selectedHolding = provinceTarget.holdings[buildingNumber];
            buildingInfoWindow.OnEnable();
            //selectedHolding.GetComponent<Image>().color = CountryManager.instance.yellow;
        }
        else if (buildingNumber == provinceTarget.holdings.Count && target == CountryManager.instance.playerCountry)
        {
            OpenCreateBuildingWindow();
        }
        else if (target != CountryManager.instance.playerCountry)
        {
            print("You cannot edit other countries provinces!");
        }
    }
    public void OpenCreateBuildingWindow()
    {
        //open the select building window and create a new building
        if (target == CountryManager.instance.playerCountry)
        {
            selectBuildingWindow.gameObject.SetActive(true);
        }
        else
        {
            print("You cannot edit other countries provinces!");
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            selectBuildingWindow.gameObject.SetActive(false);
        }
    }
    public void BuildingCreation(int buildingID)
    {
        //toggle building marker
        //if has funds and is in building capacity
        if (provinceTarget.holdings.Count < provinceTarget.buildingCapacity)
        {
            selectedBuildingType = buildingID;
            markBuildingSpot = true;
            createBuildingMarker.SetActive(true);
            CountryManager.instance.available = false;
        }
        else
        {
            print("You have reached this provinces building capacity!");
        }
    }
    //Building info stuff!
    public void BuildingDestructionConfirmation()
    {
        destroyBuildingConfirmation.gameObject.SetActive(true);
    }
    public void BuildingDestructionNo()
    {
        destroyBuildingConfirmation.gameObject.SetActive(false);
    }
    public void BuildingDestructionYes()
    {
        destroyBuildingConfirmation.gameObject.SetActive(false);
        buildingInfoWindow.CloseWindow();
    }
    #endregion

    public override void Update()
    {
        base.Update();

        //closing building selection window with escape (change to the interactbaleWindow script later!)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseBuildingWindow();
        }
    }

    //window closing
    public void CloseBuildingWindow()
    {
        markBuildingSpot = false;
        createBuildingMarker.SetActive(false);
        selectBuildingWindow.SetActive(false);

        CountryManager.instance.available = true;
    }
    //eventually make it so players cant edit non players provinces
    public void IfPlayer()
    {
        if (target == CountryManager.instance.playerCountry)
        {
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
        }
    }
}
