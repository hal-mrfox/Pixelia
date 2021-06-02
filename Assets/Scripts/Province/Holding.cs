using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Holding : MonoBehaviour, IClickable
{
    public Country owner;
    public Province provinceOwner;
    public bool hovering;
    public List<Building> buildings = new List<Building>();
    public AudioClip clickSound;
    public List<Population> pops;
    public List<Population> unemployedPops;
    public List<Population> homelessPops;

    [Header("Terrain & Resources")]
    public TerrainType terrainType;
    public List<ResourceAmount> storedResources = new List<ResourceAmount>();
    public List<ResourceAmount> rawResources = new List<ResourceAmount>();

    [System.Serializable]
    public class ResourceAmount
    {
        public Resource resource;
        public int amount;

        public ResourceAmount(Resource resource, int amount)
        {
            this.resource = resource;
            this.amount = amount;
        }
    }

    public void Start()
    {
        var terrainManager = Resources.Load<TerrainManager>("TerrainManager").terrainDetails[(int)terrainType];
        for (int i = 0; i < terrainManager.generatableResources.Length; i++)
        {
            int random = Random.Range(terrainManager.generatableResources[i].min, terrainManager.generatableResources[i].max + 1);
            rawResources.Add(new ResourceAmount(terrainManager.generatableResources[i].resource, random));
        }
    }

    public void CreatePop(PopTier popType, Building home)
    {
        pops.Add(Instantiate(Universal.popPrefab, transform));
        var newPop = pops[pops.Count - 1];

        home.housedPops.Add(newPop);
        var countryOwner = owner;
        newPop.popTier = popType;
        newPop.religion = countryOwner.religion;
        newPop.culture = countryOwner.culture;
        newPop.ideology = countryOwner.ideology;
        newPop.controller = countryOwner;
        newPop.provinceController = provinceOwner;
        newPop.name = "Pop " + (provinceOwner.pops.Count + 1).ToString();

        RefreshValues();
    }

    public void OnPointerDown()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && owner == CountryManager.instance.playerCountry)
        {
            CountryManager.instance.windowProvince.target = provinceOwner;
            CountryManager.instance.windowProvince.targetCountry = owner;
            CountryManager.instance.windowProvince.gameObject.SetActive(true);
            CountryManager.instance.windowProvince.OnClicked();
        }
    }

    public void OnPointerEnter()
    {
        CountryManager.instance.GetComponent<AudioSource>().PlayOneShot(clickSound);

        MilitaryManager.instance.hoveringHolding = this;

        hovering = true;

        //transform.position = new Vector3(transform.position.x, transform.position.y + 2);

        //transform.SetAsLastSibling();
        GetComponent<Image>().color = CountryManager.instance.orange;
    }

    public void OnPointerExit()
    {
        hovering = false;

        RefreshUI();
    }

    public void NextTurn()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].NextTurn();
        }
    }

    public void RefreshUI()
    {
        if (owner == CountryManager.instance.playerCountry)
        {
            transform.position = new Vector3(provinceOwner.transform.position.x, provinceOwner.transform.position.y + 2);
        }
        else
        {
            transform.position = new Vector3(provinceOwner.transform.position.x, provinceOwner.transform.position.y);
        }

        if (owner)
        {
            GetComponent<Image>().color = owner.countryColor;
        }
        else
        {
            GetComponent<Image>().color = CountryManager.instance.niceGray;
        }
    }

    public void RefreshValues()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].RefreshBuilding();
        }

        unemployedPops.Clear();
        homelessPops.Clear();
        for (int i = 0; i < pops.Count; i++)
        {
            if (!pops[i].job)
            {
                unemployedPops.Add(pops[i]);
            }
            if (!pops[i].home)
            {
                homelessPops.Add(pops[i]);
            }

            pops[i].workingHolding = this;
        }
    }

    public void TransferOwnership(Country newOwner)
    {
        owner.ownedHoldings.Remove(this);
        owner = newOwner;

        for (int i = 0; i < pops.Count; i++)
        {
            pops[i].controller = owner;
            pops[i].home.housedPops.Remove(pops[i]);
            pops[i].home = null;
        }

        owner.ownedHoldings.Add(this);

        owner.Refresh();
        RefreshUI();

        CountryManager.instance.windowProvince.RefreshWindow();
    }

    #region IClickables
    public Image GetImage()
    {
        return GetComponent<Image>();
    }

    public bool IsHolding()
    {
        return true;
    }

    public bool IsProvince()
    {
        return false;
    }
    #endregion
}
