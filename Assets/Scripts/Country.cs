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
    public Color countryColor;
    public ProvinceScript capitalProvince;
    public Building capital;
    public List<ProvinceScript> ownedProvinces;
    public List<Country> atWar;
    public List<Population> population;
    public List<Building> buildings;
    //public enum Religion { Shimbleworth, Shmoobli }
    //public Religion religion;
    //public enum Culture { Crumbus, Yaboi }
    //public Culture culture;
    //public enum Ideology { Tribe, Feudal }
    //public Ideology ideology;

    public Religion religion;
    public Culture culture;
    public Ideology ideology;
    public BeliefsManager.Nationality nationality;

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
