using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Holding : MonoBehaviour, IClickable
{
    public Country owner;
    public Province provinceOwner;

    [Header("Holding Type & Level")]
    public HoldingType holdingType;
    public int holdingLevel;

    public bool hovering;
    public List<Building> buildings = new List<Building>();
    public AudioClip clickSound;
    public Vector2 position;

    [Header("Pops")]
    public List<Population> pops;
    public List<Population> unemployedPops;
    public List<Population> homelessPops;

    [Header("Terrain & Resources")]
    public TerrainType terrainType;
    public List<ResourceAmount> storedResources = new List<ResourceAmount>();
    public List<ResourceAmount> rawResources = new List<ResourceAmount>();

    [Header("Trade Routes")]
    public List<TradeRoute> tradeRoutes;
    public int routeCapacity;

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

    public void Awake()
    {
        position = transform.position;
    }

    public void Start()
    {
        var terrainManager = Resources.Load<TerrainManager>("TerrainManager").terrainDetails[(int)terrainType];
        for (int i = 0; i < terrainManager.generatableResources.Length; i++)
        {
            int random = Random.Range(terrainManager.generatableResources[i].min, terrainManager.generatableResources[i].max + 1);
            rawResources.Add(new ResourceAmount(terrainManager.generatableResources[i].resource, random));
        }

        //Add all building children to building list
        for (int i = 0; i < transform.childCount; i++)
        {
            buildings.Add(transform.GetChild(i).GetComponent<Building>());
            transform.GetChild(i).GetComponent<Building>().provinceOwner = provinceOwner;
            transform.GetChild(i).GetComponent<Building>().holding = this;
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

        provinceOwner.RefreshProvinceValues();
        provinceOwner.windowProvince.RefreshWindow();
    }

    public void OnPointerDown()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && owner == CountryManager.instance.playerCountry)
        {
            CountryManager.instance.windowProvince.target = provinceOwner;
            CountryManager.instance.windowProvince.targetCountry = owner;
            CountryManager.instance.windowProvince.gameObject.SetActive(true);
            CountryManager.instance.windowProvince.holdings[provinceOwner.holdings.IndexOf(this)].transform.SetAsLastSibling();
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

        RefreshValues();
    }

    public void RefreshUI()
    {
        if (Resources.Load<MapModeManager>("MapModeManager").mapMode == MapModes.Nations)
        {
            if (owner == CountryManager.instance.playerCountry)
            {
                transform.position = new Vector3(position.x, position.y + 2);
                GetComponent<Image>().color = owner.countryColor;
            }
            else
            {
                transform.position = new Vector3(position.x, position.y);
                GetComponent<Image>().color = owner.countryColor;
            }

            //if (owner)
            //{
            //    GetComponent<Image>().color = owner.countryColor;
            //}
            //else
            //{
            //    GetComponent<Image>().color = CountryManager.instance.niceGray;
            //}
        }
        else if (Resources.Load<MapModeManager>("MapModeManager").mapMode == MapModes.Terrain)
        {
            GetComponent<Image>().color = Resources.Load<TerrainManager>("TerrainManager").terrainDetails[(int)terrainType].mapColor;
            transform.position = position;
        }

        //for (int i = 0; i < roads.Count; i++)
        //{
        //    Destroy(roads[i].gameObject);
        //}
        //roads.Clear();

        //for (int i = 0; i < connectedHoldings.Count; i++)
        //{
        //    GameObject newLine = new GameObject();
        //    newLine.AddComponent<LineRenderer>();
        //    roads.Add(newLine.GetComponent<LineRenderer>());

        //    newLine.GetComponent<LineRenderer>().SetPosition(0, transform.position);
        //    newLine.GetComponent<LineRenderer>().SetPosition(1, connectedHoldings[i].transform.position);
        //    newLine.GetComponent<LineRenderer>().sharedMaterial = RoadManager.instance.lineMaterial;
        //    newLine.transform.SetParent(RoadManager.instance.roadsHolder.transform);
        //}
    }

    public void RefreshValues()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].RefreshBuilding();
        }

        #region Pops Refresh
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

        pops.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Population>())
            {
                pops.Add(transform.GetChild(i).GetComponent<Population>());
            }
            else if (transform.GetChild(i).GetComponent<Building>())
            {
                for (int j = 0; j < transform.GetChild(i).GetComponent<Building>().pops.Count; j++)
                {
                    pops.Add(transform.GetChild(i).GetComponent<Building>().pops[j]);
                }
            }
        }
        #endregion

        for (int o = 0; o < storedResources.Count; o++)
        {
            bool foundResource = false;
            for (int j = 0; j < buildings.Count; j++)
            {
                for (int k = 0; k < buildings[j].resourceOutput.Count; k++)
                {
                    if (storedResources[o].resource == buildings[j].resourceOutput[k].resource)
                    {
                        foundResource = true;
                        break;
                    }
                }
            }
            if (!foundResource && storedResources[o].amount == 0)
            {
                storedResources.RemoveAt(o);
            }
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
