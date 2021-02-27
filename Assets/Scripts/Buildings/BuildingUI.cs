using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pigeon;

public class BuildingUI : MonoBehaviour
{
    public const float ProductionPreviewWidth = 93.575f;
    public const float ProductionPopoutHeight = 39.1496f;
    public const float PopsPopoutWidth = 116.31f;

    public float ExpandSpeed = 2f;

    public RectTransform productionPreview;
    public RectTransform productionPopout;
    public RectTransform popsPopout;

    bool expanded;
    IEnumerator expandCoroutine;

    public EaseFunctions.EaseMode easeMode = EaseFunctions.EaseMode.EaseInOutQuintic;

    public void SetValues(Province.Holding.Building building)
    {
        //buildingText = building.buildingType.ToString()
    }

    public void PopoutBuildings()
    {
        expanded = !expanded;

        if (expandCoroutine != null)
        {
            StopCoroutine(expandCoroutine);
        }

        expandCoroutine = AnimateBuildings();
        StartCoroutine(expandCoroutine);

        //buildings[i].productionPreview.sizeDelta = new Vector2(0f, holdings[i].productionPreview.sizeDelta.y);
        //buildings[i].productionPopuout.sizeDelta = new Vector2(holdings[i].productionPopuout.sizeDelta.x, HoldingUI.ProductionPopoutHeight);
        //buildings[i].popsPopout.sizeDelta = new Vector2(HoldingUI.PopsPopoutWidth, holdings[i].popsPopout.sizeDelta.y);

        //if (i == index)
        //{

        //}
        //else
        //{
        //    holdings[i].productionPreview.sizeDelta = new Vector2(HoldingUI.ProductionPreviewWidth, holdings[i].productionPreview.sizeDelta.y);
        //    holdings[i].productionPopuout.sizeDelta = new Vector2(holdings[i].productionPopuout.sizeDelta.x, 0f);
        //    holdings[i].popsPopout.sizeDelta = new Vector2(0f, holdings[i].popsPopout.sizeDelta.y);
        //}
    }

    IEnumerator AnimateBuildings()
    {
        float initialProductionPreviewSize = productionPreview.sizeDelta.x;
        float initialProductionPopoutSize = productionPopout.sizeDelta.y;
        float initialPopsPopoutSize = popsPopout.sizeDelta.x;
        float time = 0f;
        float targetProductionPreviewSize = expanded ? 0 : ProductionPreviewWidth;
        float targetProductionPopoutSize = expanded ? ProductionPopoutHeight : 0;
        float targetPopsPopoutSize = expanded ? PopsPopoutWidth : 0;

        var easeFunction = EaseFunctions.SetEaseMode(easeMode);

        while (time < 1f)
        {
            time += Time.deltaTime * ExpandSpeed;

            if (time > 1f)
            {
                time = 1f;
            }

            float t = easeFunction.Invoke(time);
            productionPreview.sizeDelta = new Vector2(Mathf.LerpUnclamped(initialProductionPreviewSize, targetProductionPreviewSize, t), productionPreview.sizeDelta.y);
            productionPopout.sizeDelta = new Vector2(productionPopout.sizeDelta.x, Mathf.LerpUnclamped(initialProductionPopoutSize, targetProductionPopoutSize, t));
            popsPopout.sizeDelta = new Vector2(Mathf.LerpUnclamped(initialPopsPopoutSize, targetPopsPopoutSize, t), popsPopout.sizeDelta.y);

            yield return null;
        }

        expandCoroutine = null;
    }
}
