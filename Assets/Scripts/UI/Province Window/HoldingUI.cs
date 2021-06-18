using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class HoldingUI : MonoBehaviour
{
    public WindowProvince provinceWindow;
    public Holding holdingCounterpart;
    public int holdingCounterpartIndex;
    [Space(10)]

    public TextMeshProUGUI holdingName;
    public TextMeshProUGUI terrainTypeText;
    public TextMeshProUGUI popsText;
    public TextMeshProUGUI unemployedText;
    public TextMeshProUGUI homelessText;
    public TextMeshProUGUI holdingLevel;
    public TextMeshProUGUI buildingsCount;
    public TextMeshProUGUI storageCap;
    public ButtonSound createHoldingButton;
    public RawResourceUI[] rawResourcesUI;
    public Commodity[] commodities;
    public GridLayoutGroup tradeRoutesLayout;
    public List<TradeRouteUI> tradeRoutes;
    public TradeRouteUI tradeRoutePrefab;
    public TradeRoute realTradeRoutePrefab;

    #region refresh
    public void Refresh(bool built)
    {
        if (built)
        {
            createHoldingButton.gameObject.SetActive(false);
        }
        else
        {
            createHoldingButton.gameObject.SetActive(true);
        }

        for (int i = 0; i < rawResourcesUI.Length; i++)
        {
            rawResourcesUI[i].Refresh(holdingCounterpart.rawResources[i].resource, holdingCounterpart.rawResources[i].amount);
        }

        terrainTypeText.text = holdingCounterpart.terrainType.ToString();
        int houseCount = 0;
        for (int i = 0; i < holdingCounterpart.buildings.Count; i++)
        {
            if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)holdingCounterpart.buildings[i].buildingType].isHousing)
            {
                houseCount++;
            }
        }
        //Resources.Load<HoldingManager>("HoldingManager").holdings[holdingCounterpart.holdingType].holdingLevels[holdingCounterpart.holdingLevel]
        popsText.text = holdingCounterpart.pops.Count.ToString() + "/" +  holdingCounterpart.housedPopCap;
        buildingsCount.text = holdingCounterpart.buildings.Count.ToString() + "/" + holdingCounterpart.buildingCap;
        #region Storage
        int total = 0;
        for (int i = 0; i < holdingCounterpart.storedResources.Count; i++)
        {
            total += holdingCounterpart.storedResources[i].amount;
        }

        if (total < holdingCounterpart.storageCap)
        {
            storageCap.color = CountryManager.instance.tan;
        }
        else
        {
            storageCap.color = CountryManager.instance.yellow;
        }

        storageCap.text = total + "/" + holdingCounterpart.storageCap;
        #endregion
        unemployedText.text = holdingCounterpart.unemployedPops.Count.ToString();
        homelessText.text = holdingCounterpart.homelessPops.Count.ToString();

        #region Commodities
        for (int i = 0; i < commodities.Length; i++)
        {
            commodities[i].gameObject.SetActive(false);
            if (i < holdingCounterpart.storedResources.Count)
            {
                int resourceOutputValue = 0;
                //shows output number as "+000"
                for (int k = 0; k < holdingCounterpart.buildings.Count; k++)
                {
                    //adding
                    for (int h = 0; h < holdingCounterpart.buildings[k].resourceOutput.Count; h++)
                    {
                        if (holdingCounterpart.storedResources[i].resource == holdingCounterpart.buildings[k].resourceOutput[h].resource)
                        {
                            resourceOutputValue += holdingCounterpart.buildings[k].resourceOutput[h].resourceCount;
                        }
                    }

                    //subtracting
                    for (int o = 0; o < holdingCounterpart.buildings[k].resourceInput.Count; o++)
                    {
                        if (holdingCounterpart.storedResources[i].resource == holdingCounterpart.buildings[k].resourceInput[o].resource
                            && holdingCounterpart.buildings[k].resourceOutput[0].resourceCount != 0)
                        {
                            resourceOutputValue -= holdingCounterpart.buildings[k].resourceInput[o].resourceNeedsCount;
                        }
                    }

                    //subtracting pops
                    if (Resources.Load<BuildingManager>("BuildingManager").buildings[(int)holdingCounterpart.buildings[k].buildingType].isHousing)
                    {
                        for (int o = 0; o < holdingCounterpart.buildings[k].popsNeeds.Count; o++)
                        {
                            if (holdingCounterpart.buildings[k].popsNeeds[o].resource == holdingCounterpart.storedResources[i].resource
                                && holdingCounterpart.buildings[k].popsNeeds[o].resourceCount >= holdingCounterpart.buildings[k].popsNeeds[o].resourceNeedsCount)
                            {
                                bool found = false;
                                int amount = 0;
                                for (int u = 0; u < holdingCounterpart.buildings[k].housedPops.Count; u++)
                                {
                                    for (int y = 0; y < holdingCounterpart.buildings[k].housedPops[u].needs.Count; y++)
                                    {
                                        if (holdingCounterpart.buildings[k].housedPops[u].needs[y].resource == holdingCounterpart.buildings[k].popsNeeds[o].resource)
                                        {
                                            if (holdingCounterpart.buildings[k].housedPops[u].needs[y].progress == 0)
                                            {
                                                found = true;
                                                amount += 1;
                                            }
                                            else if (holdingCounterpart.buildings[k].housedPops[u].needs[y].progress != 0)
                                            {
                                                found = false;
                                            }
                                        }
                                    }
                                }
                                if (found)
                                {
                                    resourceOutputValue -= amount;
                                }
                            }
                        }
                    }
                }

                commodities[i].Refresh(holdingCounterpart.storedResources[i].resource, holdingCounterpart.storedResources[i].amount, resourceOutputValue, this);
                commodities[i].gameObject.SetActive(true);
            }
        }
        #endregion

        #region Trade Routes
        for (int i = 0; i < tradeRoutes.Count; i++)
        {
            tradeRoutes[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < holdingCounterpart.tradeRoutes.Count; i++)
        {
            tradeRoutes[i].gameObject.SetActive(true);
            tradeRoutes[i].Refresh();
        }
        #endregion

        holdingLevel.text = Resources.Load<HoldingManager>("HoldingManager").holdings[(int)holdingCounterpart.holdingType].holdingLevels[holdingCounterpart.holdingLevel].levelName;

        #region Buildings
        for (int i = 0; i < buildings.Length; i++)
        {
            buildings[i].gameObject.SetActive(false);

            if (i < holdingCounterpart.buildingCap && holdingCounterpart.owner == CountryManager.instance.playerCountry)
            {
                if (i < holdingCounterpart.buildings.Count)
                {
                    buildings[i].active = true;
                    buildings[i].fullyInactive = false;
                    buildings[i].gameObject.SetActive(true);
                    buildings[i].holdingCounterpart = holdingCounterpartIndex;
                    buildings[i].buildingCounterpartIndex = i;
                    buildings[i].buildingCounterpart = holdingCounterpart.buildings[i];
                    buildings[i].buildingType = holdingCounterpart.buildings[i].buildingType;
                    buildings[i].Refresh(holdingCounterpartIndex, i);
                }
                else if (i == holdingCounterpart.buildings.Count)
                {
                    buildings[i].active = false;
                    buildings[i].fullyInactive = false;
                    buildings[i].gameObject.SetActive(true);
                    buildings[i].holdingCounterpart = holdingCounterpartIndex;
                    buildings[i].Refresh(holdingCounterpartIndex, i);
                }
                else if (i > holdingCounterpart.buildings.Count)
                {
                    buildings[i].active = false;
                    buildings[i].fullyInactive = true;
                }
            }
            else if (i >= holdingCounterpart.buildingCap && holdingCounterpart.owner == CountryManager.instance.playerCountry)
            {
                buildings[i].active = false;
                buildings[i].fullyInactive = true;
            }
        }
        #endregion
    }
    #endregion

    public void CreateTradeRoute(Resource resource)
    {
        holdingCounterpart.tradeRoutes.Add(Instantiate(realTradeRoutePrefab, holdingCounterpart.transform));
        var tradeRoute = holdingCounterpart.tradeRoutes[holdingCounterpart.tradeRoutes.Count - 1];
        tradeRoute.name = "Trade Route " + (holdingCounterpart.tradeRoutes.Count - 1);
        tradeRoute.resource = resource;
        tradeRoute.amount = 0;
        tradeRoute.progress = 0;
        tradeRoute.progressGain = 0;
        tradeRoute.continuous = false;

        tradeRoutes.Add(Instantiate(tradeRoutePrefab, tradeRoutesLayout.transform));
        tradeRoutes[tradeRoutes.Count - 1].cP = tradeRoute;
        tradeRoutes[tradeRoutes.Count - 1].holding = holdingCounterpart;
        tradeRoutes[tradeRoutes.Count - 1].holdingUI = this;
        tradeRoutes[tradeRoutes.Count - 1].Refresh();

        //Dropdown
        tradeRoutes[holdingCounterpart.tradeRoutes.Count - 1].destinations.ClearOptions();
        for (int i = 0; i < holdingCounterpart.owner.ownedHoldings.Count; i++)
        {
            if (holdingCounterpart.owner.ownedHoldings[i] != holdingCounterpart)
            {
                tradeRoutes[holdingCounterpart.tradeRoutes.Count - 1].destinations.options.Add(new TMP_Dropdown.OptionData(holdingCounterpart.owner.ownedHoldings[i].name, null));
            }
        }

        for (int i = 0; i < CountryManager.instance.countries.Count; i++)
        {
            for (int j = 0; j < CountryManager.instance.countries[i].ownedHoldings.Count; j++)
            {
                if (CountryManager.instance.countries[i].ownedHoldings[j].name == tradeRoutes[holdingCounterpart.tradeRoutes.Count - 1].destinations.options[tradeRoutes[holdingCounterpart.tradeRoutes.Count - 1].destinations.value].text)
                {
                    tradeRoute.destination = CountryManager.instance.countries[i].ownedHoldings[j];
                    break;
                }
            }
        }
    }

    public void Awake()
    {
        provinceWindow = FindObjectOfType<WindowProvince>();
    }
    #region Scrolling
    public RectTransform buildingScroller;
    const float Top = -2f;
    const float Bot = -100f;

    public BuildingUI[] buildings;

    public void ScrollBuildings(float value)
    {
        buildingScroller.anchoredPosition = new Vector2(buildingScroller.anchoredPosition.x, Mathf.Lerp(Top, Bot, value));
    }
    #endregion
    //bool buildingsExpanded;

    //public List<RectTransform> buildings = new List<RectTransform>();

    //public void PopoutBuildings()
    //{
    //    buildingsExpanded = !buildingsExpanded;

    //    for (int i = 0; i < buildings.Count; i++)
    //    {
    //        buildings[i].productionPreview.sizeDelta = new Vector2(0f, holdings[i].productionPreview.sizeDelta.y);
    //        buildings[i].productionPopuout.sizeDelta = new Vector2(holdings[i].productionPopuout.sizeDelta.x, HoldingUI.ProductionPopoutHeight);
    //        buildings[i].popsPopout.sizeDelta = new Vector2(HoldingUI.PopsPopoutWidth, holdings[i].popsPopout.sizeDelta.y);

    //        if (i == index)
    //        {

    //        }
    //        else
    //        {
    //            holdings[i].productionPreview.sizeDelta = new Vector2(HoldingUI.ProductionPreviewWidth, holdings[i].productionPreview.sizeDelta.y);
    //            holdings[i].productionPopuout.sizeDelta = new Vector2(holdings[i].productionPopuout.sizeDelta.x, 0f);
    //            holdings[i].popsPopout.sizeDelta = new Vector2(0f, holdings[i].popsPopout.sizeDelta.y);
    //        }
    //    }

    //    selectedBuilding = index;
    //}

    //IEnumerator AnimateBuildings

    //public void PopoutBuilding(int index)
    //{
    //    if (index == selectedBuilding)
    //    {
    //        index = -1;
    //    }

    //    for (int i = 0; i < buildings.Count; i++)
    //    {
    //        if (i == index)
    //        {
    //            buildings[i].productionPreview.sizeDelta = new Vector2(0f, holdings[i].productionPreview.sizeDelta.y);
    //            buildings[i].productionPopuout.sizeDelta = new Vector2(holdings[i].productionPopuout.sizeDelta.x, HoldingUI.ProductionPopoutHeight);
    //            buildings[i].popsPopout.sizeDelta = new Vector2(HoldingUI.PopsPopoutWidth, holdings[i].popsPopout.sizeDelta.y);
    //        }
    //        else
    //        {
    //            holdings[i].productionPreview.sizeDelta = new Vector2(HoldingUI.ProductionPreviewWidth, holdings[i].productionPreview.sizeDelta.y);
    //            holdings[i].productionPopuout.sizeDelta = new Vector2(holdings[i].productionPopuout.sizeDelta.x, 0f);
    //            holdings[i].popsPopout.sizeDelta = new Vector2(0f, holdings[i].popsPopout.sizeDelta.y);
    //        }
    //    }

    //    selectedBuilding = index;
    //}
}
