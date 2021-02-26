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
    public Image capitolIcon;
    public GameObject selectPopWindow;
    public List<BuildingInfoPops> buildingPopSlots;

    public void OnEnable()
    {
        CountryManager.instance.openWindows.Add(this);

        //capitolIcon.gameObject.SetActive(windowProvince.selectedHolding == windowProvince.target.capital);
    }

    public void OnDisable()
    {
        CountryManager.instance.openWindows.Remove(this);
    }

    public void CreatePop()
    {
        selectPopWindow.SetActive(true);
    }

    public override void Update()
    {
        //upkeep.text = windowProvince.selectedHolding.upkeep.ToString();
        //for (int i = 0; i < buildingPopSlots.Count; i++)
        //{
        //    if (i < windowProvince.selectedHolding.containingPops.Count)
        //    {
        //        buildingPopSlots[i].gameObject.SetActive(true);
        //        buildingPopSlots[i].popType.text = windowProvince.selectedHolding.containingPops[i].popType.ToString();
        //    }
        //    else
        //    {
        //        buildingPopSlots[i].gameObject.SetActive(false);
        //    }
        //}
    }
}
