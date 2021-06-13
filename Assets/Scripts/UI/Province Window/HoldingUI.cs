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
            holdingName.text = "holding";
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
        popsText.text = holdingCounterpart.pops.Count.ToString();
        unemployedText.text = holdingCounterpart.unemployedPops.Count.ToString();
        homelessText.text = holdingCounterpart.homelessPops.Count.ToString();

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
                }
                commodities[i].Refresh(holdingCounterpart.storedResources[i].resource, holdingCounterpart.storedResources[i].amount, resourceOutputValue, this);
                commodities[i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < tradeRoutes.Count; i++)
        {
            tradeRoutes[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < holdingCounterpart.tradeRoutes.Count; i++)
        {
            tradeRoutes[i].gameObject.SetActive(true);
            tradeRoutes[i].Refresh();
        }
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
