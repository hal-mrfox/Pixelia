using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class BuildingInfoWindow : InteractableWindow
{
    public WindowProvince windowProvince;
    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI popCapacity;
    public TextMeshProUGUI upkeep;
    public GameObject selectPopWindow;
    public List<BuildingInfoPops> buildingPopSlots;

    public void OnEnable()
    {
        CountryManager.instance.openWindows.Add(this);
        if (windowProvince.selectedBuilding != null)
        {
            windowProvince.selectedBuilding.RefreshColor();
        }
    }

    public void OnDisable()
    {
        CountryManager.instance.openWindows.Remove(this);
        if (windowProvince.selectedBuilding != null)
        {
            windowProvince.selectedBuilding.RefreshColor();
        }
    }

    //RaisePop from building at popNum
    public void RaisePopButton(int popNum)
    {
        windowProvince.selectedBuilding.RaisePop(popNum);
    }

    public void CreatePop()
    {
        selectPopWindow.SetActive(true);
    }
    public void PopCreation(int popID)
    {
        //if selected buildings pop growth count is 100% do this
        if (windowProvince.selectedBuilding.containingPops.Count < windowProvince.selectedBuilding.popCapacity)
        {
            windowProvince.selectedBuilding.CreatePop(popID);
        }
        else
        {
            print("You have reached this buildings pop capacity!");
        }
        selectPopWindow.gameObject.SetActive(false);
    }

    public override void Update()
    {
        buildingName.text = windowProvince.selectedBuilding.name;
        popCapacity.text = windowProvince.selectedBuilding.popCapacity.ToString();
        upkeep.text = windowProvince.selectedBuilding.upkeep.ToString();
        for (int i = 0; i < buildingPopSlots.Count; i++)
        {
            if (i < windowProvince.selectedBuilding.containingPops.Count)
            {
                buildingPopSlots[i].gameObject.SetActive(true);
                buildingPopSlots[i].popType.text = windowProvince.selectedBuilding.containingPops[i].popType.ToString();
            }
            else
            {
                buildingPopSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
