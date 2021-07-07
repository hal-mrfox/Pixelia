using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmorClass { Head, Torso, Legs, Horse }
public enum ArmorType {  }//for specific armor types

[CreateAssetMenu(fileName = "ArmorManager", menuName = "ArmorManager")]
public class ArmorManager : ScriptableObject
{
    public Armor[] Armors;

    public Details[] details;

    [System.Serializable]
    public class Armor
    {
        public string armorName;
        public Sprite armorIcon;
        public MaterialType armorType;
        public ArmorClass bodyPart;
        [Range(0, 100)]public float effectiveness;
    }

    [System.Serializable]
    public class Details
    {
        public MaterialType armorType;
        public int durability; //when 0, breaks
    }
}
