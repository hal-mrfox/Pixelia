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
    public enum PopType 
    { 

        //I
        Unemployed, Slave, Laborer, Farmer, Soldier,

        //II
        Artist, FactoryWorker, Missionary,

        //III
        Minister, Officer, Researcher,
        
        //IV
        Aristrocrat, Leader

    };

    public PopType popType;
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
    #region Needs
    #endregion
    public OldBuilding containingBuilding;
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
