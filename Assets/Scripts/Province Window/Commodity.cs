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
    public TextMeshProUGUI sDQuantity;

    public void Refresh(Sprite resource, Sprite surplusDefecit, Sprite outline, int quantity)
    {
        this.resource.sprite = resource;
        this.surplusDefecit.sprite = surplusDefecit;
        this.outline.sprite = outline;
        this.quantity.text = quantity.ToString();
        this.resource.GetComponent<Image>().SetNativeSize();
        this.surplusDefecit.GetComponent<Image>().SetNativeSize();
        this.outline.GetComponent<Image>().SetNativeSize();
    }
}
