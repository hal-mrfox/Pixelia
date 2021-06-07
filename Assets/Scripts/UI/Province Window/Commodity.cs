using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Commodity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public HoldingUI holding;

    public Resource resourceActual;
    public Image resource;
    public Image surplusDefecit;
    public Image outline;
    public TextMeshProUGUI quantity;
    public TextMeshProUGUI difference;

    bool hovering;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (hovering && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            holding.CreateTradeRoute(resourceActual);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void Refresh(Resource resource, int quantity, int difference, HoldingUI holding)
    {
        this.holding = holding;
        resourceActual = resource;
        this.resource.sprite = resource.icon;
        this.quantity.text = quantity.ToString();
        this.difference.text = difference.ToString(difference >= 0 ? "+0" : "0");
        if (difference > 0)
        {
            this.difference.color = CountryManager.instance.niceGreen;
        }
        else if (difference == 0)
        {
            this.difference.color = CountryManager.instance.yellow;
        }
        else
        {
            this.difference.color = CountryManager.instance.niceRed;
        }
        this.resource.GetComponent<Image>().SetNativeSize();
        //this.surplusDefecit.GetComponent<Image>().SetNativeSize();
        //this.outline.GetComponent<Image>().SetNativeSize();
    }
}
