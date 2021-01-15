using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Population : MonoBehaviour, IPointerDownHandler
{
    public enum PopType { Unemployed, Slave, Soldier, Laborer, Farmer, Clerk, Missionary, Knight, Aristocrat};
    public PopType popType;
    public enum Religion { Shimbleworth, Shmoobli}
    public Religion religion;
    public enum Culture { Crumbus, Yaboi}
    public Culture culture;
    public enum Ideology { Tribe, Feudal}
    public Ideology ideology;
    public enum Nationality { Sooblian, Idiotlian }
    public Nationality nationality;
    public Country controller;
    public ProvinceScript provinceController;
    public Building residence;
    public bool selected;

    public void Start()
    {
        RefreshColor();
        transform.position = controller.capital.transform.position;
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && CountryManager.instance.playerCountry.population.Contains(this))
        {
            CountryManager.instance.selectedPop = this;
            CountryManager.instance.VisibleMouse();
            CountryManager.instance.available = false;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && !CountryManager.instance.playerCountry.population.Contains(this))
        {
            print("This isn't your unit");
        }
    }

    //refresh after taken over
    public void RefreshColor()
    {
        GetComponent<Image>().color = controller.countryColor;
    }

    [Button]
    public void OnChangePopType()
    {
        name = controller.name + "'s " + popType;
    }

    ////different options for moving -- not just being selected
    //public void Update()
    //{
    //    if (CountryManager.instance.selectedPop != null)
    //    {
    //        CountryManager.instance.VisibleMouse();
    //    }
    //    else
    //    {
    //        CountryManager.instance.VisibleMouse();
    //    }
    //}
}
