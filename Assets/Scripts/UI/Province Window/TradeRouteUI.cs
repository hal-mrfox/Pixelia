using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class TradeRouteUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Holding holding;
    public HoldingUI holdingUI;
    public Image resourceIcon;
    public Image exporting;
    public TMP_InputField amountText;
    public TMP_Dropdown destinations;
    public ButtonSound continuous;
    public TradeRoute cP;

    bool hovering;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (hovering && Input.GetKeyDown(KeyCode.Mouse2))
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(cP.gameObject);
        holding.tradeRoutes.Remove(cP);
        holdingUI.tradeRoutes.Remove(this);
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void Refresh()
    {
        resourceIcon.sprite = cP.resource.icon;

        if (!cP.moving)
        {
            amountText.readOnly = false;
            destinations.enabled = true;

            if (int.TryParse(amountText.text, out int myInt))
            {
                if (holding.storedResources.Count > 0)
                {
                    for (int i = 0; i < holding.storedResources.Count; i++)
                    {
                        if (holding.storedResources[i].resource == cP.resource)
                        {
                            if (myInt > holding.storedResources[i].amount)
                            {
                                cP.amount = holding.storedResources[i].amount;
                                amountText.text = holding.storedResources[i].amount.ToString();
                            }
                            else
                            {
                                cP.amount = myInt;
                                amountText.text = myInt.ToString();
                            }
                        }
                    }
                }
                else
                {
                    cP.amount = 0;
                    amountText.text = "0";
                }
            }

            cP.continuous = continuous;

            for (int i = 0; i < CountryManager.instance.countries.Count; i++)
            {
                for (int j = 0; j < CountryManager.instance.countries[i].ownedHoldings.Count; j++)
                {
                    if (CountryManager.instance.countries[i].ownedHoldings[j].name == destinations.options[destinations.value].text)
                    {
                        cP.destination = CountryManager.instance.countries[i].ownedHoldings[j];
                        break;
                    }
                }
            }
        }
        else
        {
            amountText.readOnly = true;
            destinations.enabled = false;
        }
    }

    public void SendResources()
    {
        for (int k = 0; k < holding.storedResources.Count; k++)
        {
            if (holding.storedResources[k].resource == cP.resource)
            {
                holding.storedResources[k].amount -= cP.amount;
            }
        }
        if (cP.destination.storedResources.Count > 0)
        {
            bool foundResource = false;
            for (int k = 0; k < cP.destination.storedResources.Count; k++)
            {
                if (cP.destination.storedResources[k].resource == cP.resource)
                {
                    cP.destination.storedResources[k].amount += cP.amount;
                    foundResource = true;
                    break;
                }
            }
            if (!foundResource)
            {
                cP.destination.storedResources.Add(new Holding.ResourceAmount(cP.resource, cP.amount));
            }
        }
        else
        {
            cP.destination.storedResources.Add(new Holding.ResourceAmount(cP.resource, cP.amount));
        }

        holding.owner.Refresh();
        Refresh();
        holdingUI.provinceWindow.RefreshWindow();
    }
}
