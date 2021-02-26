using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
    public static BuildingsManager instance;

    public List<HoldingValues> holdingTypes;

    [System.Serializable]
    public struct HoldingValues
    {
        public List<ResourceCount> neededResources;
        public List<ResourceCount> upkeepResources;

        [System.Serializable]
        public struct ResourceCount
        {
            public Resource resource;
            public int resourceCount;
        }
    }

    public void Awake()
    {
        instance = this;
    }
}
