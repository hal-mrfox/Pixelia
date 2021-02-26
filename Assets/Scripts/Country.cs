using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class Country : MonoBehaviour
{
    public enum Tier { barony, county, duchy, kingdom, empire }
    public Tier tier;
    [Range(0,1)]public float prestige;
    public Religion religion;
    public Culture culture;
    public Ideology ideology;
    public BeliefsManager.Nationality nationality;
    public Color countryColor;
    public Province capitalProvince;
    public List<Province> ownedProvinces;
    public List<Country> atWar;
    public List<Population> population;
    public List<OldBuilding> buildings;

    public int wood;
    public int stone;

    public void UpgradeTier()
    {
        if (prestige >= 1)
        {
            tier += 1;
            prestige -= 1;
        }
    }

    public void DowngradeTier()
    {

    }
}
