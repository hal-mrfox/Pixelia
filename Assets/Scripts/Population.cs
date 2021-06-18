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
    #region Name
    public string popName;
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
    public Holding workingHolding;
    public Building job;
    public Building home;
    public float happiness;

    #region Needs
    public List<Need> needs;
    [Range(0, 10)] public int hunger;
    [Range(0, 10)] public int mood;

    [System.Serializable]
    public class Need
    {
        public Resource resource;
        public float progress;

        public Need(Resource resource, float progress)
        {
            this.resource = resource;
            this.progress = progress;
        }
    }
    #endregion

    public void DestroyPop()
    {
        job.workingPops.Remove(this);
        home.housedPops.Remove(this);
        workingHolding.pops.Remove(this);
        provinceController.RefreshProvinceValues();
        provinceController.windowProvince.RefreshWindow();
        Destroy(this);
    }

    public void Start()
    {
        //for (int i = 0; i < PopulationManager.instance.popTierDetails[(int)popTier].needs.Length; i++)
        //{
        //    needs.Add(new Need(PopulationManager.instance.popTierDetails[(int)popTier].needs[i], 0));
        //}
    }

    public void CalculateProgress()
    {
        for (int i = 0; i < needs.Count; i++)
        {
            //quality or smth?
            needs[i].progress -= .25f;
            if (needs[i].progress < 0)
            {
                needs[i].progress = 0;
            }
        }
    }

    public void Calculate()
    {

        for (int i = 0; i < needs.Count; i++)
        {

        }
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

    public bool IsHolding()
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
