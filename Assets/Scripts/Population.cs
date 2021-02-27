using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Population : MonoBehaviour, IClickable
{
    public enum PopType { Unemployed, Slave, Soldier, Laborer, Farmer, Clerk, Missionary, Knight, Aristocrat };
    public PopType popType;
    //public enum Religion { Shimbleworth, Shmoobli}
    //public Religion religion;
    //public enum Culture { Crumbus, Yaboi}
    //public Culture culture;
    //public enum Ideology { Tribe, Feudal}
    //public Ideology ideology;
    //public enum Nationality { Sooblian, Idiotlian }
    //public Nationality nationality;

    public Religion religion;
    public Culture culture;
    public Ideology ideology;
    public BeliefsManager.Nationality nationality;

    public Country controller;
    //energy is what every pop can do each turn
    public float energy;
    public Province provinceController;
    public OldBuilding containingBuilding;
    public bool selected;
    public GameObject details;
    public TextMeshProUGUI popTypeText;

    public void Start()
    {
        popTypeText.text = popType.ToString();
        gameObject.SetActive(false);
    }

    public void OnPointerDown()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && controller == CountryManager.instance.playerCountry)
        {
            CountryManager.instance.selectedPop = this;
            CountryManager.instance.popInfo.gameObject.SetActive(true);
            CountryManager.instance.popInfo.Refresh();
            CountryManager.instance.VisibleMouse();
            CountryManager.instance.available = false;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && !controller == CountryManager.instance.playerCountry)
        {
            print("This isn't your unit");
        }
    }
    public void Update()
    {
        RefreshColor();

        if (CountryManager.instance.altMode)
        {
            details.SetActive(true);
        }
        else
        {
            details.SetActive(false);
        }
    }
    //refresh after taken over
    public void RefreshColor()
    {
        if (CountryManager.instance.selectedPop == this)
        {
            CountryManager.instance.selectedPop.GetComponent<Image>().color = CountryManager.instance.yellow;
        }
        else
        {
            GetComponent<Image>().color = controller.countryColor;
        }
    }

    public void OnChangePopType()
    {
        name = controller.name + "'s " + popType;
    }

    public Image GetImage()
    {
        return GetComponent<Image>();
    }

    public void OnPointerEnter()
    {

    }

    public void OnPointerExit()
    {

    }

    public bool IsProvince()
    {
        return false;
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
