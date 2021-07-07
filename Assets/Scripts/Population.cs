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
    public Need[] needs;
    [Range(0, 10)] public int hunger;
    [Range(0, 10)] public int mood;

    [System.Serializable]
    public class Need
    {
        public Resource resource;
        public int progress;

        public Need(Resource resource, int progress)
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

    public void NextTurn()
    {
        hunger--;
        mood--;

        for (int i = 0; i < needs.Length; i++)
        {
            if (needs[i].progress == 0 && home)
            {
                for (int j = 0; j < workingHolding.storedResources.Count; j++)
                {
                    if (workingHolding.storedResources[j].resource == needs[i].resource && workingHolding.storedResources[j].amount > 0)
                    {
                        workingHolding.storedResources[j].amount -= 1;
                        needs[i].progress = 10;
                    }
                }
            }
            else
            {
                if (needs[i].resource.Type == ResourceType.Food)
                {
                    if (needs[i].progress > 0)
                    {
                        needs[i].progress -= Resources.Load<ResourceManager>("ResourceManager").resources[(int)needs[i].resource.Type].resource.baseUses;
                        hunger += 2; //need resource pips things
                    }
                    if (hunger > 10)
                    {
                        hunger = 10;
                    }
                    if (hunger < 0)
                    {
                        hunger = 0;
                    }
                }
                else
                {
                    if (needs[i].progress > 0)
                    {
                        needs[i].progress -= Resources.Load<ResourceManager>("ResourceManager").resources[(int)needs[i].resource.Type].resource.baseUses;
                        mood += 2;
                    }
                    if (mood > 10)
                    {
                        mood = 10;
                    }
                    if (mood < 0)
                    {
                        mood = 0;
                    }
                }
            }
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
