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
    public GameObject createBuildingMarker;
    public GameObject selectBuildingWindow;
    public int selectedBuildingType;
    public BuildingInfoWindow buildingInfoWindow;
    //destroying buildings stuff
    public GameObject destroyBuildingConfirmation;
    //local
    bool markBuildingSpot;

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
        createBuildingMarker.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        CountryManager.instance.openWindows.Add(this);
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
            if (i < provinceTarget.rawResources.Count)
            {
                //switch to finding the buildings output
                commodities[i].Refresh(ResourceManager.instance.resources[i].icon, ResourceManager.instance.resources[i].icon, ResourceManager.instance.resources[i].outline, provinceTarget.rawResources[i].resourceCount);
                commodities[i].gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < rawResources.Length; i++)
        {
            rawResources[i].gameObject.SetActive(false);
            if (i < provinceTarget.rawResources.Count)
            {
                rawResources[i].Refresh(ResourceManager.instance.resources[i].icon, provinceTarget.rawResources[i].resourceCount);
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
                holdings[i].Refresh(i, true);

                for (int j = 0; j < holdings[i].buildings.Length; j++)
                {
                    //setting buildings active
                    holdings[i].buildings[j].gameObject.SetActive(false);
                    if (j < provinceTarget.holdings[i].buildings.Count)
                    {
                        holdings[i].buildings[j].gameObject.SetActive(true);
                        holdings[i].buildings[j].active = true;
                        holdings[i].buildings[j].Refresh(i, j);
                    }
                    if (j == provinceTarget.holdings[i].buildings.Count)
                    {
                        holdings[i].buildings[j].gameObject.SetActive(true);
                        holdings[i].buildings[j].active = false;
                        holdings[i].buildings[j].Refresh(i, j);
                    }
                }
            }
            //show create holding ui (not a real holding)
            if (i == provinceTarget.holdings.Count)
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
