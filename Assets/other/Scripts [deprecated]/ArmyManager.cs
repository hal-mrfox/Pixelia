using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager : MonoBehaviour
{
    public ArmyTypes[] armyTypes;
}

[System.Serializable]
public class ArmyTypes
{
    public enum ArmyType { Swordsman, Pikemen, Bowman, Cavalry }
    public ArmyType armyType;
    //public enum Counters { Swordsman, Pikemen, Bowman, Cavalry }
    //public Counters counters;
}