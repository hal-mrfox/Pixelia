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
    public int turn;
    public TextMeshProUGUI playerCountryName;
    public RectTransform backgroundPrestige;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;

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
    public OldBuilding buildingPrefab;
    public GameObject popParent;
    public GameObject buildingParent;

    public GameObject cursorIcon;

    public List<OldBuilding> totalBuildings;
    public List<Population> totalPops;

    public Image crownIcon;
    public Sprite[] crownTiers;

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

    public void Start()
    {
        playerCountryName.text = playerCountry.ToString().Replace("(Country)", "");
        woodText.text = playerCountry.wood.ToString();
        stoneText.text = playerCountry.stone.ToString();

        nextCountry = countries.IndexOf(playerCountry) + 1;

        for (int i = 0; i < totalBuildings.Count; i++)
        {
            totalBuildings[i].CreatePop(1);
        }
        SetUI();
    }

    public void NextTurn()
    {
        if (nextCountry >= countries.Count)
        {
            nextCountry = 0;
        }
        playerCountry = countries[nextCountry];
        playerCountryName.text = playerCountry.ToString().Replace("(Country)", "");
        woodText.text = playerCountry.wood.ToString();
        stoneText.text = playerCountry.stone.ToString();
        window.CloseWindow();
        windowProvince.CloseWindow();
        windowPop.CloseWindow();
        selectedPop = null;
        VisibleMouse();
        nextCountry++;
        turn++;
        for (int i = 0; i < provinces.Count; i++)
        {
            provinces[i].RefreshProvinceColors();
        }

        //SetUI();
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

    public void SetUI()
    {
        Sprite crown = crownTiers[(int)playerCountry.tier];
        crownIcon.sprite = crown;
        crownIcon.SetNativeSize();

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

        SetUI();
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