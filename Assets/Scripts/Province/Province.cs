using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class Province : MonoBehaviour, IClickable
{
    public Country owner;
    public WindowProvince windowProvince;
    public static Recipes recipes;
    public static BuildingManager buildingManager;
    public static HoldingManager holdingManager;
    public int size;
    public GameObject unemployed;
    public GameObject homeless;
    public Image highlightedCountry;
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
    [BoxGroup("Stats")]
    public List<Population> pops;
    [BoxGroup("Stats")]
    public List<Population> unemployedPops;
    [BoxGroup("Stats")]
    public List<Population> homelessPops;
    [BoxGroup("Stats")]
    public List<Unit> units;
    [BoxGroup("Stats")]
    public int holdingCapacity;
    [BoxGroup("Stats")]
    public int buildingCapacity;
    [BoxGroup("Stats")]
    public int supplyLimit;
    [BoxGroup("Stats")]
    public float tax;
    #endregion
    #region Resources
    [BoxGroup("Resources")]
    public List<ProvinceResource> storedResources;
    [BoxGroup("Resources")]
    public List<RawResource> rawResources;
    #region ProvinceResource
    [System.Serializable]
    public class ProvinceResource
    {
        public Resource resource;
        public int resourceCount;
        public int resourceNeedsCount;

        public ProvinceResource(Resource resource, int resourceCount, int resourceNeedsCount)
        {
            this.resource = resource;
            this.resourceCount = resourceCount;
            this.resourceNeedsCount = resourceNeedsCount;
        }
    }

    [System.Serializable]
    public class RawResource
    {
        public Resource resource;
        public int quality;
    }
    #endregion
    #endregion
    #region Improvements
    [BoxGroup("Improvements")]
    public List<Holding> holdings;
    #endregion
    #region General
    [BoxGroup("General")]
    bool popCanMove;
    [BoxGroup("General")]
    public bool hovering;
    #endregion

    #region awake & start
    public void Awake()
    {
        recipes = Resources.Load<Recipes>("Recipes");
        buildingManager = Resources.Load<BuildingManager>("BuildingManager");
        holdingManager = Resources.Load<HoldingManager>("HoldingManager");
    }

    public void FirstStart()
    {
        //highlightedCountry = Instantiate(GetComponent<Image>(), transform.position + new Vector3(0f, 2f), Quaternion.identity, transform);
        //Destroy(highlightedCountry.GetComponent<Province>());
        //for (int i = 0; i < highlightedCountry.transform.childCount; i++)
        //{
        //    Destroy(highlightedCountry.transform.GetChild(i).gameObject);
        //}

        for (int h = 0; h < holdings.Count; h++)
        {
            holdings[h].provinceOwner = this;
            //holdings[h].transform.position = transform.position;
            holdings[h].RefreshUI();
        }

        RefreshProvinceValues();
        RefreshProvinceColors();
    }
    #endregion

    #region Next Turn
    public void NextTurn()
    {
        //for (int h = 0; h < holdings.Count; h++)
        //{
        //    for (int b = 0; b < holdings[h].buildings.Count; b++)
        //    {
        //       holdings[h].buildings[b].NextTurn();
        //    }
        //}

        RefreshProvinceColors();
    }
    #endregion

    #region Generate Voronoi Cells
    public void GenerateCells()
    {
        #region old
        //int x = (int)((RectTransform)transform).rect.width;
        //int y = (int)((RectTransform)transform).rect.height;

        ////Find large side
        ////int large = x > y ? x : y;

        ////Getting two values based on the size of this province
        //int pointOneX = (x / 2) - (x / 6);
        //int pointTwoX = (x / 2) + (x / 6);
        ////Generating random points based on the last two values
        //int randPointOne = Random.Range(pointOneX, pointTwoX);
        //int randPointTwo = Random.Range(pointOneX, pointTwoX);

        //Vector2 topPoint = new Vector2(x / 2 + randPointOne, y / 2);
        //Vector2 bottomPoint = new Vector2(x / 2 + randPointTwo, y / 2);

        ////gizmoOne = new Vector3( topPoint.x, topPoint.y, 0);
        ////gizmoTwo = new Vector3(bottomPoint.x, bottomPoint.y, 0);

        //gizmoOne = new Vector3(transform.position.x + topPoint.x, transform.position.y + topPoint.y, 0);
        //gizmoTwo = new Vector3(transform.position.x + bottomPoint.x, transform.position.y - bottomPoint.y, 0);

        //Texture2D texture = GetComponent<Image>().sprite.texture;

        //Color32[] pixels = texture.GetPixels32();

        //int minX = randPointOne < randPointTwo ? randPointOne : randPointTwo;

        //for (int i = 0; i < texture.height; i++)
        //{
        //    for (int j = 0; j < texture.width; j++)
        //    {
        //        int index = texture.width * i + j;

        //        if (j < minX)
        //        {
        //            pixels[index] = new Color32(0, 0, 0, 0);
        //        }
        //        else
        //        {
        //            float point = Mathf.Sign((bottomPoint.x - topPoint.x) * (i - topPoint.y) - (bottomPoint.x - topPoint.y) * (j - topPoint.x));

        //            if (point < 0)
        //            {
        //                pixels[index] = new Color32(0, 0, 0, 0);
        //            }

        //            //Vector2 line = new Vector2(posOneX - posTwoX, texture.height);
        //            //Vector2 pixelDirection = new Vector2(j - posTwoX, i);

        //            //float cross = Vector3.Cross(line, pixelDirection).y;

        //            //if (cross < 0)
        //            //{
        //            //    pixels[index] = new Color32(0, 0, 0, 0);
        //            //}
        //        }
        //    }
        //Texture2D newTexture = new Texture2D(texture.width, texture.height)
        //{
        //    filterMode = FilterMode.Point
        //};
        //
        //newTexture.SetPixels32(pixels);
        //newTexture.Apply();
        //
        //CreateHolding(transform.position, newTexture);
        #endregion

        #region shit
        //float x = (int)((RectTransform)transform).rect.width;
        //float y = (int)((RectTransform)transform).rect.height;
        //
        //Vector2 center = transform.position;
        //
        //List<Vector2> points = new List<Vector2>();
        //
        //float radius = x > y ? x / 2 : y / 2;
        //
        //for (int i = 0; i < size; i++)
        //{
        //    points.Add(Random.insideUnitCircle.normalized * radius + new Vector2(GetComponent<Image>().sprite.texture.width, GetComponent<Image>().sprite.texture.height) / 2);
        //}
        //
        //for (int p = 0; p < points.Count; p++)
        //{
        //    Texture2D texture = new Texture2D(GetComponent<Image>().sprite.texture.width, GetComponent<Image>().sprite.texture.height);
        //
        //    Color32[] pixels = new Color32[texture.width * texture.height];
        //
        //    for (int i = 0; i < texture.height; i++)
        //    {
        //        for (int j = 0; j < texture.width; j++)
        //        {
        //            if (ContainsPoint(points, new Vector2(j, i)))
        //            {
        //                pixels[j] = new Color32(255, 255, 255, 255);
        //            }
        //            else
        //            {
        //                pixels[j] = new Color32(0, 0, 0, 0);
        //            }
        //        }
        //    }
        //
        //    Texture2D newTexture = new Texture2D(texture.width, texture.height)
        //    {
        //        filterMode = FilterMode.Point
        //    };
        //
        //    newTexture.SetPixels32(pixels);
        //    newTexture.Apply();
        //
        //    CreateHolding(transform.position, newTexture);
        //}
        //
        //
        //#region poly contains point
        //static bool ContainsPoint(List<Vector2> polyPoints, Vector2 p)
        //{
        //    var j = polyPoints.Count - 1;
        //    var inside = false;
        //    for (int i = 0; i < polyPoints.Count; j = i++)
        //    {
        //        var pi = polyPoints[i];
        //        var pj = polyPoints[j];
        //        if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
        //            (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
        //            inside = !inside;
        //    }
        //    return inside;
        //}
        #endregion
    }
    #endregion

    #region Create Holding
    public void CreateHolding(Vector2 position, Texture2D texture)
    {
        Holding newHolding = new GameObject().AddComponent<Holding>();
        Sprite sprite = Sprite.Create(texture, GetComponent<Image>().sprite.rect, new Vector2(0.5f, 0.5f));
        newHolding.gameObject.AddComponent<Image>().sprite = sprite;
        newHolding.gameObject.GetComponent<Image>().SetNativeSize();
        newHolding.transform.parent = transform;
        newHolding.transform.position = position;
        newHolding.name = "Holding";
        holdings.Add(newHolding);
        RefreshProvinceValues();
    }
    #endregion

    #region Create Building
    public void CreateBuilding(int holding, BuildingType buildingType)
    {
        Building newBuilding = new GameObject().AddComponent<Building>();
        newBuilding.transform.parent = holdings[holding].transform;
        newBuilding.name = buildingType.ToString();
        newBuilding.provinceOwner = this;
        newBuilding.holding = holdings[holding];
        newBuilding.buildingType = buildingType;
        newBuilding.on = true;

        //this, buildingType, true, 0, 0, 0, null, null, null, null
        holdings[holding].buildings.Add(newBuilding);
        windowProvince.RefreshWindow();
    }
    #endregion

    #region Refreshing
    public void RefreshProvinceValues()
    {
        #region Beliefs Refresh
        //int[] religionCounts = new int[BeliefsManager.instance.religions.Count];
        //int[] cultureCounts = new int[BeliefsManager.instance.cultures.Count];
        //int[] ideologyCounts = new int[BeliefsManager.instance.ideologies.Count];
        //for (int i = 0; i < pops.Count; i++)
        //{
        //    religionCounts[BeliefsManager.instance.religions.IndexOf(pops[i].religion)]++;
        //    cultureCounts[BeliefsManager.instance.cultures.IndexOf(pops[i].culture)]++;
        //    ideologyCounts[BeliefsManager.instance.ideologies.IndexOf(pops[i].ideology)]++;
        //}
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
        //religion = BeliefsManager.instance.religions[dominantReligion];
        //culture = BeliefsManager.instance.cultures[dominantCulture];
        //ideology = BeliefsManager.instance.ideologies[dominantIdeology];
        #endregion

        #region Pop Counting

        unemployedPops.Clear();
        homelessPops.Clear();

        //for (int i = 0; i < pops.Count; i++)
        //{
        //    if (!pops[i].job)
        //    {
        //        unemployedPops.Add(pops[i]);
        //    }
        //    if (!pops[i].home)
        //    {
        //        homelessPops.Add(pops[i]);
        //    }
        //}

        pops.Clear();
        for (int i = 0; i < holdings.Count; i++)
        {
            holdings[i].RefreshValues();
            for (int j = 0; j < holdings[i].pops.Count; j++)
            {
                pops.Add(holdings[i].pops[j]);
            }
        }

        for (int i = 0; i < unemployedPops.Count; i++)
        {
            if (!pops.Contains(unemployedPops[i]))
            {
                pops.Add(unemployedPops[i]);
            }
        }

        for (int i = 0; i < homelessPops.Count; i++)
        {
            if (!pops.Contains(homelessPops[i]))
            {
                pops.Add(homelessPops[i]);
            }
        }
        #endregion

        //Refresh Buildings
        for (int i = 0; i < holdings.Count; i++)
        {
            holdings[i].RefreshUI();
            for (int j = 0; j < holdings[i].buildings.Count; j++)
            {
                holdings[i].buildings[j].RefreshBuilding();
            }
        }

        //Refresh Stored Resources (Just removing if not being produced and has 0 resourceCount)
        for (int o = 0; o < storedResources.Count; o++)
        {
            bool foundResource = false;
            for (int i = 0; i < holdings.Count; i++)
            {
                for (int j = 0; j < holdings[i].buildings.Count; j++)
                {
                    for (int k = 0; k < holdings[i].buildings[j].resourceOutput.Count; k++)
                    {
                        if (storedResources[o].resource == holdings[i].buildings[j].resourceOutput[k].resource)
                        {
                            foundResource = true;
                            break;
                        }
                    }
                }
            }
            if (!foundResource && storedResources[o].resourceCount == 0)
            {
                storedResources.RemoveAt(o);
            }
        }
        ////Refresh Buildings Again
        //for (int i = 0; i < holdings.Count; i++)
        //{
        //    for (int j = 0; j < holdings[i].buildings.Count; j++)
        //    {
        //        holdings[i].buildings[j].RefreshBuilding();
        //    }
        //}
    }

    #region Province Color
    public void RefreshProvinceColors()
    {
        if (Resources.Load<MapModeManager>("MapModeManager").mapMode == MapModes.Nations)
        {
            if (owner)
            {
                if (owner == CountryManager.instance.playerCountry)
                {
                    Color.RGBToHSV(owner.countryColor, out float h, out float s, out float v);
                    v -= 0.3f;
                    if (v < 0)
                    {
                        v = 0;
                    }
                    GetComponent<Image>().color = Color.HSVToRGB(h, s, v);
                    //GetComponent<Image>().color = owner.countryColor;
                }
                else
                {
                    GetComponent<Image>().color = owner.countryColor;
                }
            }
        }
        else if (Resources.Load<MapModeManager>("MapModeManager").mapMode == MapModes.Terrain)
        {
            
        }

        for (int i = 0; i < holdings.Count; i++)
        {
            holdings[i].RefreshUI();
        }
    }
    #endregion

    #endregion

    #region OnPointerDown
    public void OnPointerDown()
    {
        //right click on province to open up diplomacy with owner
        //if (Input.GetKeyDown(KeyCode.Mouse1))
        //{
        //    if (CountryManager.instance.selectedPop != null && hovering == true && popCanMove && CountryManager.instance.available == false)
        //    {
        //        CountryManager.instance.selectedPop.provinceController.pops.Remove(CountryManager.instance.selectedPop);
        //        if (owner == CountryManager.instance.playerCountry)
        //        {
        //            pops.Add(CountryManager.instance.selectedPop);
        //            CountryManager.instance.selectedPop.provinceController = this;
        //        }
        //        CountryManager.instance.selectedPop.transform.position = Input.mousePosition;
        //        CountryManager.instance.available = true;
        //        CountryManager.instance.selectedPop = null;
        //        CountryManager.instance.VisibleMouse();
        //    }
        //    else if (CountryManager.instance.selectedPop != null && popCanMove == false && CountryManager.instance.available == true)
        //    {
        //        print("You cannot move here because you have no military access");
        //    }
        //    else if (CountryManager.instance.selectedPop == null && CountryManager.instance.available == true)
        //    {
        //        CountryManager.instance.window.target = owner;
        //        CountryManager.instance.window.provinceTarget = this;
        //        CountryManager.instance.window.gameObject.SetActive(true);
        //        CountryManager.instance.window.OnClicked();
        //        CountryManager.instance.openWindowSound.Play();
        //    }
        //}
        ////left click on province to open up province viewer
        //if (Input.GetKeyDown(KeyCode.Mouse0) && CountryManager.instance.available == true)
        //{
        //    CountryManager.instance.windowProvince.buildingInfoWindow.gameObject.SetActive(false);
        //    CountryManager.instance.windowProvince.targetCountry = owner;
        //    CountryManager.instance.windowProvince.target = this;
        //    CountryManager.instance.windowProvince.gameObject.SetActive(true);
        //    CountryManager.instance.windowProvince.OnClicked();
        //}
    }
    #endregion

    #region Army stuff (if pop can move)
    public void IfPopCanMove()
    {
        if (owner == CountryManager.instance.playerCountry || CountryManager.instance.playerCountry.atWar.Contains(owner))
        {
            popCanMove = true;
            CountryManager.instance.cursorIcon.GetComponent<Image>().color = Color.white;
        }
        else
        {
            popCanMove = false;
            CountryManager.instance.cursorIcon.GetComponent<Image>().color = Color.grey;
        }
    }
    #endregion

    #region Changing Province Ownership
    public void ChangeProvinceOwnership()
    {
        //changing ownership of pops -- have option to kill all or add to yours?
        for (int i = 0; i < pops.Count; i++)
        {
            if (pops[i].controller == owner)
            {
                CountryManager.instance.playerCountry.population.Add(pops[i]);
            }
            pops[i].controller = CountryManager.instance.playerCountry;
            owner.population.Remove(pops[i]);
            pops[i].RefreshColor();
        }
        //Removing province from old owner
        owner.ownedProvinces.Remove(this);
        //Calculating prestige gain
        float prestigeGain = 0f;
        for (int i = 0; i < holdings.Count; i++)
        {
            prestigeGain += CountryManager.instance.buildingPrestige;
        }
        for (int i = 0; i < pops.Count; i++)
        {
            prestigeGain += CountryManager.instance.popPrestige;
        }
        //adding and subtracting prestige values
        CountryManager.instance.playerCountry.prestige += prestigeGain;
        owner.prestige -= prestigeGain;
        if (owner.prestige < 0)
        {
            owner.prestige -= owner.prestige;
        }
        //destroying country if it owns no provinces & Ending war for both sides if country owns no more provinces
        if (owner.ownedProvinces.Count == 0)
        {
            CountryManager.instance.countries.Remove(owner);
            owner.atWar.Remove(CountryManager.instance.playerCountry);
            CountryManager.instance.playerCountry.atWar.Remove(owner);
        }
        //Setting new owner to be playerCountry
        owner = CountryManager.instance.playerCountry;
        //Adding Province to new owner
        owner.ownedProvinces.Add(this);
        RefreshProvinceColors();
        CountryManager.instance.window.CloseWindow();
        //CountryManager.instance.SetUI();
    }
    #endregion

    #region IClickables
    public void OnPointerEnter()
    {
        hovering = true;
        //if (CountryManager.instance.selectedPop != null)
        //{
        //    IfPopCanMove();
        //}
    }

    public void OnPointerExit()
    {
        hovering = false;
    }
    
    public bool IsProvince()
    {
        return true;
    }
    
    public Image GetImage()
    {
        return GetComponent<Image>();
    }

    public bool IsHolding()
    {
        return false;
    }
    #endregion
}
