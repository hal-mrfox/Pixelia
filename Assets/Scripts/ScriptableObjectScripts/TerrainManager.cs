using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum TerrainType
{

    Plains, Forest, Hills, Mountains, Desert, DesertMountains, Jungle, Savanna, Oasis

}

[CreateAssetMenu(fileName = "TerrainManager", menuName = "TerrainManager")]
public class TerrainManager : ScriptableObject
{
    public TerrainDetails[] terrainDetails;

    [System.Serializable]
    public struct TerrainDetails
    {
        public TerrainType terrainType;

        public Color mapColor;

        public ResourceDetails[] generatableResources;
    }

    [System.Serializable]
    public struct ResourceDetails
    {
        public Resource resource;

        [Range(0, 3)]public int min;
        [Range(0, 3)] public int max;
    }
}
