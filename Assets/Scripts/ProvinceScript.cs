using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ProvinceScript : MonoBehaviour , IClickable
{
    public Country owner;
    public bool hovering;
    public List<Population> pops;
    public List<Building> buildings;
    public GameObject popsParent;
    public GameObject buildingsParent;
    public int buildingCapacity;
    [Range(0, 1)] public float unrest;
    public int supplyLimit;
    public float tax;
    bool popCanMove;
    int popNameNumber;

    public void Start()
    {
        pops.Add(Instantiate(CountryManager.instance.popPrefab));
        pops[pops.Count - 1].transform.parent = popsParent.transform;
        owner.population.Add(pops[pops.Count - 1]);
        //adding to capitals containing pops (CHANGE TO RANDOM BUILDING IN PROVINCE)
        owner.capital.containingPops.Add(pops[pops.Count - 1]);
        pops[pops.Count - 1].controller = owner;
        pops[pops.Count - 1].provinceController = owner.capitalProvince;
        pops[pops.Count - 1].OnChangePopType();

        //randomizing pop beliefs
        pops[pops.Count - 1].religion = (Population.Religion)Random.Range(0, System.Enum.GetNames(typeof(Population.Religion)).Length);
        pops[pops.Count - 1].culture = (Population.Culture)Random.Range(0, System.Enum.GetNames(typeof(Population.Culture)).Length);
        pops[pops.Count - 1].ideology = (Population.Ideology)Random.Range(0, System.Enum.GetNames(typeof(Population.Ideology)).Length);
        pops[pops.Count - 1].nationality = (Population.Nationality)Random.Range(0, System.Enum.GetNames(typeof(Population.Nationality)).Length);
        CountryManager.instance.totalPops.Add(pops[pops.Count - 1]);
        popNameNumber++;
    }

    //migrate add and remove pop to building script
    [Button]
    public void AddPop()
    {
        pops.Add(Instantiate(CountryManager.instance.popPrefab));
        pops[pops.Count - 1].transform.parent = popsParent.transform;
        owner.population.Add(pops[pops.Count - 1]);
        //adding to capitals containing pops (CHANGE TO RANDOM BUILDING IN PROVINCE)
        owner.capital.containingPops.Add(pops[pops.Count - 1]);
        pops[pops.Count - 1].controller = owner;
        pops[pops.Count - 1].provinceController = owner.capitalProvince;
        pops[pops.Count - 1].OnChangePopType();

        //randomizing pop beliefs
        pops[pops.Count - 1].religion = (Population.Religion)Random.Range(0, System.Enum.GetNames(typeof(Population.Religion)).Length);
        pops[pops.Count - 1].culture = (Population.Culture)Random.Range(0, System.Enum.GetNames(typeof(Population.Culture)).Length);
        pops[pops.Count - 1].ideology = (Population.Ideology)Random.Range(0, System.Enum.GetNames(typeof(Population.Ideology)).Length);
        pops[pops.Count - 1].nationality = (Population.Nationality)Random.Range(0, System.Enum.GetNames(typeof(Population.Nationality)).Length);
        CountryManager.instance.totalPops.Add(pops[pops.Count - 1]);
        CountryManager.instance.windowProvince.SetPops();
        popNameNumber++;
    }

    [Button]
    public void RemovePop()
    {
        if (pops.Count > 1)
        {
            Destroy(pops[pops.Count - 1].gameObject);
            owner.population.Remove(pops[pops.Count - 1]);
            CountryManager.instance.totalPops.Remove(pops[pops.Count - 1]);
            pops.Remove(pops[pops.Count - 1]);
            popNameNumber--;
        }
    }
    public void OnPointerDown()
    {
        //right click on province to open up diplomacy with owner
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (CountryManager.instance.selectedPop != null && hovering == true && popCanMove && CountryManager.instance.available == false)
            {
                CountryManager.instance.selectedPop.provinceController.pops.Remove(CountryManager.instance.selectedPop);
                pops.Add(CountryManager.instance.selectedPop);
                CountryManager.instance.selectedPop.transform.position = Input.mousePosition;
                CountryManager.instance.selectedPop.provinceController = this;
                CountryManager.instance.available = true;
                CountryManager.instance.selectedPop = null;
                CountryManager.instance.VisibleMouse();
                CountryManager.instance.windowProvince.Refresh();
            }
            else if (CountryManager.instance.selectedPop != null && popCanMove == false && CountryManager.instance.available == true)
            {
                print("You cannot move here because you have no military access");
            }
            else if (CountryManager.instance.selectedPop == null && CountryManager.instance.available == true)
            {
                CountryManager.instance.window.target = owner;
                CountryManager.instance.window.provinceTarget = this;
                CountryManager.instance.window.gameObject.SetActive(true);
                CountryManager.instance.window.OnClicked();
                CountryManager.instance.openWindowSound.Play();
            }
        }
        //left click on province to open up province viewer
        if (Input.GetKeyDown(KeyCode.Mouse0) && CountryManager.instance.available == true)
        {
            CountryManager.instance.windowProvince.target = owner;
            CountryManager.instance.windowProvince.provinceTarget = this;
            CountryManager.instance.windowProvince.gameObject.SetActive(true);
            CountryManager.instance.windowProvince.OnClicked();
        }
    }

    public void IfPopCanMove()
    {
        if (owner == CountryManager.instance.playerCountry || CountryManager.instance.playerCountry.atWar.Contains(owner))
        {
            popCanMove = true;
            CountryManager.instance.cursorIcon.GetComponent<Image>().color = Color.white;
        }
        else
        {
            popCanMove = false;
            CountryManager.instance.cursorIcon.GetComponent<Image>().color = Color.grey;
        }
    }

    public void OnPointerEnter()
    {
        hovering = true;
        if (CountryManager.instance.selectedPop != null)
        {
            IfPopCanMove();
        }
    }

    public void ChangeProvinceOwnership()
    {
        //changing ownership of pops -- have option to kill all or add to yours?
        for (int i = 0; i < pops.Count; i++)
        {
            pops[i].controller = CountryManager.instance.playerCountry;
            pops[i].RefreshColor();
        }
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].controller = CountryManager.instance.playerCountry;
            buildings[i].RefreshColor();
        }
        //Removing province from old owner
        owner.ownedProvinces.Remove(this);
        //Removing pops from old owner and adding new playercountry before playercountry becomes new owner
        for (int i = 0; i < owner.population.Count; i++)
        {
            CountryManager.instance.playerCountry.population.Add(owner.population[i]);
            owner.population.Remove(owner.population[i]);
        }
        //Ending war for both sides
        owner.atWar.Remove(CountryManager.instance.playerCountry);
        CountryManager.instance.playerCountry.atWar.Remove(owner);
        //Setting new owner to be playerCountry
        owner = CountryManager.instance.playerCountry;
        //Adding Province to new owner
        owner.ownedProvinces.Add(this);
        //calculate prestige gain?
        owner.RefreshProvinceColors();
    }
    
    public void OnPointerExit()
    {
        hovering = false;
    }
    
    public bool IsProvince()
    {
        return true;
    }
    
    public Image GetImage()
    {
        return GetComponent<Image>();
    }
}
