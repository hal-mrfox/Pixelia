using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum HoldingType { Settlement, Castle, City }

[CreateAssetMenu(fileName = "HoldingManager", menuName = "HoldingManager")]
public class HoldingManager : ScriptableObject
{
    public Holding[] holdings;


    [System.Serializable]
    public class Holding
    {
        public HoldingType holdingType;

        public Level[] holdingLevels;


        [System.Serializable]
        public class Level
        {
            public string levelName;
            public int buildingSlots;
            public int defenseLevel;
            public int baseStorage;
            public int garrisonCap;

            public BuildingType[] whiteListedBuildings;
            public BuildingType[] blackListedBuildings;
        }
    }
}
