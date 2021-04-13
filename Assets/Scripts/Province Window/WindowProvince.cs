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

    #region Create Improvements

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

    [BoxGroup("Icons")]
    public Sprite[] buildingIcons;

    [System.Serializable]
    public class CreateImprovement
    {
        public Image window;
        public ButtonSound[] options;
    }

    int holdingIndex;

    #endregion

    #region Building Hover UI
    public bool altMode;
    public OutputUIButton hoveredOutput;
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
        createHolding.window.gameObject.SetActive(false);
        createBuildingWindow.gameObject.SetActive(false);
        OnClicked();
    }

    //Set every value on enable here!
    public void OnClicked()
    {
        IfPlayer();
        RefreshWindow();
    }

    public void RefreshWindow()
    {
        #region Commodities & Raw Resources
        for (int i = 0; i < commodities.Length; i++)
        {
            commodities[i].gameObject.SetActive(false);
            if (i < provinceTarget.storedResources.Count)
            {
                //switch to finding the buildings output
                commodities[i].Refresh(provinceTarget.storedResources[i].resource.icon, provinceTarget.storedResources[i].resource.icon, provinceTarget.storedResources[i].resource.outline, provinceTarget.storedResources[i].resourceCount);
                commodities[i].gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < rawResources.Length; i++)
        {
            rawResources[i].gameObject.SetActive(false);
            if (i < provinceTarget.rawResources.Count)
            {
                rawResources[i].Refresh(provinceTarget.rawResources[i].resource.icon, provinceTarget.rawResources[i].resourceCount);
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
                holdings[i].Refresh(i, true);

                for (int j = 0; j < holdings[i].buildings.Length; j++)
                {
                    //setting buildings active
                    holdings[i].buildings[j].gameObject.SetActive(false);
                    if (j < provinceTarget.holdings[i].buildings.Count)
                    {
                        holdings[i].buildings[j].gameObject.SetActive(true);
                        holdings[i].buildings[j].active = true;
                        holdings[i].buildings[j].holdingCounterpart = i;
                        holdings[i].buildings[j].buildingCounterpart = j;
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
                holdings[i].Refresh(i, false);
                for (int j = 0; j < holdings[i].buildings.Length; j++)
                {
                    holdings[i].buildings[j].gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }

    public void SetHoldingIndex(int holdingIndex)
    {
        this.holdingIndex = holdingIndex;
    }

    public void CreateHolding(int holdingType)
    {
        provinceTarget.CreateHolding(holdingType);
        createHolding.window.gameObject.SetActive(false);
        provinceTarget.RefreshProvinceValues();
        RefreshWindow();
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
