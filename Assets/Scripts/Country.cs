using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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
    public enum Religion { Shimbleworth, Shmoobli }
    public Religion religion;
    public enum Culture { Crumbus, Yaboi }
    public Culture culture;
    public enum Ideology { Tribe, Feudal }
    public Ideology ideology;

    [Button]
    public void UpgradeTier()
    {

    }
    [Button]
    public void DowngradeTier()
    {

    }

    public void RefreshProvinceColors()
    {
        for (int i = 0; i < ownedProvinces.Count; i++)
        {
            ownedProvinces[i].GetComponent<Image>().color = countryColor;
        }
    }
}
