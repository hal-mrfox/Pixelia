using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Province : MonoBehaviour, IClickable
{
    //public bool occupied;
    public enum Countries { Memeland, Scotland, Ireland, Wales, Serkonos }
    public Countries occupiedCountry;

    public bool selected;

    public bool hovering;

    public CountryValues countryValues;
    public ProvinceManager provinceManager;

    public GameObject backgroundProvince;

    Color altColor;

    public TextMeshProUGUI provinceNameText;
    public TextMeshProUGUI provinceHolderText;

    public void Start()
    {
        //create province nice lookin vibe

        backgroundProvince = new GameObject(gameObject.name + " Background", typeof(RectTransform));
        backgroundProvince.transform.SetParent(transform);
        backgroundProvince.transform.localPosition = Vector3.up * 2;
        RectTransform rectTransform = (RectTransform)backgroundProvince.transform;
        RectTransform thisRectTransform = (RectTransform)transform;
        rectTransform.sizeDelta = thisRectTransform.sizeDelta;
        Image backgroundColor = backgroundProvince.AddComponent<Image>();
        backgroundColor.color = GetComponent<Image>().color;
        backgroundColor.sprite = GetComponent<Image>().sprite;

        provinceNameText = countryValues.topBarText;
        provinceHolderText = countryValues.provinceHolderText;

        for (int i = 0; i < countryValues.countries.Length; i++)
        {
            if (countryValues.countries[i].provinces.Contains(this))
            {
                occupiedCountry = countryValues.countries[i].country;
                break;
            }
        }
    }
    public void OnPointerEnter()
    {
        provinceNameText.text = gameObject.name;

        countryValues.countryFlagProvince.GetComponent<Image>().color = countryValues.countries[(int)occupiedCountry].countryColor;

        countryValues.provinceValuesBackground.SetActive(true);
        countryValues.crownPos.SetActive(true);
        hovering = true;
    }
    public void OnPointerExit()
    {
        countryValues.provinceValuesBackground.SetActive(false);
        countryValues.crownPos.SetActive(false);
        hovering = false;
    }
    public void OnPointerDown()
    {
        if (countryValues.attackMode == true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && countryValues.changeCountryMode)
            {
                countryValues.playerCountry = occupiedCountry;
            }
            //&& countryValues.selectedArmy != null && provinceManager.line.rect.width < countryValues.selectedArmy.armyRange
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                for (int i = 0; i < countryValues.countries.Length; i++)
                {
                    if (countryValues.countries[i].provinces.Contains(this))
                    {
                        countryValues.countries[i].provinces.Remove(this);
                        countryValues.countries[countryValues.playerCountry.GetHashCode()].provinces.Add(this);
                        occupiedCountry = countryValues.playerCountry;
                        break;
                    }
                }
            }
        }

        if (occupiedCountry == countryValues.playerCountry && Input.GetKeyDown(KeyCode.Mouse1) && countryValues.attackMode == false)
        {
            for (int i = 0; i < countryValues.countries[0].provinces.Count; i++)
            {
                selected = false;
            }

            selected = !selected;
        }
    }
    public void CountryColor(Countries country)
    {
        Color color;

        switch (country)
        {
            case Countries.Memeland:
                color = countryValues.countries[0].countryColor;
                break;
            case Countries.Scotland:
                color = countryValues.countries[1].countryColor;
                break;
            case Countries.Ireland:
                color = countryValues.countries[2].countryColor;
                break;
            case Countries.Wales:
                color = countryValues.countries[3].countryColor;
                break;
            case Countries.Serkonos:
                color = countryValues.countries[4].countryColor;
                break;
            default:
                color = countryValues.Blank;
                break;
        }

        //alt color (darker)
        Color.RGBToHSV(color, out float H, out float S, out float V);

        V -= 0.4f;

        altColor = Color.HSVToRGB(H, S, V);

        gameObject.GetComponent<Image>().color = color;

        if (countryValues.playerCountry == occupiedCountry)
        {
            backgroundProvince.GetComponent<Image>().color = color;
            gameObject.GetComponent<Image>().color = altColor;
        }
        else if (countryValues.playerCountry != occupiedCountry)
        {
            gameObject.GetComponent<Image>().color = color;
        }
    }

    public void Update()
    {
        if (hovering == true)
        {
            countryValues.provinceValuesBackground.transform.position = new Vector3(Input.mousePosition.x + 20, Input.mousePosition.y);

            for (int i = 0; i < countryValues.countries.Length; i++)
            {
                //if country rank
                if (countryValues.countries[i].provinces.Contains(this))
                {
                    //provinceHolderText.text = occupiedCountry.ToString();
                    countryValues.crownIcons[0].SetActive(countryValues.countries[i].countryRank == 0);
                    countryValues.crownIcons[1].SetActive(countryValues.countries[i].countryRank == 1);
                    countryValues.crownIcons[2].SetActive(countryValues.countries[i].countryRank == 2);
                    countryValues.crownIcons[3].SetActive(countryValues.countries[i].countryRank == 3);
                    countryValues.crownIcons[4].SetActive(countryValues.countries[i].countryRank == 4);
                }
            }
        }
        //===-Occupation-===\\

        CountryColor(occupiedCountry);

        if (countryValues.playerCountry == occupiedCountry)
        {
            backgroundProvince.SetActive(true);
        }
        else if (countryValues.playerCountry != occupiedCountry)
        {
            backgroundProvince.SetActive(false);
        }
    }

    public Image GetImage()
    {
        return GetComponent<Image>();
    }

    public bool IsProvince()
    {
        return true;
    }
}
