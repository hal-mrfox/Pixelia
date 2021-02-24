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
    public ProvinceScript provinceTarget;
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
    public List<Button> buildingSlots;
    //sub windows
    public OldBuilding selectedBuilding;
    //seeing building info window stuff
    public BuildingInfoWindow buildingInfoWindow;
    //creating new buildings stuff
    public int snapDistance;
    public GameObject createBuildingMarker;
    public GameObject selectBuildingWindow;
    public int selectedBuildingType;
    //destroying buildings stuff
    public GameObject destroyBuildingConfirmation;
    //local
    bool markBuildingSpot;

    public void Awake()
    {
        createBuildingMarker.SetActive(false);
    }
    public void OnEnable()
    {
        CountryManager.instance.openWindows.Add(this);
        OnClicked();
    }
    public void OnDisable()
    {
        CountryManager.instance.openWindows.Remove(this);
        if (selectedBuilding != null)
        {
            selectedBuilding.RefreshColor();
        }
    }

    //Set every value on enable here!
    public void OnClicked()
    {
        IfPlayer();
        RefreshProvinceValues();
    }

    public void RefreshProvinceValues()
    {
        SetBuildings();
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
        //seeing if over capacity
        int capacity = 0;
        for (int i = 0; i < provinceTarget.buildings.Count; i++)
        {
            capacity += provinceTarget.buildings[i].popCapacity;
        }

        if (provinceTarget.pops.Count > capacity)
        {
            popsCount.color = CountryManager.instance.red;
            overPopulation = provinceTarget.pops.Count - capacity;
        }
        else
        {
            popsCount.color = CountryManager.instance.green;
        }

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
        if (buildingNumber < provinceTarget.buildings.Count && isPlayer)
        {
            if (selectedBuilding != null)
            {
                selectedBuilding.RefreshColor();
            }
            buildingInfoWindow.gameObject.SetActive(true);
            selectedBuilding = provinceTarget.buildings[buildingNumber];
            buildingInfoWindow.OnEnable();
            selectedBuilding.GetComponent<Image>().color = CountryManager.instance.yellow;
        }
        else if (buildingNumber == provinceTarget.buildings.Count && target == CountryManager.instance.playerCountry)
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
        if (provinceTarget.buildings.Count < provinceTarget.buildingCapacity)
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
    public void SetBuildings()
    {
        for (int i = 0; i < buildingSlots.Count; i++)
        {
            if (i < provinceTarget.buildings.Count)
            {
                buildingSlots[i].gameObject.SetActive(true);
                buildingSlots[i].GetComponent<Image>().color = CountryManager.instance.tan;
            }
            else if (i == provinceTarget.buildings.Count)
            {
                buildingSlots[i].gameObject.SetActive(true);
                buildingSlots[i].GetComponent<Image>().color = CountryManager.instance.green;
            }
            else if (i > provinceTarget.buildings.Count)
            {
                buildingSlots[i].gameObject.SetActive(false);
            }
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
        selectedBuilding.DestroyBuilding();
        SetBuildings();
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

        if (markBuildingSpot == true)
        {
            Vector2 coordinates = Input.mousePosition;
            coordinates.x = Mathf.Round(coordinates.x / snapDistance) * snapDistance;
            coordinates.y = Mathf.Round(coordinates.y / snapDistance) * snapDistance;

            createBuildingMarker.transform.position = coordinates;

            if (Input.GetKeyDown(KeyCode.Mouse0) && provinceTarget.hovering && provinceTarget.buildings.Count < provinceTarget.buildingCapacity)
            {
                provinceTarget.buildings.Add(Instantiate(CountryManager.instance.buildingPrefab));
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].gameObject.transform.position = coordinates;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].transform.parent = provinceTarget.buildingsParent.transform;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].provinceController = provinceTarget;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].buildingType = (OldBuilding.BuildingType)selectedBuildingType;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].name = provinceTarget.name + "'s " + provinceTarget.buildings[provinceTarget.buildings.Count - 1].buildingType;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].controller = provinceTarget.owner;
                //pop capacity
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].popCapacity = 6;
                CountryManager.instance.totalBuildings.Add(provinceTarget.buildings[provinceTarget.buildings.Count - 1]);
                target.buildings.Add(provinceTarget.buildings[provinceTarget.buildings.Count - 1]);
                if (target.buildings.Count == 1)
                {
                    target.capital = provinceTarget.buildings[provinceTarget.buildings.Count - 1];
                }
            }
            else if (provinceTarget.buildings.Count >= provinceTarget.buildingCapacity)
            {
                print("You have reached this provinces building capacity!");
                CloseBuildingWindow();
            }
            //close creating building window
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                CloseBuildingWindow();
            }
            SetBuildings();
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
