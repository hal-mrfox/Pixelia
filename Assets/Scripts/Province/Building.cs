using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Province provinceOwner;
    public Holding holding;
    public WindowProvince provinceWindow;

    [System.Serializable]
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
    public List<ProvinceBuildingResource> popsNeeds = new List<ProvinceBuildingResource>();
    [Space(50)]
    public List<Population> workingPops = new List<Population>();
    [Space(50)]
    public List<Population> housedPops = new List<Population>();

    #region Destroy pop
    public void DestroyPop()
    {
        Destroy(workingPops[workingPops.Count - 1].gameObject);
        workingPops.RemoveAt(workingPops.Count - 1);
    }
    #endregion

    #region Create Military Unit
    public void CreateUnit()
    {
        //set unit stats based on pops stats
        for (int i = 0; i < workingPops.Count; i++)
        {
            workingPops[i].DestroyPop();

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

    #region Refresh Building
    public void RefreshBuilding()
    {
        #region Production
        //Fields
        var recipes = Resources.Load<Recipes>("Recipes");
        var holdingManager = Resources.Load<HoldingManager>("HoldingManager");
        var buildingManager = Resources.Load<BuildingManager>("BuildingManager");

        //Efficiency Calculation REDO NEEDED
        if (!Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isHousing)
        {

        }
        //Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].workerCapacity)
        Mathf.FloorToInt(efficiency = workingPops.Count / 10f * 100f);

        resourceInput.Clear();
        for (int i = 0; i < resourceOutput.Count; i++)
        {
            //Refreshing stored resources
            bool foundResource = false;
            for (int j = 0; j < holding.storedResources.Count; j++)
            {
                if (resourceOutput[i].resource == holding.storedResources[j].resource)
                {
                    foundResource = true;
                    break;
                }
            }
            if (!foundResource)
            {
                holding.storedResources.Add(new Holding.ResourceAmount(resourceOutput[i].resource, 0));
            }

            //Resource Output Value Calculation and setting
            int resourceQuality = 0;
            for (int j = 0; j < holding.rawResources.Count; j++)
            {
                if (holding.rawResources[j].resource == resourceOutput[i].resource)
                {
                    resourceQuality = holding.rawResources[j].amount;
                    break;
                }
            }//                                                                       6, 26
            resourceOutput[i].resourceCount = Mathf.CeilToInt(efficiency / Mathf.Lerp(1, 10, resourceOutput[i].resource.acquisitionDifficulty) * (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isManufactury ? 1 : resourceQuality) /*/ resourceOutput.Count*/);

            int outputInt = ResourceManager.instance.resources.IndexOf(resourceOutput[i].resource);
            if (recipes.resourceRecipes[outputInt].requiredResources.Length > 0)
            {
                for (int j = 0; j < recipes.resourceRecipes[outputInt].requiredResources.Length; j++)
                {
                    int resourceAmount = 0;
                    for (int k = 0; k < holding.storedResources.Count; k++)
                    {
                        if (recipes.resourceRecipes[outputInt].requiredResources[j].resource == holding.storedResources[k].resource)
                        {
                            resourceAmount = holding.storedResources[k].amount;
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

        //upkeep
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
        #endregion

        #region Housing
        for (int i = 0; i < workingPops.Count; i++)
        {
            workingPops[i].job = this;
        }
        for (int i = 0; i < housedPops.Count; i++)
        {
            housedPops[i].home = this;
        }

        if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].isHousing)
        {
            for (int i = 0; i < PopulationManager.instance.popTierDetails.Length; i++)
            {
                if (PopulationManager.instance.popTierDetails[i].popTier == Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops[0])
                {
                    bool foundResource = false;
                    for (int j = 0; j < PopulationManager.instance.popTierDetails[i].needs.Length; j++)
                    {
                        for (int k = 0; k < popsNeeds.Count; k++)
                        {
                            if (popsNeeds[k].resource == PopulationManager.instance.popTierDetails[i].needs[j])
                            {
                                foundResource = true;
                                popsNeeds[k].resourceNeedsCount = housedPops.Count;
                            }
                        }
                        if (!foundResource)
                        {
                            popsNeeds.Add(new ProvinceBuildingResource(PopulationManager.instance.popTierDetails[i].needs[j], 0, housedPops.Count));
                        }
                    }
                }
            }
        }

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
            //popGrowthMultiplier = 50;
            #endregion
        }
        else
        {
            popGrowthMultiplier = 0;
        }
        #endregion

        #region Military

        #endregion
    }

    public void NextTurn()
    {
        for (int i = 0; i < resourceOutput.Count; i++)
        {
            bool foundResource = false;
            for (int j = 0; j < holding.storedResources.Count; j++)
            {
                if (resourceOutput[i].resource == holding.storedResources[j].resource)
                {
                    holding.storedResources[j].amount += resourceOutput[i].resourceCount;
                    foundResource = true;
                    break;
                }
            }

            if (!foundResource)
            {
                holding.storedResources.Add(new Holding.ResourceAmount(resourceOutput[i].resource, resourceOutput[i].resourceCount));
            }
        }

        //add pop growth
        popGrowth += popGrowthMultiplier;

        if (popGrowth >= 100f)
        {
            holding.CreatePop(Resources.Load<BuildingManager>("BuildingManager").buildings[(int)buildingType].allowedPops[0], this);

            popGrowth = 0;
        }

        //subtracting input from stored resources
        for (int i = 0; i < resourceInput.Count; i++)
        {
            for (int j = 0; j < holding.storedResources.Count; j++)
            {
                if (resourceInput[i].resource == holding.storedResources[j].resource
                    && resourceOutput[0].resourceCount != 0)
                {
                    holding.storedResources[j].amount -= resourceInput[i].resourceNeedsCount;
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
            for (int i = 0; i < workingPops.Count; i++)
            {
                workingPops[i].transform.SetParent(workingPops[i].workingHolding.transform);
                workingPops[i].job = null;
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
            for (int i = 0; i < workingPops.Count; i++)
            {
                workingPops[i].transform.SetParent(workingPops[i].workingHolding.transform);
                workingPops[i].job = null;
            }
        }


        workingPops.Clear();
        housedPops.Clear();
        holding.owner.Refresh();
        holding.buildings.Remove(this);

        Destroy(gameObject);
    }
    #endregion
}
