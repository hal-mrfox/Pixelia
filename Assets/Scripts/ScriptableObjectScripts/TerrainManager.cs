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

        public int defenseModifier;
        public Vector2Int baseBattlefieldSize;

        public ResourceDetails[] generatableResources;
    }

    public void OnValidate()
    {
        //make value divisible by 2
        for (int i = 0; i < terrainDetails.Length; i++)
        {
            if (terrainDetails[i].baseBattlefieldSize.y < 0)
            {
                terrainDetails[i].baseBattlefieldSize.y = 0;
                continue;
            }

            if (terrainDetails[i].baseBattlefieldSize.x < 0)
            {
                terrainDetails[i].baseBattlefieldSize.x = 0;
                continue;
            }

            //X
            if (terrainDetails[i].baseBattlefieldSize.x % 2 == 0)
            {


                //Y
                if (terrainDetails[i].baseBattlefieldSize.y % 2 == 0)
                {
                    continue;
                }
                else
                {
                    terrainDetails[i].baseBattlefieldSize.y--;
                }
            }
            else
            {
                terrainDetails[i].baseBattlefieldSize.x--;
            }
        }
    }

    [System.Serializable]
    public struct ResourceDetails
    {
        public Resource resource;

        //use naughtyattribute MinMaxSlider
        [Range(0, 3)] public int min;
        [Range(0, 3)] public int max;
    }
}
