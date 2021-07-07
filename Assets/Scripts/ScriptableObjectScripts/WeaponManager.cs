using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum MaterialType { Wood, LightMetal, HeavyMetal, AlloyMetal }

[CreateAssetMenu(fileName = "WeaponManager", menuName = "WeaponManager")]
public class WeaponManager : ScriptableObject
{
    public Weapon[] weapons;
}
