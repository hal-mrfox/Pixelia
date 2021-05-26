using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Province provinceOwner;
    public Holding holding;
    public WindowProvince provinceWindow;
    public class ProvinceBuildingResource
    {
        public Resource resource;
        public int resourceCount;
        public int resourceNeedsCount;

        public ProvinceBuildingResource(Resource resource, int resourceCount, int resourceNeedsCount)
        {
            this.resource = resource;
            this.resourceCount = resourceCount;
            this.resourceNeedsCount = resourceNeedsCount;
        }
    }

    public void Awake()
    {
        provinceWindow = FindObjectOfType<WindowProvince>();
    }

    public BuildingType buildingType;

    public bool on;

    public float efficiency;

    public float popGrowth = 0f;
    public float popGrowthMultiplier = 0f;

    [Space(50)]
    public List<ProvinceBuildingResource> resourceOutput = new List<ProvinceBuildingResource>();
    [Space(50)]
    public List<ProvinceBuildingResource> resourceInput = new List<ProvinceBuildingResource>();
    [Space(50)]
    public List<Population> pops = new List<Population>();
    [Space(50)]
    public List<Population> housedPops = new List<Population>();

    #region Add Pops
    public void AddPop(PopTier popType, Building house, Holding holding)
    {
        if (pops.Count < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].workerCapacity)
        {
            pops.Add(Instantiate(Universal.popPrefab, transform));
            var newPop = pops[pops.Count - 1];

            var countryOwner = provinceOwner.owner;
            newPop.popTier = popType;
            newPop.religion = countryOwner.religion;
            newPop.culture = countryOwner.culture;
            newPop.ideology = countryOwner.ideology;
            newPop.controller = countryOwner;
            newPop.provinceController = provinceOwner;
            newPop.job = this;
            newPop.home = house;
            newPop.name = "Pop " + (provinceOwner.pops.Count + 1).ToString();
        }

        provinceOwner.RefreshProvinceValues();
    }
    public void DestroyPop()
    {
        Destroy(pops[pops.Count - 1].gameObject);
        pops.RemoveAt(pops.Count - 1);
    }
    #endregion

    #region Create Military Unit
    public void CreateUnit()
    {
        //set unit stats based on pops stats
        for (int i = 0; i < pops.Count; i++)
        {
            pops[i].DestroyPop();

            if (provinceOwner.units.Count < 1)
            {
                provinceOwner.units.Add(Instantiate(MilitaryManager.instance.unitPrefab, MilitaryManager.instance.transform));
                var newUnit = provinceOwner.units[provinceOwner.units.Count - 1];
                CountryManager.instance.totalUnits.Add(newUnit);
                newUnit.location = provinceOwner;
                newUnit.owner = provinceOwner.owner;
                newUnit.transform.position = provinceOwner.transform.position;
                newUnit.numPops = 1;
                newUnit.Refresh();
            }
            else
            {
                provinceOwner.units[0].numPops++;
                provinceOwner.units[0].Refresh();
            }
        }
    }
    #endregion

    #region Refresh Building && Next Turn
    public void RefreshBuilding()
    {
        //Fields
        var recipes = Resources.Load<Recipes>("Recipes");
        var holdingManager = Resources.Load<HoldingManager>("HoldingManager");
        var buildingManager = Resources.Load<BuildingManager>("BuildingManager");

        //Efficiency Calculation
        Mathf.FloorToInt(efficiency = pops.Count / 20f * 100f);

        resourceInput.Clear();
        for (int i = 0; i < resourceOutput.Count; i++)
        {

            //Refreshing stored resources
            bool foundResource = false;
            for (int j = 0; j < provinceOwner.storedResources.Count; j++)
            {
                if (resourceOutput[i].resource == provinceOwner.storedResources[j].resource)
                {
                    foundResource = true;
                    break;
                }
            }
            if (!foundResource)
            {
                provinceOwner.storedResources.Add(new Province.ProvinceResource(resourceOutput[i].resource, 0, 0));
            }
            //Resource Output Value Calculation and setting

            int resourceQuality = 1;
            for (int j = 0; j < provinceOwner.rawResources.Count; j++)
            {
                if (provinceOwner.rawResources[j].resource == resourceOutput[i].resource)
                {
                    resourceQuality = provinceOwner.rawResources[j].quality;
                    break;
                }
            }//                                                                        6, 26
            resourceOutput[i].resourceCount = Mathf.CeilToInt((efficiency / Mathf.Lerp(1, 10, resourceOutput[i].resource.acquisitionDifficulty)) * resourceQuality /*/ resourceOutput.Count*/);

            int outputInt = ResourceManager.instance.resources.IndexOf(resourceOutput[i].resource);
            if (recipes.resourceRecipes[outputInt].requiredResources.Length > 0)
            {
                for (int j = 0; j < recipes.resourceRecipes[outputInt].requiredResources.Length; j++)
                {
                    int resourceAmount = 0;
                    for (int k = 0; k < provinceOwner.storedResources.Count; k++)
                    {
                        if (recipes.resourceRecipes[outputInt].requiredResources[j].resource == provinceOwner.storedResources[k].resource)
                        {
                            resourceAmount = provinceOwner.storedResources[k].resourceCount - provinceOwner.storedResources[k].resourceNeedsCount;
                            break;
                        }
                    }
                    resourceInput.Add(new ProvinceBuildingResource(recipes.resourceRecipes[outputInt].requiredResources[j].resource, resourceAmount, recipes.resourceRecipes[outputInt].requiredResources[j].amount * resourceOutput[i].resourceCount));
                }
            }

            for (int j = 0; j < resourceInput.Count; j++)
            {
                if (resourceInput[j].resourceCount < resourceInput[j].resourceNeedsCount || resourceInput[j].resourceCount <= 0)
                {
                    resourceOutput[i].resourceCount = 0;
                }
            }
        }

        for (int i = 0; i < buildingManager.buildings.Count; i++)
        {
            if (buildingType == buildingManager.buildings[i].buildingType)
            {
                for (int j = 0; j < buildingManager.buildings[i].upkeepCost.resourceCost.Length; j++)
                {
                    resourceInput.Add(new ProvinceBuildingResource(buildingManager.buildings[i].upkeepCost.resourceCost[j].resourceType, 0, buildingManager.buildings[i].upkeepCost.resourceCost[j].amount));
                }
                break;
            }
        }

        //if not on set output to 0
        if (!on)
        {
            for (int i = 0; i < resourceOutput.Count; i++)
            {
                resourceOutput[i].resourceCount = 0;
            }
        }

        #region Housing
        //if housing then setting pop growth
        if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isHousing && housedPops.Count >= 2)
        {
            int popAmount = 0;
            var buildingPopType = Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops[0];

            for (int i = 0; i < housedPops.Count; i++)
            {
                if (housedPops[i].popTier == buildingPopType)
                {
                    popAmount++;
                }
            }

            #region Pop Growth Equation
            popGrowthMultiplier = (Universal.basePopGrowth * (Universal.instance.PopulationMultiplierGraph.Evaluate(popAmount / 100f) * 10));
            //popGrowthMultiplier = 25;
            #endregion
        }
        else
        {
            popGrowthMultiplier = 0;
        }

        //over capacity
        //if (housedPops.Count > Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].housingCapacity)
        //{
        //    for (int i = 0; i < provinceOwner.holdings.Count; i++)
        //    {
        //        for (int j = 0; j < provinceOwner.holdings[i].buildings.Count; j++)
        //        {
        //            var building = provinceOwner.holdings[i].buildings[j];
        //            var buildingType = Resources.Load<BuildingManager>("BuildingManager").buildings[(int)building.buildingType];
        //
        //            int difference = buildingType.housingCapacity - building.housedPops.Count;
        //
        //            //make it so it doesnt try to add wrong pops to wrong buildings
        //            if (buildingType.isHousing && building.housedPops.Count <= buildingType.housingCapacity && building != this)
        //            {
        //                for (int k = 0; k < difference; k++)
        //                {
        //                    building.housedPops.Add(this.housedPops[this.housedPops.Count - 1]);
        //                    this.housedPops.RemoveAt(this.housedPops.Count - 1);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Military

        #endregion
    }

    public void NextTurn()
    {
        for (int i = 0; i < resourceOutput.Count; i++)
        {
            bool foundResource = false;
            for (int j = 0; j < provinceOwner.storedResources.Count; j++)
            {
                if (resourceOutput[i].resource == provinceOwner.storedResources[j].resource)
                {
                    provinceOwner.storedResources[j].resourceCount += resourceOutput[i].resourceCount;
                    foundResource = true;
                    break;
                }
            }

            if (!foundResource)
            {
                provinceOwner.storedResources.Add(new Province.ProvinceResource(resourceOutput[i].resource, resourceOutput[i].resourceCount, 0));
            }
        }

        //add pop growth
        popGrowth += popGrowthMultiplier;

        if (popGrowth >= 100f)
        {
            int buildingCount = 0;
            for (int i = 0; i < provinceOwner.holdings.Count; i++)
            {
                for (int j = 0; j < provinceOwner.holdings[i].buildings.Count; j++)
                {
                    buildingCount++;
                }
            }

            for (int i = 0; i < buildingCount; i++)
            {
                //choose random province (but it doesnt work sometimes?????!?!?!?!?!?!?!?!?!?!s)
                int holdingIndex = Random.Range(0, provinceOwner.holdings.Count);
                int buildingIndex = Random.Range(0, provinceOwner.holdings[holdingIndex].buildings.Count);

                var allowedPop = Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops[0];
                var targetallowedPop = Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceOwner.holdings[holdingIndex].buildings[buildingIndex].buildingType].allowedPops[0];

                if (!Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceOwner.holdings[holdingIndex].buildings[buildingIndex].buildingType].isHousing
                    && !Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceOwner.holdings[holdingIndex].buildings[buildingIndex].buildingType].isMilitary
                    && Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceOwner.holdings[holdingIndex].buildings[buildingIndex].buildingType].allowedPops.Contains(allowedPop)
                    && provinceOwner.holdings[holdingIndex].buildings[buildingIndex].pops.Count < Resources.Load<BuildingManager>("BuildingManager").buildings[(int)provinceOwner.holdings[holdingIndex].buildings[buildingIndex].buildingType].workerCapacity
                    && provinceOwner.holdings[holdingIndex].buildings[buildingIndex] != this)
                {
                    var target = provinceOwner.holdings[holdingIndex].buildings[buildingIndex];
                    target.AddPop(allowedPop, this, holding);
                    housedPops.Add(target.pops[target.pops.Count - 1]);
                    popGrowth = 0;
                    break;
                }
                else
                {
                    popGrowth = 100;
                }
            }

        }

        //subtracting input from stored resources
        for (int i = 0; i < resourceInput.Count; i++)
        {
            for (int j = 0; j < provinceOwner.storedResources.Count; j++)
            {
                if (resourceInput[i].resource == provinceOwner.storedResources[j].resource
                    && resourceOutput[0].resourceCount != 0)
                {
                    provinceOwner.storedResources[j].resourceCount -= resourceInput[i].resourceNeedsCount;
                }
            }
        }
    }
    #endregion

    #region Destroy Building
    public void DestroyBuilding()
    {
        if (!Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isHousing)
        {
            for (int i = 0; i < pops.Count; i++)
            {
                pops[i].job = null;
                pops[i].transform.SetParent(provinceOwner.unemployed.transform);
            }
        }
        else if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isHousing)
        {
            for (int i = 0; i < housedPops.Count; i++)
            {
                housedPops[i].home = null;
            }
        }
        else if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isMilitary)
        {
            for (int i = 0; i < pops.Count; i++)
            {
                pops[i].job = null;
                pops[i].transform.SetParent(provinceOwner.unemployed.transform);
            }
        }

        pops.Clear();

        Destroy(gameObject);

        holding.buildings.Remove(this);
    }
    #endregion
}
