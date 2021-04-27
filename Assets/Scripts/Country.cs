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
    #region Country Level
    public enum Tier { barony, county, duchy, kingdom, empire }
    [BoxGroup("Country Level")]
    public Tier tier;
    [BoxGroup("Country Level")]
    [Range(0,1)]public float prestige;
    #endregion
    #region Cosmetics
    [BoxGroup("Cosmetics")]
    public Color countryColor;
    #endregion
    #region Beliefs
    [BoxGroup("Beliefs")]
    public Religion religion;
    [BoxGroup("Beliefs")]
    public Culture culture;
    [BoxGroup("Beliefs")]
    public Ideology ideology;
    [BoxGroup("Beliefs")]
    public BeliefsManager.Nationality nationality;
    #endregion
    #region Statistics
    [BoxGroup("Statistics")]
    public Province capitalProvince;
    [BoxGroup("Statistics")]
    public List<Province> ownedProvinces;
    [BoxGroup("Statistics")]
    public List<Population> population;
    [BoxGroup("Statistics")]
    public List<OldBuilding> buildings;
    #endregion
    #region Money & Resources
    [BoxGroup("Money & Resources")]
    public int money;
    [BoxGroup("Money & Resources")]
    public List<CountryResource> resources = new List<CountryResource>();
    #endregion
    #region Diplomacy
    [BoxGroup("Diplomacy")]
    public List<Country> atWar;
    #endregion

    #region Resource Class
    [System.Serializable]
    public class CountryResource
    {
        public Resource resource;
        public int amount;

        public CountryResource(Resource resource, int amount)
        {
            this.resource = resource;
            this.amount = amount;
        }
    }
    #endregion

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

    public void CalculateResources()
    {
        resources.Clear();
        for (int i = 0; i < ownedProvinces.Count; i++)
        {
            for (int j = 0; j < ownedProvinces[i].storedResources.Count; j++)
            {
                resources.Add(new CountryResource(ownedProvinces[i].storedResources[j].resource, ownedProvinces[i].storedResources[j].resourceCount));
            }
        }
    }
}
