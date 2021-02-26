using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class WindowProvince : InteractableWindow
{
    public Country target;
    public Province provinceTarget;
    //UI
    public TextMeshProUGUI popsCount;
    public TextMeshProUGUI provinceName;
    public TextMeshProUGUI taxText;
    public TextMeshProUGUI unrestText;
    public TextMeshProUGUI supplyLimitText;
    public TextMeshProUGUI religionText;
    public TextMeshProUGUI cultureText;
    public TextMeshProUGUI ideologyText;
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
        RefreshProvinceValues();
    }

    public void RefreshProvinceValues()
    {
        provinceName.text = provinceTarget.name;
        popsCount.text = provinceTarget.pops.Count.ToString();
        //setting backbar color
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

        //province religion, culture, and ideology text
        religionText.text = provinceTarget.religion.religionName;
        cultureText.text = provinceTarget.culture.cultureName;
        ideologyText.text = provinceTarget.ideology.ideologyName;
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
    }
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
    public override void Update()
    {
        base.Update();
        if (gameObject.activeSelf)
        {
            RefreshProvinceValues();
        }

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
