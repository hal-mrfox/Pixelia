using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Unit : ButtonSound
{
    [Header("Data")]
    public Province location;
    public Country owner;
    public int numPops;

    [Header("UI")]
    public Image[] identifiers;
    public TextMeshProUGUI numPopsText;

    bool clicking;

    public void Refresh()
    {
        if (owner == CountryManager.instance.playerCountry)
        {
            for (int i = 0; i < identifiers.Length; i++)
            {
                identifiers[i].color = CountryManager.instance.niceGreen;
            }
        }
        else
        {
            for (int i = 0; i < identifiers.Length; i++)
            {
                identifiers[i].color = CountryManager.instance.niceGray;
            }
        }

        numPopsText.text = numPops.ToString();
    }

    [Button]
    public void TakeHolding()
    {
        MilitaryManager.instance.hoveringHolding.TransferOwnership(owner);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MilitaryManager.instance.selectedUnit = this;
            clicking = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            TakeHolding();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        MilitaryManager.instance.selectedUnit = null;

        clicking = false;
    }

    public void Update()
    {
        if (clicking)
        {
            transform.position = Input.mousePosition;
        }
    }
}
