using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class Province : MonoBehaviour, IClickable
{
    public Country owner;
    public static Recipes recipes;
    public static BuildingManager buildingManager;
    public static HoldingManager holdingManager;
    #region Beliefs
    public Image highlightedCountry;
    [BoxGroup("Beliefs")]
    public Religion religion;
    [BoxGroup("Beliefs")]
    public Culture culture;
    [BoxGroup("Beliefs")]
    public Ideology ideology;
    [BoxGroup("Beliefs")]
    public BeliefsManager.Nationality nationality;
    #endregion
    #region Statistics
    [BoxGroup("Stats")]
    public List<Population> pops;
    [BoxGroup("Stats")]
    public int holdingCapacity;
    [BoxGroup("Stats")]
    public int buildingCapacity;
    [BoxGroup("Stats")]
    public int supplyLimit;
    [BoxGroup("Stats")]
    public float tax;
    #endregion
    #region Improvements
    [BoxGroup("Improvements")]
    public List<ProvinceHolding> holdings;
    #region Improvements Serializing


    [System.Serializable]
    public class ProvinceHolding
    {
        public HoldingType holdingType;

        public List<ProvinceBuilding> buildings;

        [System.Serializable]
        public class ProvinceBuilding
        {
            public BuildingType buildingType;

            public float efficiency;

            public List<ProvinceResource> resourceOutput;
            public List<ProvinceResource> resourceInput;

            public List<Population> pops;

            public void RefreshBuilding()
            {
                resourceInput.Clear();
                for (int i = 0; i < resourceOutput.Count; i++)
                {
                    int outputInt = ResourceManager.instance.resources.IndexOf(resourceOutput[i].resource);
                    if (recipes.resourceRecipes[outputInt].requiredResources.Length > 0)
                    {
                        for (int j = 0; j < recipes.resourceRecipes[outputInt].requiredResources.Length; j++)
                        {
                            resourceInput.Add(new ProvinceResource(recipes.resourceRecipes[outputInt].requiredResources[j].resource, recipes.resourceRecipes[outputInt].requiredResources[j].amount));
                        }
                    }
                }
            }
        }
    }
    #endregion
    #endregion
    #region Resources
    [BoxGroup("Resources")]
    public List<ProvinceResource> ownedResources;
    [BoxGroup("Resources")]
    public List<ProvinceResource> rawResources;
    #region ProvinceResource
    [System.Serializable]
    public class ProvinceResource
    {
        public Resource resource;
        public int resourceCount;

        public ProvinceResource(Resource resource, int resourceCount)
        {
            this.resource = resource;
            this.resourceCount = resourceCount;
        }
    }
    #endregion
    #endregion
    #region General
    [BoxGroup("General")]
    bool popCanMove;
    [BoxGroup("General")]
    public bool hovering;
    #endregion

    public void Awake()
    {
        recipes = UnityEngine.Resources.Load<Recipes>("Recipes");
        buildingManager = UnityEngine.Resources.Load<BuildingManager>("BuildingManager");
        holdingManager = UnityEngine.Resources.Load<HoldingManager>("HoldingManager");
    }

    public void Start()
    {
        GetComponent<Image>().color = owner.countryColor;
        highlightedCountry = Instantiate(GetComponent<Image>(), transform.position + new Vector3(0f, 2f), Quaternion.identity, transform);
        Destroy(highlightedCountry.GetComponent<Province>());
        RefreshProvinceValues();
        RefreshProvinceColors();
    }

    #region Refreshing
    public void RefreshProvinceValues()
    {
        #region Beliefs Refresh
        //int[] religionCounts = new int[BeliefsManager.instance.religions.Count];
        //int[] cultureCounts = new int[BeliefsManager.instance.cultures.Count];
        //int[] ideologyCounts = new int[BeliefsManager.instance.ideologies.Count];
        //for (int i = 0; i < pops.Count; i++)
        //{
        //    religionCounts[BeliefsManager.instance.religions.IndexOf(pops[i].religion)]++;
        //    cultureCounts[BeliefsManager.instance.cultures.IndexOf(pops[i].culture)]++;
        //    ideologyCounts[BeliefsManager.instance.ideologies.IndexOf(pops[i].ideology)]++;
        //}
        //int dominantReligion = 0;
        //for (int i = 1; i < religionCounts.Length; i++)
        //{
        //    if (religionCounts[i] > religionCounts[dominantReligion])
        //    {
        //        dominantReligion = i;
        //    }
        //}
        //int dominantCulture = 0;
        //for (int i = 1; i < cultureCounts.Length; i++)
        //{
        //    if (cultureCounts[i] > cultureCounts[dominantCulture])
        //    {
        //        dominantCulture = i;
        //    }
        //}
        //int dominantIdeology = 0;
        //for (int i = 1; i < ideologyCounts.Length; i++)
        //{
        //    if (ideologyCounts[i] > ideologyCounts[dominantIdeology])
        //    {
        //        dominantIdeology = i;
        //    }
        //}
        //religion = BeliefsManager.instance.religions[dominantReligion];
        //culture = BeliefsManager.instance.cultures[dominantCulture];
        //ideology = BeliefsManager.instance.ideologies[dominantIdeology];
        #endregion

        #region Pop Counting
        pops.Clear();
        for (int i = 0; i < holdings.Count; i++)
        {
            for (int j = 0; j < holdings[i].buildings.Count; j++)
            {
                for (int k = 0; k < holdings[i].buildings[j].pops.Count; k++)
                {
                    pops.Add(holdings[i].buildings[j].pops[k]);
                }
            }
        }
        #endregion

        #region Production Efficiency Calculation
        for (int i = 0; i < holdings.Count; i++)
        {
            for (int j = 0; j < holdings[i].buildings.Count; j++)
            {
                if (holdings[i].buildings[j].resourceOutput.Count > 0)
                {
                    holdings[i].buildings[j].efficiency = holdings[i].buildings[j].pops.Count;
                }
                else
                {
                    holdings[i].buildings[j].efficiency = 0;
                }
            }
        }
        #endregion

        //Refresh
        for (int i = 0; i < holdings.Count; i++)
        {
            for (int j = 0; j < holdings[i].buildings.Count; j++)
            {
                holdings[i].buildings[j].RefreshBuilding();
            }
        }
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
    #endregion

    #region Removing Pop
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
    #endregion

    #region OnPointerDown
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
    #endregion

    #region Army stuff (if pop can move)
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
    #endregion

    #region Changing Province Ownership
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
    #endregion

    #region IClickables
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
    #endregion
}
