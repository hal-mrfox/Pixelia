using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class PopInfo : InteractableWindow
{
    public TextMeshProUGUI popName;
    public TextMeshProUGUI popType;
    public TextMeshProUGUI religion;
    public TextMeshProUGUI culture;
    public TextMeshProUGUI ideology;
    public TextMeshProUGUI nationality;
    public TextMeshProUGUI job;
    public TextMeshProUGUI happiness;

    public void OnEnable()
    {
        CountryManager.instance.openWindows.Add(this);
        Refresh();
    }

    public void OnDisable()
    {
        CountryManager.instance.openWindows.Remove(this);
    }

    public void Refresh()
    {
        popName.text = CountryManager.instance.selectedPop.name;
        popType.text = CountryManager.instance.selectedPop.popTier.ToString();
        religion.text = CountryManager.instance.selectedPop.religion.ToString();
        culture.text = CountryManager.instance.selectedPop.culture.ToString();
        ideology.text = CountryManager.instance.selectedPop.ideology.ToString();
        nationality.text = CountryManager.instance.selectedPop.nationality.ToString();

        if (CountryManager.instance.selectedPop.religion == CountryManager.instance.playerCountry.religion)
        {
            religion.color = CountryManager.instance.green;
        }
        else
        {
            religion.color = CountryManager.instance.red;
        }
        if (CountryManager.instance.selectedPop.culture == CountryManager.instance.playerCountry.culture)
        {
            culture.color = CountryManager.instance.green;
        }
        else
        {
            culture.color = CountryManager.instance.red;
        }
        if (CountryManager.instance.selectedPop.ideology == CountryManager.instance.playerCountry.ideology)
        {
            ideology.color = CountryManager.instance.green;
        }
        else
        {
            ideology.color = CountryManager.instance.red;
        }

        if (CountryManager.instance.selectedPop.job != null)
        {
            job.text = CountryManager.instance.selectedPop.job.ToString();
        }
        else
        {
            job.text = "Wandering";
        }
    }
}
