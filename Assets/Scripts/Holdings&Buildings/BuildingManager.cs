using System.Collections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{

    Mine, Farm, Logging, Plantation, OilRig,

    GoodsFactory, AssemblyFactory, Smeltery,

    Housing, University

}

[CreateAssetMenu(fileName = "BuildingManager", menuName = "BuildingManager")]
public class BuildingManager : ScriptableObject
{
    public List<Building> buildings;

    [System.Serializable]
    public class Building
    {
        public BuildingType buildingType;

        public HoldingType holder;

        public Resource[] creatableResources;

        [Space(10)]

        [Header("Costs")]
        public Cost buildCost;
        public Cost upkeepCost;

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
            public Resource type;
            public int amount;
        }
        #endregion
    }
}
