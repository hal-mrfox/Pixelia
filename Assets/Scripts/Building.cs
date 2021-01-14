﻿using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Building : MonoBehaviour, IClickable
{
    public enum BuildingType { Castle, Farmlands, Logging_Camp, Mines, Village }
    public BuildingType buildingType;
    public Country controller;
    public ProvinceScript provinceController;
    public List<Population> containingPops;
    public int popCapacity;
    [Range(0, 1)] public float popGrowthCount;
    public float popGrowth;
    [ReadOnly]
    public bool hovering;
    public Sprite castle;
    public Sprite farmlands;
    public List<Population.PopType> farmlandsContainable;
    public Sprite loggingCamp;
    public List<Population.PopType> loggingContainable;
    public Sprite mines;
    public List<Population.PopType> minesContainable;
    public Sprite village;
    public List<Population.PopType> villageContainable;
    bool popCanEnter;
    bool controllersAtWar;
    bool allControlled;

    public void Start()
    {
        RefreshColor();

        if (buildingType == BuildingType.Castle)
        {
            GetComponent<Image>().sprite = castle;
        }
        else if (buildingType == BuildingType.Farmlands)
        {
            GetComponent<Image>().sprite = farmlands;
        }
        else if (buildingType == BuildingType.Logging_Camp)
        {
            GetComponent<Image>().sprite = loggingCamp;
        }
        else if (buildingType == BuildingType.Mines)
        {
            GetComponent<Image>().sprite = mines;
        }
        else if (buildingType == BuildingType.Village)
        {
            GetComponent<Image>().sprite = village;
        }

        GetComponent<Image>().SetNativeSize();
    }
    public void RaisePop(int popNum)
    {
        //place pops in correct place! (capitals coords)
        if (containingPops.Count == 0)
        {
            print("You have no pops in this building");
        }
        else
        {
            //setting 1 pop in building to free
            containingPops[popNum].gameObject.SetActive(true);
            containingPops[popNum].transform.position = transform.position;
            containingPops.Remove(containingPops[popNum]);
        }
        CountryManager.instance.windowProvince.SetPops();
    }

    public void DestroyBuilding()
    {
        if (containingPops.Count == 0 && controller.capital != this)
        {
            CountryManager.instance.totalBuildings.Remove(this);
            provinceController.buildings.Remove(this);
            Destroy(gameObject);
        }
        else if (controller.capital == this)
        {
            print("You cannot destroy your capital");
        }
        else if (containingPops.Count > 0)
        {
            print("You still have pops in this building!");
        }
    }

    //pop entering building
    public void OnPointerDown()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && hovering && CountryManager.instance.selectedPop != null && CountryManager.instance.available == false)
        {
            if (popCanEnter)
            {
                if (!controllersAtWar)
                {
                    if (buildingType == BuildingType.Castle)
                    {
                        MoveIntoBuilding();
                    }
                    else if (buildingType == BuildingType.Farmlands && farmlandsContainable.Contains(CountryManager.instance.selectedPop.popType))
                    {
                        MoveIntoBuilding();
                    }
                }
                else
                {
                    //***DO BATTLE HERE***\\
                    ChangeBuildingOwnership();
                    MoveIntoBuilding();
                }
                CountryManager.instance.selectedPop = null;
                CountryManager.instance.VisibleMouse();
            }
            else
            {
                print("You cannot enter here because you have no military access");
            }
        }
    }

    public void MoveIntoBuilding()
    {
        containingPops.Add(CountryManager.instance.selectedPop);
        CountryManager.instance.selectedPop.provinceController.pops.Remove(CountryManager.instance.selectedPop);
        provinceController.pops.Add(CountryManager.instance.selectedPop);
        CountryManager.instance.available = true;
        CountryManager.instance.windowProvince.SetPops();
        CountryManager.instance.windowProvince.Refresh();
        RefreshPopState();
    }

    public void ChangeBuildingOwnership()
    {
        //changing ownership of pops -- have option to kill all or add to yours?
        for (int i = 0; i < containingPops.Count; i++)
        {
            containingPops[i].controller = CountryManager.instance.playerCountry;
        }
        containingPops.Add(CountryManager.instance.selectedPop);


        //changing ownership of this buildings province if its either the capital or all buildings in province are owned by player country
        //looping through all buildings to see if all buildings are owned by player country
        for (int i = 0; i < provinceController.buildings.Count; i++)
        {
            if (provinceController.buildings[i].provinceController != provinceController.owner)
            {
                allControlled = true;
            }
            else
            {
                allControlled = false;
            }
        }
        //do after player selects to end the war!!!!
        if (this == controller.capital || allControlled)
        {
            provinceController.ChangeProvinceOwnership();
            provinceController.owner = CountryManager.instance.playerCountry;
        }
        //changing ownership of this building
        controller = CountryManager.instance.playerCountry;
        RefreshPopState();
        RefreshColor();
    }

    public void OnPointerEnter()
    {
        hovering = true;

        if (CountryManager.instance.playerCountry.atWar.Contains(controller))
        {
            controllersAtWar = true;
        }
        else
        {
            controllersAtWar = false;
        }

        if (CountryManager.instance.selectedPop != null)
        {
            IfPopCanEnter();
        }
    }

    public void IfPopCanEnter()
    { 
        if (controller == CountryManager.instance.playerCountry)
        {
            popCanEnter = true;
            CountryManager.instance.cursorIcon.GetComponent<Image>().color = Color.green;
        }
        else if (controllersAtWar)
        {
            popCanEnter = true;
            CountryManager.instance.cursorIcon.GetComponent<Image>().color = Color.red;
        }
        else if (!controllersAtWar)
        {
            popCanEnter = false;
            CountryManager.instance.cursorIcon.GetComponent<Image>().color = Color.grey;
        }
    }
    public void OnPointerExit()
    {
        hovering = false;
    }

    //refresh after taken over
    public void RefreshColor()
    {
        GetComponent<Image>().color = controller.countryColor;
    }

    public void RefreshPopState()
    {
        //sets selectedPop in countrymanager to null so theres nothing selected!
        for (int i = 0; i < containingPops.Count; i++)
        {
            containingPops[i].gameObject.SetActive(false);
        }
    }

    public Image GetImage()
    {
        return GetComponent<Image>();
    }

    public bool IsProvince()
    {
        return false;
    }
}