using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class CountryManager : MonoBehaviour
{
    public static CountryManager instance;

    public List<Country> countries;
    public List<ProvinceScript> provinces;
    public Country playerCountry;
    public Window window;

    public Population popPrefab;
    public Building buildingPrefab;

    public GameObject cursorIcon;

    public List<Building> totalBuildings;
    public List<Population> totalPops;
    public Population selectedPop;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        UpdateColors();
    }


    //UI\\


    public void VisibleMouse()
    {
        if (selectedPop == null)
        {
            Cursor.visible = true;
            cursorIcon.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            cursorIcon.SetActive(true);
        }
    }

    
    public void Update()
    {
        //open country menu
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!window.gameObject.activeSelf)
            {
                window.target = playerCountry;
                window.provinceTarget = playerCountry.capitalProvince;
                window.countryName.text = playerCountry.name;
                window.gameObject.SetActive(true);
                window.IfPlayer();
                window.IfAlreadyWar();
            }
            else
            {
                window.markBuildingSpot = false;
                window.ExitButton();
            }
        }

        if (selectedPop != null)
        {
            cursorIcon.transform.position = Input.mousePosition;
        }
    }


    //Colors\\


    //refreshing owners of pops and buildings
    [Button]
    public void RefreshControllers()
    {
        for (int i = 0; i < provinces.Count; i++)
        {
            for (int j = 0; j < provinces[i].pops.Count; j++)
            {
                provinces[i].pops[j].controller = provinces[i].owner;
            }

            for (int j = 0; j < provinces[i].buildings.Count; j++)
            {
                provinces[i].buildings[j].controller = provinces[i].owner;
            }
        }
    }
    //ALWAYS ONLY SET PROVINCE OWNERSHIP!
    public void UpdateColors()
    {
        for (int i = 0; i < countries.Count; i++)
        {
            for (int j = 0; j < countries[i].ownedProvinces.Count; j++)
            {
                countries[i].ownedProvinces[j].GetComponent<Image>().color = countries[i].countryColor;
            }
        }
    }

    public void ResetColors()
    {
        for (int i = 0; i < countries.Count; i++)
        {
            for (int j = 0; j < countries[i].ownedProvinces.Count; j++)
            {
                countries[i].ownedProvinces[j].GetComponent<Image>().color = Color.white;
            }
        }
    }
}