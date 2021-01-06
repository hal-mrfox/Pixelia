using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountryValues : MonoBehaviour
{
    public Country[] countries;

    public Color Blank;

    public Province.Countries playerCountry;

    public bool attackMode;
    public bool changeCountryMode;
    public int chooseMode;

    public Army selectedArmy;

    public TextMeshProUGUI topBarText;
    public TextMeshProUGUI provinceHolderText;
    public GameObject provinceValuesBackground;
    public GameObject countryFlagUI;
    public GameObject countryFlagProvince;
    public TextMeshProUGUI playerCountryName;
    public GameObject army;
    public GameObject armyCanvas;
    public GameObject crownPos;

    public GameObject[] crownIcons;
    public GameObject[] playerCrownIcons;

    public RectTransform attackGameObject;
    public RectTransform buildGameObject;
    public RectTransform devText;
    public RectTransform selectionBox;

    public GameObject goldOne;
    public GameObject goldTwo;
    public GameObject goldThree;

    public GameObject manpowerOne;
    public GameObject manpowerTwo;
    public GameObject manpowerThree;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Instantiate(army, Input.mousePosition, Quaternion.identity, armyCanvas.transform.parent);
        }

        //---CROWN UI---\\

        for (int i = 0; i < countries.Length; i++)
        {
            if (countries[i].country == playerCountry)
            {
                playerCrownIcons[0].SetActive(countries[i].countryRank == 0);
                playerCrownIcons[1].SetActive(countries[i].countryRank == 1);
                playerCrownIcons[2].SetActive(countries[i].countryRank == 2);
                playerCrownIcons[3].SetActive(countries[i].countryRank == 3);
                playerCrownIcons[4].SetActive(countries[i].countryRank == 4);
            }
        }
        
        //---FLAG---\\
        countryFlagUI.GetComponent<Image>().color = countries[(int)playerCountry].countryColor;
        playerCountryName.text = playerCountry.ToString();

        //---HoveringUI---\\
        chooseMode = Mathf.Clamp(chooseMode, 0, 2);

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            attackMode = false;
            changeCountryMode = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            attackMode = true;
            changeCountryMode = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            attackMode = true;
            changeCountryMode = true;
        }

        
        else if (chooseMode == 2)
        {
            attackMode = true;
            changeCountryMode = true;
        }

        for (int i = 0; i < countries.Length; i++)
        {
            if (countries[i].provinces.Count <= 1)
            {
                countries[i].countryRank = 0;
            }
            else if (countries[i].provinces.Count <= 2)
            {
                countries[i].countryRank = 1;
            }
            else if (countries[i].provinces.Count <= 4)
            {
                countries[i].countryRank = 2;
            }
            else if (countries[i].provinces.Count <= 6)
            {
                countries[i].countryRank = 3;
            }
            else if (countries[i].provinces.Count <= 8)
            {
                countries[i].countryRank = 4;
            }
        }
    }

    [System.Serializable]
    public class Country
    {
        public Province.Countries country;

        [Range(0, 4)] public int countryRank;

        public Color countryColor;

        public int gold;
        public int troops;

        public List<Province> provinces = new List<Province>();
    }
}