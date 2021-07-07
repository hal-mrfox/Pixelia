using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum StanceType { Weasel, basic1 }

[CreateAssetMenu(fileName = "TacticsManager", menuName = "TacticsManager")]
public class TacticsManager : ScriptableObject
{
    public MeleeTactic[] meleeTactics;
    public ArmyStance[] armyStances;

    [System.Serializable]
    public class MeleeTactic
    {
        public string tacticName;
        [ShowAssetPreview]
        public Sprite icon;

        public float accuracyBonus;

        public float rangeBonus;

        public int durabilityBonus;

        public int headBonus;
        public int torsoBonus;
        public int legsBonus;

        public int woodBonus;
        public int lightMetalBonus;
        public int heavyMetalBonus;

        public WeaponType[] specificWeapons;
    }

    [System.Serializable]
    public class ArmyStance
    {
        public string stanceName;
        [ShowAssetPreview]
        public Sprite icon;

        public Line[] lines;// line[0] is front line. leave blank lines (with no units) to have gaps in tactic

        [System.Serializable]
        public struct Line
        {
            public Unit[] units;

            [System.Serializable]
            public struct Unit
            {
                [Range(0, 1)] public float low;
                [Range(0, 1)] public float high;

                public UnitType unitType;
            }
        }
    }
}
