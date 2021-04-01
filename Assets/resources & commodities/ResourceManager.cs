using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Wood, Stone, Coal, Sulfur, LightMetalOre, Gold, Food, Gunpowder, SulfurOre, HeavyMetalOre, LightMetal, HeavyMetal, LuxuryGem, RecreationalDrugs }

public class ResourceManager : MonoBehaviour
{
    public List<Resource> resources;

    public static ResourceManager instance;

    public void Awake()
    {
        instance = this;
    }
}
