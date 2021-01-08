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

    public List<Population> pops;
    public List<Building> buildings;
    public GameObject popsParent;
    public GameObject buildingsParent;

    public bool hovering;
    bool popCanMove;

    int popNameNumber;

    public void Start()
    {
        AddPop();
    }

    [Button]
    public void AddPop()
    {
        pops.Add(Instantiate(CountryManager.instance.popPrefab));
        pops[pops.Count - 1].transform.parent = popsParent.transform;
        owner.population.Add(pops[pops.Count - 1]);
        //adding to capitals containing pops
        owner.capital.containingPops.Add(pops[pops.Count - 1]);
        pops[pops.Count - 1].controller = owner;
        pops[pops.Count - 1].OnChangePopType();
        CountryManager.instance.totalPops.Add(pops[pops.Count - 1]);
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
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CountryManager.instance.openWindowSound.Play();
            if (CountryManager.instance.selectedPop != null && hovering == true && popCanMove)
            {
                CountryManager.instance.selectedPop.transform.position = Input.mousePosition;
                CountryManager.instance.selectedPop = null;
                CountryManager.instance.VisibleMouse();
            }
            else if (CountryManager.instance.selectedPop != null && popCanMove == false)
            {
                print("You cannot move here because you have no military access");
            }
            else if (CountryManager.instance.selectedPop == null)
            {
                CountryManager.instance.window.target = owner;
                CountryManager.instance.window.provinceTarget = this;
                CountryManager.instance.window.countryName.text = owner.name;
                CountryManager.instance.window.gameObject.SetActive(true);
                CountryManager.instance.window.IfPlayer();
                CountryManager.instance.window.IfAlreadyWar();
            }
        }
    }

    public void ifPopCanMove()
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
            ifPopCanMove();
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
