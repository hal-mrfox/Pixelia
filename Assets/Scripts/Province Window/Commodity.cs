using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Commodity : MonoBehaviour
{
    public Image resource;
    public Image surplusDefecit;
    public Image outline;
    public TextMeshProUGUI quantity;
    public TextMeshProUGUI difference;

    public void Refresh(Sprite resource, int quantity, int difference)
    {
        this.resource.sprite = resource;
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
