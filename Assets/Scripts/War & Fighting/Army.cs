using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour
{
    public string armyName;
    public Country owner;
    public List<Unit> units;
    public List<UnitType> unitTypes;
    public ArmyTier armyTier;
    public StanceType stance;
    [Range(0, 100)] public float organization;
    [Range(0, 100)] public float morale;
    [Range(0, 100)] public float food;
    //commander
}
