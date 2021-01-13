using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class WindowProvince : MonoBehaviour
{
    public Country target;
    public ProvinceScript provinceTarget;
    //UI
    public TextMeshProUGUI popsCount;
    public TextMeshProUGUI countryName;
    public TextMeshProUGUI provinceName;
    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI taxText;
    public TextMeshProUGUI unrestText;
    public TextMeshProUGUI supplyLimitText;
    public TextMeshProUGUI dominantReligion;
    public TextMeshProUGUI dominantCulture;
    public TextMeshProUGUI dominantIdeology;

    //resources!
    bool isPlayer;
    public List<Button> buildingSlots;
    //sub windows
    //seeing building info window stuff
    public GameObject buildingInfoWindow;
    //creating new buildings stuff
    public GameObject createBuildingMarker;
    public GameObject selectBuildingWindow;
    public int selectedBuildingType;
    //destroying buildings stuff
    public Building selectedBuilding;
    public GameObject destroyBuildingConfirmation;
    //local
    bool markBuildingSpot;

    public void Awake()
    {
        createBuildingMarker.SetActive(false);
    }
    public void OnEnable()
    {
        OnClicked();
    }

    //Set every value on enable here!
    public void OnClicked()
    {
        IfPlayer();
        Refresh();
        CloseBuildingInfoWindow();
    }

    public void Refresh()
    {
        SetBuildings();
        countryName.text = target.name;
        provinceName.text = provinceTarget.name;
        popsCount.text = provinceTarget.pops.Count.ToString();


        //selecting dominant religion, culture, and ideology
        int[] religionCounts = new int[System.Enum.GetNames(typeof(Population.Religion)).Length];
        int[] cultureCounts = new int[System.Enum.GetNames(typeof(Population.Culture)).Length];
        int[] ideologyCounts = new int[System.Enum.GetNames(typeof(Population.Ideology)).Length];

        for (int i = 0; i < provinceTarget.pops.Count; i++)
        {
            religionCounts[(int)provinceTarget.pops[i].religion]++;
            cultureCounts[(int)provinceTarget.pops[i].culture]++;
            ideologyCounts[(int)provinceTarget.pops[i].ideology]++;
        }

        int dominantReligion = 0;
        for (int i = 1; i < religionCounts.Length; i++)
        {
            if (religionCounts[i] > religionCounts[dominantReligion])
            {
                dominantReligion = i;
            }
        }
        int dominantCulture = 0;
        for (int i = 1; i < cultureCounts.Length; i++)
        {
            if (cultureCounts[i] > cultureCounts[dominantCulture])
            {
                dominantCulture = i;
            }
        }
        int dominantIdeology = 0;
        for (int i = 1; i < ideologyCounts.Length; i++)
        {
            if (ideologyCounts[i] > ideologyCounts[dominantIdeology])
            {
                dominantIdeology = i;
            }
        }
        this.dominantReligion.text = ((Population.Religion)dominantReligion).ToString();
        this.dominantCulture.text = ((Population.Culture)dominantCulture).ToString();
        this.dominantIdeology.text = ((Population.Ideology)dominantIdeology).ToString();
    }
    public void BuildingButton(int buildingNumber)
    {
        if (buildingNumber < provinceTarget.buildings.Count)
        {
            if (selectedBuilding != null)
            {
                selectedBuilding.RefreshColor();
            }
            buildingInfoWindow.gameObject.SetActive(buildingNumber < provinceTarget.buildings.Count);
            selectedBuilding = provinceTarget.buildings[buildingNumber];
            buildingName.text = selectedBuilding.name;
            selectedBuilding.GetComponent<Image>().color = CountryManager.instance.yellow;
        }
        else if (buildingNumber == provinceTarget.buildings.Count)
        {
            CreateBuildingButton();
        }
    }
    //Creating Building Window
    public void CreateBuildingButton()
    {
        //open the select building window and create a new building
        if (target == CountryManager.instance.playerCountry)
        {
            selectBuildingWindow.gameObject.SetActive(true);
        }
        else
        {
            print("this aint yo country");
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
    public void Update()
    {
        if (markBuildingSpot == true)
        {
            createBuildingMarker.transform.position = Input.mousePosition;

            if (Input.GetKeyDown(KeyCode.Mouse0) && provinceTarget.hovering && provinceTarget.buildings.Count < provinceTarget.buildingCapacity)
            {
                provinceTarget.buildings.Add(Instantiate(CountryManager.instance.buildingPrefab));
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].gameObject.transform.position = Input.mousePosition;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].transform.parent = provinceTarget.buildingsParent.transform;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].provinceController = provinceTarget;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].buildingType = (Building.BuildingType)selectedBuildingType;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].name = provinceTarget.name + "'s " + provinceTarget.buildings[provinceTarget.buildings.Count - 1].buildingType;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].controller = provinceTarget.owner;
                CountryManager.instance.totalBuildings.Add(provinceTarget.buildings[provinceTarget.buildings.Count - 1]);
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
    public void CloseMainWindow()
    {
        gameObject.SetActive(false);
        if (selectedBuilding != null)
        {
            selectedBuilding.RefreshColor();
        }
        //setting selected building color back to its lieges
        if (selectedBuilding != null)
        {
            selectedBuilding.RefreshColor();
        }
    }
    public void CloseBuildingInfoWindow()
    {
        if (selectedBuilding != null)
        {
            selectedBuilding.RefreshColor();
        }
        buildingInfoWindow.SetActive(false);
    }


    //refresh stuff

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
