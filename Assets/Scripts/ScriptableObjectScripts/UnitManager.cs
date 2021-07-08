using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;

public enum UnitType
{
    MeleeInfantry, Infantry, Cavalry, Artillery, LightArmor, MediumArmor, HeavyArmor, Landship, AntiTank, AntiAir, LightShip, MediumShip, HeavyShip, LightAir, HeavyAir, Bomber
}

public enum ArmyTier
{
    Troop, Battalion, Brigade, Division, Regiment, Legion
}

[CreateAssetMenu(fileName = "UnitManager", menuName = "UnitManager")]
public class UnitManager : ScriptableObject
{
    public Army armyPrefab;
    public Unit unitPrefab;

    public ArmySetting[] armies;

    [System.Serializable]
    public class ArmySetting
    {
        public ArmyTier armyType;

        public int unitCap;
    }

    public void OnEnable()
    {
        units.Clear();
        #region infantry
        for (int i = 0; i < infantryUnits.Length; i++)
        {
            units.Add(infantryUnits[i]);
        }
        #endregion
        #region cavalry
        for (int i = 0; i < cavalryUnits.Length; i++)
        {
            units.Add(cavalryUnits[i]);
        }
        #endregion
    }
    #region Units

    //empty array, fill with all units on start or awake
    public List<UnitSetting> units;

    public Infantry[] infantryUnits;
    public Cavalry[] cavalryUnits;

    #region Unit
    [System.Serializable]
    public class UnitSetting
    {
        public UnitType unitType;

        [ShowAssetPreview]
        public Sprite icon;

        public int baseFlankingRange; //range is horizontal from directly opposite sqaure
        public int baseSpeed; //increased with different unit types/parts
        public int baseRange; //changed by parts *(heavily)
        public int baseValue; //value increased with upgraded units (for AI)
        public int maxMorale; //increased with parts/commander traits

        public ArmorClass[] allowedArmor;
        public WeaponType[] allowedWeapons;

        public virtual void Initialize(Unit unit)
        {
            unit.flankingRange = baseFlankingRange;
            unit.speed = baseSpeed;
            unit.range = baseRange;
            unit.value = baseValue;
            unit.maxMorale = maxMorale;
        }
    }
    #endregion

    #region Infantry
    [System.Serializable]
    public class Infantry : UnitSetting
    {
        public int baseHeadHealth;
        public int baseTorsoHealth;
        public int baseLegsHealth;

        public override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            unit.isInfantry = true;
            unit.isCavalry = false;
            unit.infantry.headHealth = baseHeadHealth;
            unit.infantry.torsoHealth = baseTorsoHealth;
            unit.infantry.legsHealth = baseLegsHealth;
        }

        //virtual copy method
        //override in unit monobehaviour
    }

    [System.Serializable]
    public class Cavalry : Infantry
    {
        public int baseAnimalHealth;

        public override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            unit.isInfantry = false;
            unit.isCavalry = true;
            unit.cavalry.headHealth = baseHeadHealth;
            unit.cavalry.torsoHealth = baseTorsoHealth;
            unit.cavalry.legsHealth = baseLegsHealth;
        }
    }
    #endregion


    //commander medals
    #endregion
}
