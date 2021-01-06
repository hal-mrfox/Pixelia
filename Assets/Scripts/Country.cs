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
    [ProgressBar("Prestige", 100, EColor.Gray)]
    public int prestige;
    public Color countryColor;
    public ProvinceScript capitalProvince;
    public Building capital;
    public List<ProvinceScript> ownedProvinces;
    public List<Country> atWar;
    public List<Population> population;

    public void RefreshProvinceColors()
    {
        for (int i = 0; i < ownedProvinces.Count; i++)
        {
            ownedProvinces[i].GetComponent<Image>().color = countryColor;
        }
    }
}
