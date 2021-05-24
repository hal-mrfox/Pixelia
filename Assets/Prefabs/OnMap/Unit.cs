using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Unit : ButtonSound
{
    [Header("Data")]
    public Province location;
    public Country owner;

    [Header("UI")]
    public Image[] identifiers;
    public TextMeshProUGUI numPops;

    public void Refresh()
    {
        if (owner == CountryManager.instance.playerCountry)
        {
            for (int i = 0; i < identifiers.Length; i++)
            {
                identifiers[i].color = CountryManager.instance.niceGreen;
            }
        }
        else
        {
            for (int i = 0; i < identifiers.Length; i++)
            {
                identifiers[i].color = CountryManager.instance.niceGray;
            }
        }
    }
}
