using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class CountryManager : MonoBehaviour
{
    public static CountryManager instance;

    public Country playerCountry;
    public int nextCountry;
    public TextMeshProUGUI playerCountryName;
    public RectTransform backgroundPrestige;

    public List<Country> countries;
    public List<ProvinceScript> provinces;
    public Population selectedPop;
    public PopInfo popInfo;
    public bool available;
    public WindowCountry window;
    public WindowProvince windowProvince;
    public PopInfo windowPop;
    public List<InteractableWindow> openWindows;
    public bool altMode;

    public Population popPrefab;
    public Building buildingPrefab;
    public GameObject popParent;
    public GameObject buildingParent;

    public GameObject cursorIcon;
    public RectTransform crownLine;
    public RectTransform crown;

    public List<Building> totalBuildings;
    public List<Population> totalPops;

    public Color red;
    public Color blue;
    public Color green;
    public Color yellow;
    public Color orange;
    public Color tan;

    public AudioSource openWindowSound;

    public float buildingPrestige;
    public float popPrestige;

    public void Awake()
    {
        instance = this;
    }

    public void CreateTitle()
    {
        playerCountry.UpgradeTier();
    }

    public void OnValidate()
    {
        RefreshTabs();
    }

    public void Start()
    {
        playerCountryName.text = playerCountry.ToString().Replace("(Country)", "");

        nextCountry = countries.IndexOf(playerCountry) + 1;

        for (int i = 0; i < totalBuildings.Count; i++)
        {
            totalBuildings[i].CreatePop(1);
        }

        UpdateColors();

        RefreshTabs();
    }

    public void NextTurn()
    {
        if (nextCountry >= countries.Count)
        {
            nextCountry = 0;
        }
        for (int i = 0; i < playerCountry.ownedProvinces.Count; i++)
        {
            playerCountry.ownedProvinces[i].highlightedCountry.gameObject.SetActive(false);
        }
        playerCountry = countries[nextCountry];
        for (int i = 0; i < playerCountry.ownedProvinces.Count; i++)
        {
            playerCountry.ownedProvinces[i].highlightedCountry.gameObject.SetActive(true);
        }
        playerCountryName.text = playerCountry.ToString().Replace("(Country)", "");
        window.CloseWindow();
        windowProvince.CloseWindow();
        windowPop.CloseWindow();
        selectedPop = null;
        VisibleMouse();
        nextCountry++;
        UpdateColors();
        RefreshTabs();
    }

    public void RefreshPlayerCountry()
    {
        for (int i = 0; i < provinces.Count; i++)
        {
            if (provinces[i].owner == playerCountry)
            {
                provinces[i].highlightedCountry.gameObject.SetActive(true);
                provinces[i].highlightedCountry.color = playerCountry.countryColor;
                Color.RGBToHSV(provinces[i].GetComponent<Image>().color, out float h, out float s, out float v);
                v -= 0.1f;
                print(v);
                playerCountry.GetComponent<Image>().color = Color.HSVToRGB(h, s, v);
            }
            else
            {
                provinces[i].highlightedCountry.gameObject.SetActive(false);
                provinces[i].GetComponent<Image>().color = playerCountry.countryColor;
            }
        }
    }

    //UI\\

    public void VisibleMouse()
    {
        if (selectedPop == null)
        {
            Cursor.visible = true;
            cursorIcon.SetActive(false);
            available = true;
        }
        else
        {
            Cursor.visible = false;
            cursorIcon.SetActive(true);
        }
    }

    public void RefreshTabs()
    {
        backgroundPrestige.sizeDelta = new Vector2(Mathf.Clamp(110 * playerCountry.prestige, 0, 110), backgroundPrestige.sizeDelta.y);
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            altMode = true;
        }
        else
        {
            altMode = false;
        }

        //open country menu
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!window.gameObject.activeSelf)
            {
                window.target = playerCountry;
                window.provinceTarget = playerCountry.capitalProvince;
                window.countryName.text = playerCountry.name;
                window.gameObject.SetActive(true);
                window.IfPlayer();
                window.IfAlreadyWar();
            }
            else
            {
                window.ExitButton();
            }
        }

        if (selectedPop != null)
        {
            cursorIcon.transform.position = Input.mousePosition;
        }
    }
    //refreshing owners of pops and buildings
    public void RefreshControllers()
    {
        for (int i = 0; i < provinces.Count; i++)
        {
            for (int j = 0; j < provinces[i].pops.Count; j++)
            {
                provinces[i].pops[j].controller = provinces[i].owner;
            }

            for (int j = 0; j < provinces[i].buildings.Count; j++)
            {
                provinces[i].buildings[j].controller = provinces[i].owner;
            }
        }
    }
    //ALWAYS ONLY SET PROVINCE OWNERSHIP!
    public void UpdateColors()
    {
        for (int i = 0; i < countries.Count; i++)
        {
            for (int j = 0; j < countries[i].ownedProvinces.Count; j++)
            {
                countries[i].ownedProvinces[j].GetComponent<Image>().color = countries[i].countryColor;
            }
        }
    }

    public void ResetColors()
    {
        for (int i = 0; i < countries.Count; i++)
        {
            for (int j = 0; j < countries[i].ownedProvinces.Count; j++)
            {
                countries[i].ownedProvinces[j].GetComponent<Image>().color = Color.white;
            }
        }
    }
}