using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Province : MonoBehaviour , IClickable
{
    public Country owner;
    public bool hovering;
    public Image highlightedCountry;
    public Religion religion;
    public Culture culture;
    public Ideology ideology;
    public BeliefsManager.Nationality nationality;

    public List<Population> pops;
    [BoxGroup("Improvements")]
    public List<Holding> holdings;
    public List<Population> occupants;
    public GameObject buildingsParent;
    public int buildingCapacity;
    public int supplyLimit;
    public float tax;
    bool popCanMove;

    #region Holdings & Buildings
    [System.Serializable]
    public class Holding
    {
        public enum HoldingType { HoldingType1, HoldingType2 }
        public HoldingType holdingType;

        public List<Building> buildings;

        [System.Serializable]
        public class Building
        {
            public enum BuildingType { BuildingType1, BuildingType2 }
            public BuildingType buildingType;
        }
    }
    #endregion

    public void Start()
    {
        GetComponent<Image>().color = owner.countryColor;
        highlightedCountry = Instantiate(GetComponent<Image>(), transform.position + new Vector3(0f, 2f), Quaternion.identity, transform);
        Destroy(highlightedCountry.GetComponent<Province>());
        RefreshProvinceColors();
    }

    public void Update()
    {
        RefreshProvinceValues();
    }

    public void RefreshProvinceValues()
    {
        int[] religionCounts = new int[BeliefsManager.instance.religions.Count];
        int[] cultureCounts = new int[BeliefsManager.instance.cultures.Count];
        int[] ideologyCounts = new int[BeliefsManager.instance.ideologies.Count];
        for (int i = 0; i < pops.Count; i++)
        {
            religionCounts[BeliefsManager.instance.religions.IndexOf(pops[i].religion)]++;
            cultureCounts[BeliefsManager.instance.cultures.IndexOf(pops[i].culture)]++;
            ideologyCounts[BeliefsManager.instance.ideologies.IndexOf(pops[i].ideology)]++;
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
        religion = BeliefsManager.instance.religions[dominantReligion];
        culture = BeliefsManager.instance.cultures[dominantCulture];
        ideology = BeliefsManager.instance.ideologies[dominantIdeology];
    }

    public void RefreshProvinceColors()
    {
        if (owner == CountryManager.instance.playerCountry)
        {
            highlightedCountry.gameObject.SetActive(true);
            highlightedCountry.color = owner.countryColor;
            Color.RGBToHSV(owner.countryColor, out float h, out float s, out float v);
            v -= 0.3f;
            if (v < 0)
            {
                v = 0;
            }
            GetComponent<Image>().color = Color.HSVToRGB(h, s, v);
        }
        else
        {
            highlightedCountry.gameObject.SetActive(false);
            GetComponent<Image>().color = owner.countryColor;
        }
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
            if (pops[i].controller == owner)
            {
                CountryManager.instance.playerCountry.population.Add(pops[i]);
            }
            pops[i].controller = CountryManager.instance.playerCountry;
            owner.population.Remove(pops[i]);
            pops[i].RefreshColor();
        }
        //Removing province from old owner
        owner.ownedProvinces.Remove(this);
        //Calculating prestige gain
        float prestigeGain = 0f;
        for (int i = 0; i < holdings.Count; i++)
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
        //destroying country if it owns no provinces & Ending war for both sides if country owns no more provinces
        if (owner.ownedProvinces.Count == 0)
        {
            CountryManager.instance.countries.Remove(owner);
            owner.atWar.Remove(CountryManager.instance.playerCountry);
            CountryManager.instance.playerCountry.atWar.Remove(owner);
        }
        //Setting new owner to be playerCountry
        owner = CountryManager.instance.playerCountry;
        //Adding Province to new owner
        owner.ownedProvinces.Add(this);
        RefreshProvinceColors();
        CountryManager.instance.window.CloseWindow();
        //CountryManager.instance.SetUI();
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
