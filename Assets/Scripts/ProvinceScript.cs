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
    public Image highlightedCountry;
    public enum Religion { Shimbleworth, Shmoobli }
    public Religion provinceReligion;
    public enum Culture { Crumbus, Yaboi }
    public Culture provinceCulture;
    public enum Ideology { Tribe, Feudal }
    public Ideology provinceIdeology;
    public List<Population> pops;
    public List<Building> buildings;
    public List<Population> occupants;
    public GameObject buildingsParent;
    public int buildingCapacity;
    [Range(0, 1)] public float unrest;
    public int supplyLimit;
    public float tax;
    bool popCanMove;

    public void Start()
    {
        highlightedCountry = Instantiate(GetComponent<Image>(), transform.position + new Vector3(0f, 2f), Quaternion.identity, transform);
        Destroy(highlightedCountry.GetComponent<ProvinceScript>());
        highlightedCountry.color = owner.countryColor;
        if (owner == CountryManager.instance.playerCountry)
        {
            highlightedCountry.gameObject.SetActive(true);
        }
        else
        {
            highlightedCountry.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        RefreshProvinceValues();
    }

    public void RefreshProvinceValues()
    {
        int[] religionCounts = new int[System.Enum.GetNames(typeof(Population.Religion)).Length];
        int[] cultureCounts = new int[System.Enum.GetNames(typeof(Population.Culture)).Length];
        int[] ideologyCounts = new int[System.Enum.GetNames(typeof(Population.Ideology)).Length];
        for (int i = 0; i < pops.Count; i++)
        {
            religionCounts[(int)pops[i].religion]++;
            cultureCounts[(int)pops[i].culture]++;
            ideologyCounts[(int)pops[i].ideology]++;
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
        provinceReligion = (Religion)dominantReligion;
        provinceCulture = (Culture)dominantCulture;
        provinceIdeology = (Ideology)dominantIdeology;
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
                if (owner == CountryManager.instance.playerCountry)
                {
                    pops.Add(CountryManager.instance.selectedPop);
                    CountryManager.instance.selectedPop.provinceController = this;
                }
                else
                {
                    occupants.Add(CountryManager.instance.selectedPop);
                }
                CountryManager.instance.selectedPop.transform.position = Input.mousePosition;
                CountryManager.instance.available = true;
                CountryManager.instance.selectedPop = null;
                CountryManager.instance.VisibleMouse();
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
            CountryManager.instance.windowProvince.buildingInfoWindow.gameObject.SetActive(false);
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
        //Calculating prestige gain
        float prestigeGain = 0f;
        for (int i = 0; i < buildings.Count; i++)
        {
            prestigeGain += CountryManager.instance.buildingPrestige;
        }
        for (int i = 0; i < pops.Count; i++)
        {
            prestigeGain += CountryManager.instance.popPrestige;
        }
        //adding and subtracting prestige values
        CountryManager.instance.playerCountry.prestige += prestigeGain;
        owner.prestige -= prestigeGain;
        if (owner.prestige < 0)
        {
            owner.prestige -= owner.prestige;
        }
        //destroying country if it owns no provinces
        if (owner.ownedProvinces.Count == 0)
        {
            CountryManager.instance.countries.Remove(owner);
        }
        //Setting new owner to be playerCountry
        owner = CountryManager.instance.playerCountry;
        //Adding Province to new owner
        owner.ownedProvinces.Add(this);
        owner.RefreshProvinceColors();
        highlightedCountry.color = owner.countryColor;
        highlightedCountry.gameObject.SetActive(true);
        CountryManager.instance.window.CloseWindow();
        CountryManager.instance.UpdateColors();
        CountryManager.instance.RefreshTabs();
    }

    public void OnPointerEnter()
    {
        hovering = true;
        if (CountryManager.instance.selectedPop != null)
        {
            IfPopCanMove();
        }
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
