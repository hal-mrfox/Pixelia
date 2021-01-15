using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Window : MonoBehaviour
{
    public Country target;
    public ProvinceScript provinceTarget;
    public TextMeshProUGUI countryReligion;
    public TextMeshProUGUI countryCulture;
    public TextMeshProUGUI countryIdeology;
    public Button exitButton;
    public TextMeshProUGUI countryName;
    public Button warButton;
    public TextMeshProUGUI warButtonText;
    public Button trade;
    public Button offerAlliance;
    bool atWar;

    public void OnEnable()
    {
        OnClicked();
    }

    public void OnClicked()
    {
        countryName.text = target.name;

        ////selecting dominant religion, culture, and ideology
        //int[] religionCounts = new int[System.Enum.GetNames(typeof(Population.Religion)).Length];
        //int[] cultureCounts = new int[System.Enum.GetNames(typeof(Population.Culture)).Length];
        //int[] ideologyCounts = new int[System.Enum.GetNames(typeof(Population.Ideology)).Length];
        //
        //for (int i = 0; i < target.ownedProvinces.Count; i++)
        //{
        //    //religionCounts[(int)target.ownedProvinces[i].]++;
        //    cultureCounts[(int)provinceTarget.pops[i].culture]++;
        //    ideologyCounts[(int)provinceTarget.pops[i].ideology]++;
        //}
        //
        //int dominantReligion = 0;
        //for (int i = 1; i < religionCounts.Length; i++)
        //{
        //    if (religionCounts[i] > religionCounts[dominantReligion])
        //    {
        //        dominantReligion = i;
        //    }
        //}
        //int dominantCulture = 0;
        //for (int i = 1; i < cultureCounts.Length; i++)
        //{
        //    if (cultureCounts[i] > cultureCounts[dominantCulture])
        //    {
        //        dominantCulture = i;
        //    }
        //}
        //int dominantIdeology = 0;
        //for (int i = 1; i < ideologyCounts.Length; i++)
        //{
        //    if (ideologyCounts[i] > ideologyCounts[dominantIdeology])
        //    {
        //        dominantIdeology = i;
        //    }
        //}
        //this.countryReligion.text = ((Population.Religion)dominantReligion).ToString();
        //if (dominantReligion == (int)CountryManager.instance.playerCountry.religion)
        //{
        //    this.countryReligion.color = CountryManager.instance.green;
        //}
        //else
        //{
        //    this.countryReligion.color = CountryManager.instance.red;
        //}
        //
        //this.countryCulture.text = ((Population.Culture)dominantCulture).ToString();
        //if (dominantCulture == (int)CountryManager.instance.playerCountry.culture)
        //{
        //    this.countryCulture.color = CountryManager.instance.green;
        //}
        //else
        //{
        //    this.countryCulture.color = CountryManager.instance.red;
        //}
        //
        //this.countryIdeology.text = ((Population.Ideology)dominantIdeology).ToString();
        //if (dominantIdeology == (int)CountryManager.instance.playerCountry.ideology)
        //{
        //    this.countryIdeology.color = CountryManager.instance.green;
        //}
        //else
        //{
        //    this.countryIdeology.color = CountryManager.instance.red;
        //}

        IfPlayer();
        IfAlreadyWar();

        CountryManager.instance.crown.anchoredPosition = new Vector2(Mathf.Lerp(0, CountryManager.instance.crownLine.rect.width - CountryManager.instance.crown.rect.width, target.prestige), 0);
    }

    //Check to see player
    public void IfPlayer()
    {
        if (target == CountryManager.instance.playerCountry)
        {
            warButton.gameObject.SetActive(false);
            trade.gameObject.SetActive(false);
            offerAlliance.gameObject.SetActive(false);
        }
        else
        {
            warButton.gameObject.SetActive(true);
            trade.gameObject.SetActive(true);
            offerAlliance.gameObject.SetActive(true);
        }
    }

    //Buttons\\
    public void WarButton()
    {
        if (!atWar)
        {
            CountryManager.instance.playerCountry.atWar.Add(target);
            target.atWar.Add(CountryManager.instance.playerCountry);
            IfAlreadyWar();
        }
        else
        {
            CountryManager.instance.playerCountry.atWar.Remove(target);
            target.atWar.Remove(CountryManager.instance.playerCountry);
            IfAlreadyWar();
        }
    }

    //change button text to be accurate to war state
    public void IfAlreadyWar()
    {
        if (CountryManager.instance.playerCountry.atWar.Contains(target))
        {
            atWar = true;
            warButtonText.text = "Offer Peace";
            warButton.GetComponent<Image>().color = CountryManager.instance.blue;
        }
        else
        {
            atWar = false;
            warButtonText.text = "Declare War";
            warButton.GetComponent<Image>().color = CountryManager.instance.red;
        }
    }

    public void ExitButton()
    {
        gameObject.SetActive(false);
    }

}
