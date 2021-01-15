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
    public TextMeshProUGUI religionText;
    public TextMeshProUGUI cultureText;
    public TextMeshProUGUI ideologyText;

    //resources!
    bool isPlayer;
    public List<Button> buildingSlots;
    //sub windows
    //seeing building info window stuff
    public GameObject buildingInfoWindow;
    public List<GameObject> buildingPopSlots;
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
        RefreshProvinceValues();
        CloseBuildingInfoWindow();
    }

    public void RefreshProvinceValues()
    {
        SetBuildings();
        countryName.text = target.name;
        provinceName.text = provinceTarget.name;
        popsCount.text = provinceTarget.pops.Count.ToString();


        //province religion, culture, and ideology text
        religionText.text = provinceTarget.provinceReligion.ToString();
        cultureText.text = provinceTarget.provinceCulture.ToString();
        ideologyText.text = provinceTarget.provinceIdeology.ToString();
        if (provinceTarget.provinceReligion == (ProvinceScript.Religion)CountryManager.instance.playerCountry.religion)
        {
            religionText.color = CountryManager.instance.green;
        }
        else
        {
            religionText.color = CountryManager.instance.red;
        }

        if (provinceTarget.provinceCulture == (ProvinceScript.Culture)CountryManager.instance.playerCountry.culture)
        {
            cultureText.color = CountryManager.instance.green;
        }
        else
        {
            cultureText.color = CountryManager.instance.red;
        }

        if (provinceTarget.provinceIdeology == (ProvinceScript.Ideology)CountryManager.instance.playerCountry.ideology)
        {
            ideologyText.color = CountryManager.instance.green;
        }
        else
        {
            ideologyText.color = CountryManager.instance.red;
        }
    }

    public void RaisePopButton(int popNum)
    {
        selectedBuilding.RaisePop(popNum);
    }

    public void BuildingButton(int buildingNumber)
    {
        if (buildingNumber < provinceTarget.buildings.Count && target == CountryManager.instance.playerCountry)
        {
            if (selectedBuilding != null)
            {
                selectedBuilding.RefreshColor();
            }
            buildingInfoWindow.gameObject.SetActive(buildingNumber < provinceTarget.buildings.Count);
            selectedBuilding = provinceTarget.buildings[buildingNumber];
            buildingName.text = selectedBuilding.name;
            selectedBuilding.GetComponent<Image>().color = CountryManager.instance.yellow;
            SetPops();
        }
        else if (buildingNumber == provinceTarget.buildings.Count && target == CountryManager.instance.playerCountry)
        {
            CreateBuildingButton();
        }
        else if (target != CountryManager.instance.playerCountry)
        {
            print("You cannot edit other countries provinces!");
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
    //confirmation of destroying buildings
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
    public void SetPops()
    {
        for (int i = 0; i < buildingPopSlots.Count; i++)
        {
            if (i < selectedBuilding.containingPops.Count)
            {
                buildingPopSlots[i].gameObject.SetActive(true);
            }
            else
            {
                buildingPopSlots[i].gameObject.SetActive(false);
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
