using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Population : MonoBehaviour, IClickable
{
    #region Pop Type
    public PopTier popTier;
    #endregion
    #region Beliefs
    public Religion religion;
    public Culture culture;
    public Ideology ideology;
    public BeliefsManager.Nationality nationality;
    #endregion
    #region Controllers and Details
    public Country controller;
    public Province provinceController;
    public bool selected;
    #endregion
    public Building job;
    public Building home;

    public void Start()
    {
        
    }

    public void OnPointerDown()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0) && controller == CountryManager.instance.playerCountry)
        //{
        //    CountryManager.instance.selectedPop = this;
        //    CountryManager.instance.popInfo.gameObject.SetActive(true);
        //    CountryManager.instance.popInfo.Refresh();
        //    CountryManager.instance.VisibleMouse();
        //    CountryManager.instance.available = false;
        //}
        //else if (Input.GetKeyDown(KeyCode.Mouse0) && !controller == CountryManager.instance.playerCountry)
        //{
        //    print("This isn't your unit");
        //}
    }
    //public void Update()
    //{
    //    RefreshColor();
    //}
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
        name = controller.name + "'s " + popTier;
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
