using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class OldBuilding : MonoBehaviour, IClickable
{
    public enum BuildingType { Castle, Farmlands, Logging_Camp, Mines, Village }
    public BuildingType buildingType;
    public Country controller;
    public Province provinceController;
    public List<Population> containingPops;
    public List<Population> occupyingPops;
    //public Resource
    public int popCapacity;
    [Range(0, 1)] public float popGrowthCount;
    public float popGrowth;
    public float upkeep;
    [ReadOnly]
    public bool hovering;
    public List<Buildings> buildings;
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

    [System.Serializable]
    public struct Buildings
    {
        public OldBuilding building;
    }

    [System.Serializable]
    public struct BuildingSettings
    {
        public bool isBuilding;
        public Sprite buildingSprite;
        public List<Population.PopType> containable;
    }

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
            containingPops[popNum].containingBuilding = null;
            containingPops.Remove(containingPops[popNum]);
        }
    }

    //pops editing
    public void CreatePop(int chosenPopID)
    {
        //adding new pop to this, this.provinceController, this.controller and CountryManager totalPops
        containingPops.Add(Instantiate(CountryManager.instance.popPrefab));
        provinceController.pops.Add(containingPops[containingPops.Count - 1]);
        controller.population.Add(containingPops[containingPops.Count - 1]);
        CountryManager.instance.totalPops.Add(containingPops[containingPops.Count - 1]);
        //setting pop type
        containingPops[containingPops.Count - 1].popType = (Population.PopType)chosenPopID;
        //setting its parent to be the correct one
        containingPops[containingPops.Count - 1].transform.parent = CountryManager.instance.popParent.transform;
        //setting new pops controllers
        containingPops[containingPops.Count - 1].controller = controller;
        containingPops[containingPops.Count - 1].provinceController = provinceController;
        containingPops[containingPops.Count - 1].OnChangePopType();
        //setting pop beliefs/race
        containingPops[containingPops.Count - 1].religion = containingPops[containingPops.Count - 1].controller.religion;
        containingPops[containingPops.Count - 1].culture = containingPops[containingPops.Count - 1].controller.culture;
        containingPops[containingPops.Count - 1].ideology = containingPops[containingPops.Count - 1].controller.ideology;
        containingPops[containingPops.Count - 1].nationality = containingPops[containingPops.Count - 1].controller.nationality;
    }

    //pop entering building
    public void OnPointerDown()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && hovering && CountryManager.instance.selectedPop != null && CountryManager.instance.available == false)
        {
            if (popCanEnter)
            {
                if (containingPops.Count < popCapacity)
                {
                    MoveIntoBuilding();
                    if (controllersAtWar)
                    {
                        //***DO BATTLE HERE***\\
                        
                        ChangeBuildingOwnership();
                    }
                }
                else
                {
                    print("This building is at maximum capacity");
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
        CountryManager.instance.selectedPop.containingBuilding = this;
        containingPops.Add(CountryManager.instance.selectedPop);
        CountryManager.instance.selectedPop.provinceController.pops.Remove(CountryManager.instance.selectedPop);
        CountryManager.instance.selectedPop.provinceController = provinceController;
        provinceController.pops.Add(CountryManager.instance.selectedPop);
        CountryManager.instance.available = true;
        CountryManager.instance.windowProvince.RefreshProvinceValues();
        DeactivateContainingPops();
    }

    public void ChangeBuildingOwnership()
    {
        //changing ownership of this buildings province if its either the capital or all buildings in province are owned by player country
        //looping through all buildings to see if all buildings are owned by player country
        //int buildingsControlled = 0;
            //increment up to see how many are controlled
        //for (int i = 0; i < provinceController.buildings.Count; i++)
        //{
        //    if (provinceController.buildings[i].controller != controller)
        //    {
        //        buildingsControlled++;
        //    }
        //}
        //do after player selects to end the war!!!!
        //if (buildingsControlled == provinceController.buildings.Count - 1)
        {
            provinceController.ChangeProvinceOwnership();
            provinceController.owner = CountryManager.instance.playerCountry;
        }
        //changing ownership of this building
        controller = CountryManager.instance.playerCountry;
        DeactivateContainingPops();
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

    public void DeactivateContainingPops()
    {
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
