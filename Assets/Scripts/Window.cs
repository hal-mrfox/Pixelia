using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Window : MonoBehaviour
{
    public Button exitButton;
    public Button warButton;
    public Button createBuildingButton;
    public TextMeshProUGUI warButtonText;
    public TextMeshProUGUI countryName;
    public InputField nameBuilding;
    bool atWar;

    //[ReadOnly]
    public GameObject creatingBuildingMarker;
    public bool markBuildingSpot;
    public Country target;
    public ProvinceScript provinceTarget;

    public void Start()
    {
        exitButton.onClick.AddListener(ExitButton);
        warButton.onClick.AddListener(WarButton);
        createBuildingButton.onClick.AddListener(CreateBuilding);
        gameObject.SetActive(false);
        creatingBuildingMarker.SetActive(false);
        nameBuilding.gameObject.SetActive(false);
    }

    //Check to see player
    public void IfPlayer()
    {
        if (target == CountryManager.instance.playerCountry)
        {
            warButton.gameObject.SetActive(false);
            createBuildingButton.gameObject.SetActive(true);
        }
        else
        {
            warButton.gameObject.SetActive(true);
            createBuildingButton.gameObject.SetActive(false);
        }
    }

    //Creating Building stuff*
    public void CreateBuilding()
    {
        //toggle building mode
        markBuildingSpot = true;
        nameBuilding.gameObject.SetActive(true);
    }
    //*
    public void Update()
    {
        //create building
        if (markBuildingSpot == true)
        {
            creatingBuildingMarker.transform.position = Input.mousePosition;
            creatingBuildingMarker.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Mouse0) && provinceTarget.hovering)
            {
                provinceTarget.buildings.Add(Instantiate(CountryManager.instance.buildingPrefab));
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].gameObject.transform.position = Input.mousePosition;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].transform.parent = provinceTarget.buildingsParent.transform;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].provinceController = provinceTarget;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].name = provinceTarget.name + " " + nameBuilding.text;
                provinceTarget.buildings[provinceTarget.buildings.Count - 1].controller = provinceTarget.owner;
                CountryManager.instance.totalBuildings.Add(provinceTarget.buildings[provinceTarget.buildings.Count - 1]);
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                markBuildingSpot = false;
            }
        }
        else
        {
            creatingBuildingMarker.SetActive(false);
        }
    }

    //change button text to be accurate to war state
    public void IfAlreadyWar()
    {
        if (CountryManager.instance.playerCountry.atWar.Contains(target))
        {
            atWar = true;
            warButtonText.text = "Offer Peace";
        }
        else
        {
            atWar = false;
            warButtonText.text = "Declare War";
        }
    }



    //Buttons\\
    public void WarButton()
    {
        if (!atWar)
        {
            CountryManager.instance.playerCountry.atWar.Add(target);
            target.atWar.Add(CountryManager.instance.playerCountry);
            IfAlreadyWar();
        }
        else
        {
            CountryManager.instance.playerCountry.atWar.Remove(target);
            target.atWar.Remove(CountryManager.instance.playerCountry);
            IfAlreadyWar();
        }
    }

    public void ExitButton()
    {
        gameObject.SetActive(false);
    }

}
