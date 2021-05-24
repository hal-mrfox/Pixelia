using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{

    Clubman, Swordsman

}

[CreateAssetMenu(fileName = "UnitManager", menuName = "UnitManager")]
public class UnitManager : ScriptableObject
{
    public Unit[] unitTypes;
    
    [System.Serializable]
    public class Unit
    {
        public UnitType unitType;

        public Resource[] neededWeapons;
    }
}
