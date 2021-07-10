using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum MaterialType { Wood, LightMetal, HeavyMetal, AlloyMetal }

[CreateAssetMenu(fileName = "WeaponManager", menuName = "WeaponManager")]
public class WeaponManager : ScriptableObject
{
    public Weapon[] weapons;

    public MaterialDetails[] materials;
    
    [System.Serializable]
    public class MaterialDetails
    {
        public MaterialType material;
        public Counter[] counters;
    }

    [System.Serializable]
    public class Counter
    {
        public MaterialType material;
        [Range(-1, 1)] public float percent;
    }
}
