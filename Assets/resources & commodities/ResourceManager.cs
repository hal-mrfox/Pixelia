using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Wood, Stone, Coal, Sulfur, LightMetalOre, Gold, Food, Gunpowder, SulfurOre, HeavyMetalOre, LightMetal, HeavyMetal, LuxuryGem, Opium, Sword, Spear, Bow, Rifle, Club, ConsumerGoods, LuxuryGoods }

[CreateAssetMenu(fileName = "ResourceManager", menuName = "ResourceManager")]
public class ResourceManager : ScriptableObject
{
    public List<ResourceDetails> resources;

    [System.Serializable]
    public class ResourceDetails
    {
        public Resource resource;
        public int baseUses;
    }
}
