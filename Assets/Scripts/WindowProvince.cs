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
    public TextMeshProUGUI countryName;
    public TextMeshProUGUI provinceName;
    public Button exitButton;
    bool isPlayer;
    public List<Button> buildingSlots;
    //sub windows
    public GameObject buildingInfoWindow;
    public GameObject selectBuildingWindow;
    public Button castle;
    public Button farmlands;
    public Button loggingCamp;
    public Button mine;
    public Button village;

    public void Awake()
    {
        gameObject.SetActive(false);
        exitButton.onClick.AddListener(ExitButton);
    }

    //Set every value on enable here!
    public void OnEnable() 
    {
        countryName.text = target.name;
        provinceName.text = provinceTarget.name;
        IfPlayer();
        SetBuildings();
    }

    public void SetBuildings()
    {
        for (int i = 0; i < buildingSlots.Count; i++)
        {
            if (i < provinceTarget.buildings.Count)
            {
                buildingSlots[i].gameObject.SetActive(true);
                buildingSlots[i].onClick.AddListener(BuildingButton);
            }
            else if (i == provinceTarget.buildings.Count)
            {
                buildingSlots[i].gameObject.SetActive(true);
                buildingSlots[i].onClick.AddListener(CreateBuildingButton);
            }
            else
            {
                buildingSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void BuildingButton()
    {
        //open building info
        buildingInfoWindow.gameObject.SetActive(true);
    }

    public void CreateBuildingButton()
    {
        //open the select building window and create a new building
        selectBuildingWindow.gameObject.SetActive(true);
    }


    // SELECTING BUILDING TYPE TO CREATE WINDOW \\



    public void CreatingBuilding()
    {

    }

    public void ExitButton()
    {
        gameObject.SetActive(false);
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
