using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{

    Mine, Farm, Logging, OilRig, GoodsFactory, Factory, Smeltery, LuxuryHousing, University, Temple, Barracks, Stables, Ranges, Housing1, Housing2, Housing3

}

[CreateAssetMenu(fileName = "BuildingManager", menuName = "BuildingManager")]
public class BuildingManager : ScriptableObject
{
    public List<Building> buildings;

    [System.Serializable]
    public class Building
    {
        public BuildingType buildingType;

        public int outputCapacity;
        public Resource[] creatableResources;

        [Space(10)]

        [Header("Sounds")]
        public AudioClip constructionSound;

        [Space(10)]

        [Header("Costs")]
        public Cost buildCost;
        public Cost upkeepCost;

        [Space(10)]

        [Header("Pops")]
        public int housingCapacity;
        public int workerCapacity;
        public PopTier[] allowedPops;
        public bool isHousing;
        public bool isMilitary;
        public bool isManufactury;

        [Space(10)]

        [Header("Tiers")]
        public Tier[] tiers;

        #region Cost
        [System.Serializable]
        public struct Cost
        {
            public int moneyCost;
            public ResourceCount[] resourceCost;
        }
        #endregion
        #region Resource Count
        [System.Serializable]
        public struct ResourceCount
        {
            public Resource resourceType;
            public int amount;
        }
        #endregion
        [System.Serializable]
        public class Tier
        {
            public string tierName;
            public int defensePoints;

            public Resource[] producableResources;
        }
    }
}
