using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class RawResourceUI : MonoBehaviour
{
    public Image icon;

    public Image[] qualityUI;

    public void Refresh(Resource resource, int quantity)
    {
        icon.sprite = resource.icon;
        icon.SetNativeSize();
        for (int i = 0; i < 3; i++)
        {
            if (i < quantity)
            {
                qualityUI[i].color = Resources.Load<UIManager>("UIManager").popWhite;
            }
            else
            {
                qualityUI[i].color = Resources.Load<UIManager>("UIManager").popGray;
            }
        }

        if (quantity == 4)
        {
            qualityUI[3].gameObject.SetActive(true);
            qualityUI[3].color = CountryManager.instance.niceGreen;
        }
        else
        {
            qualityUI[3].gameObject.SetActive(false);
        }
    }
}
