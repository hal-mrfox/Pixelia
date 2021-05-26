using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HoldingUI : MonoBehaviour
{
    public WindowProvince provinceWindow;
    public int holdingCounterpart;
    [Space(10)]

    public TextMeshProUGUI holdingName;
    public ButtonSound createHoldingButton;

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
    }

    public void CreateHolding()
    {
        //provinceWindow.provinceTarget.CreateHolding();
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
