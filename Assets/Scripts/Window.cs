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
