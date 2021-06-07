using System.Collections;
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
    public Country targetCountry;
    [BoxGroup("Main")]
    public Province target;
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

    public GameObject holdingsLayoutGroup;

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
        var listPopIconsGameObject = popList[target.pops.IndexOf(highlightedPop)].highlight.gameObject;
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
            int jobHolding = target.holdings.IndexOf(highlightedPop.job.holding);
            int jobBuilding = target.holdings[jobHolding].buildings.IndexOf(highlightedPop.job);
            int jobPop = target.holdings[jobHolding].buildings[jobBuilding].pops.IndexOf(highlightedPop);
            var jobPopIconsGameObject = holdings[jobHolding].buildings[jobBuilding].jobPopIcons[jobPop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

            return jobPopIconsGameObject;
        }
        #endregion

        #region Home
        GameObject GetHome()
        {
            int homeHolding = target.holdings.IndexOf(highlightedPop.home.holding);
            int homeBuilding = target.holdings[homeHolding].buildings.IndexOf(highlightedPop.home);
            int homePop = target.holdings[homeHolding].buildings[homeBuilding].housedPops.IndexOf(highlightedPop);
            var homePopIconsGameObject = holdings[homeHolding].buildings[homeBuilding].housedPopIcons[homePop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

            return homePopIconsGameObject;
        }
        #endregion
    }

    public void UnHighlightPops(Population highlightedPop, PopulationUICounterpart.UIType type)
    {
        var listPopIconsGameObject = popList[target.pops.IndexOf(highlightedPop)].highlight.gameObject;

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
            int jobHolding = target.holdings.IndexOf(highlightedPop.job.holding);
            int jobBuilding = target.holdings[jobHolding].buildings.IndexOf(highlightedPop.job);
            int jobPop = target.holdings[jobHolding].buildings[jobBuilding].pops.IndexOf(highlightedPop);
            var jobPopIconsGameObject = holdings[jobHolding].buildings[jobBuilding].jobPopIcons[jobPop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

            return jobPopIconsGameObject;
        }
        #endregion

        #region Home
        GameObject GetHome()
        {
            int homeHolding = target.holdings.IndexOf(highlightedPop.home.holding);
            int homeBuilding = target.holdings[homeHolding].buildings.IndexOf(highlightedPop.home);
            int homePop = target.holdings[homeHolding].buildings[homeBuilding].housedPops.IndexOf(highlightedPop);
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
        #region Name and Colors
        provinceName.text = target.name;
        if (isPlayer)
        {
            backBar.color = CountryManager.instance.green;
        }
        else
        {
            backBar.color = CountryManager.instance.red;
        }
        //set capitol icon active if it is capitol
        capitolIcon.gameObject.SetActive(target == CountryManager.instance.playerCountry.capitalProvince);
        #endregion
        #region Demographics
        //popsCount.text = target.pops.Count.ToString();
        //province religion, culture, and ideology text
        //religionText.text = provinceTarget.religion.religionName;
        //cultureText.text = provinceTarget.culture.cultureName;
        //ideologyText.text = provinceTarget.ideology.ideologyName;
        //if (target.religion == CountryManager.instance.playerCountry.religion)
        //{
        //    religionText.color = CountryManager.instance.green;
        //}
        //else
        //{
        //    religionText.color = CountryManager.instance.red;
        //}
        //
        //if (target.culture == CountryManager.instance.playerCountry.culture)
        //{
        //    cultureText.color = CountryManager.instance.green;
        //}
        //else
        //{
        //    cultureText.color = CountryManager.instance.red;
        //}
        //
        //if (target.ideology == CountryManager.instance.playerCountry.ideology)
        //{
        //    ideologyText.color = CountryManager.instance.green;
        //}
        //else
        //{
        //    ideologyText.color = CountryManager.instance.red;
        //}
        //
        //if (target.pops.Count == 0)
        //{
        //    religionText.text = "-";
        //    religionText.color = CountryManager.instance.yellow;
        //    cultureText.text = "-";
        //    cultureText.color = CountryManager.instance.yellow;
        //    ideologyText.text = "-";
        //    ideologyText.color = CountryManager.instance.yellow;
        //}

        #endregion
        #region Holdings and Buildings
        for (int i = 0; i < holdings.Length; i++)
        {
            holdings[i].gameObject.SetActive(false);

            int childIndex = holdingsLayoutGroup.transform.childCount - 1;
            GameObject lastChild = holdingsLayoutGroup.transform.GetChild(childIndex).gameObject;

            if (i < target.holdings.Count && target.holdings[i].owner == CountryManager.instance.playerCountry && holdings[i].gameObject == lastChild)
            {
                holdings[i].gameObject.SetActive(true);
                holdings[i].holdingCounterpartIndex = i;
                holdings[i].holdingCounterpart = target.holdings[i];
                holdings[i].Refresh(true);

                for (int j = 0; j < holdings[i].buildings.Length; j++)
                {
                    //setting buildings active
                    holdings[i].buildings[j].gameObject.SetActive(false);
                    if (j < target.holdings[i].buildings.Count && target.holdings[i].owner == CountryManager.instance.playerCountry)
                    {
                        holdings[i].buildings[j].gameObject.SetActive(true);
                        holdings[i].buildings[j].active = true;
                        holdings[i].buildings[j].notActiveBool = false;
                        holdings[i].buildings[j].holdingCounterpart = i;
                        holdings[i].buildings[j].buildingCounterpartIndex = j;
                        holdings[i].buildings[j].buildingCounterpart = target.holdings[i].buildings[j];
                        holdings[i].buildings[j].buildingType = target.holdings[i].buildings[j].buildingType;
                        holdings[i].buildings[j].Refresh(i, j);
                    }
                    else if (j == target.holdings[i].buildings.Count && target.holdings[i].owner == CountryManager.instance.playerCountry)
                    {
                        holdings[i].buildings[j].holdingCounterpart = i;
                        holdings[i].buildings[j].gameObject.SetActive(true);
                        holdings[i].buildings[j].notActiveBool = false;
                        holdings[i].buildings[j].active = false;
                        holdings[i].buildings[j].Refresh(i, j);
                    }
                    else if (j > target.holdings[i].buildings.Count && target.holdings[i].owner == CountryManager.instance.playerCountry)
                    {
                        holdings[i].buildings[j].holdingCounterpart = i;
                        holdings[i].buildings[j].gameObject.SetActive(true);
                        holdings[i].buildings[j].active = false;
                        holdings[i].buildings[j].notActiveBool = true;
                        holdings[i].buildings[j].Refresh(i, j);
                    }
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

        for (int i = 0; i < target.pops.Count; i++)
        {
            popList.Add(Instantiate(popListPrefab, popListGridLayout.transform));
        }

        for (int i = 0; i < popList.Count; i++)
        {
            popList[i].popCP = target.pops[i];
            popList[i].provinceWindow = this;
            popList[i].uIType = PopulationUICounterpart.UIType.list;
            popList[i].popTierUIStrip.color = PopulationManager.instance.popTierDetails[(int)popList[i].popCP.popTier].popColor;

            if (target.pops[i].job)
            {
                popList[i].jobStatus.color = Resources.Load<UIManager>("UIManager").popWhite;
            }
            else
            {
                popList[i].jobStatus.color = Resources.Load<UIManager>("UIManager").popGray;
            }

            if (target.pops[i].home)
            {
                popList[i].homeStatus.color = Resources.Load<UIManager>("UIManager").popWhite;
            }
            else
            {
                popList[i].homeStatus.color = Resources.Load<UIManager>("UIManager").popGray;
            }
        }
        #endregion

        targetCountry.CalculateResources();
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
        target.CreateBuilding(holdingIndex, buildingType);
        createBuildingWindow.gameObject.SetActive(false);
        target.RefreshProvinceValues();
        RefreshWindow();
    }

    #region old building stuff to get rid of
    public void BuildingButton(int buildingNumber)
    {
        if (buildingNumber < target.holdings.Count && isPlayer)
        {
            buildingInfoWindow.gameObject.SetActive(true);
            //selectedHolding = provinceTarget.holdings[buildingNumber];
            buildingInfoWindow.OnEnable();
            //selectedHolding.GetComponent<Image>().color = CountryManager.instance.yellow;
        }
        else if (buildingNumber == target.holdings.Count && targetCountry == CountryManager.instance.playerCountry)
        {
            OpenCreateBuildingWindow();
        }
        else if (targetCountry != CountryManager.instance.playerCountry)
        {
            print("You cannot edit other countries provinces!");
        }
    }
    public void OpenCreateBuildingWindow()
    {
        //open the select building window and create a new building
        if (targetCountry == CountryManager.instance.playerCountry)
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
        if (target.holdings.Count < target.buildingCapacity)
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
        if (targetCountry == CountryManager.instance.playerCountry)
        {
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
        }
    }
}
