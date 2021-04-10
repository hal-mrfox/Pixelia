using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldingType { RawResourceGathering, Manufactury, City, Castle}

[CreateAssetMenu(fileName = "HoldingManager", menuName = "HoldingManager")]
public class HoldingManager : ScriptableObject
{
    public List<Holding> holdings;


    [System.Serializable]
    public class Holding
    {
        public HoldingType holdingType;

        public Cost buildCost;
        public Cost upkeepCost;
        public BuildingType[] holdables;

        #region Cost
        [System.Serializable]
        public struct Cost
        {
            public int moneyCost;
            public ResourceCount[] resourceCost;
        }
        #endregion
        #region Resource Struct
        [System.Serializable]
        public struct ResourceCount
        {
            public Resource type;
            public int amount;
        }
        #endregion
    }
}
