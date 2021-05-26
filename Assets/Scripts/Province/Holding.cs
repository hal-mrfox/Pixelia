using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Holding : MonoBehaviour, IClickable
{
    public Country owner;
    public Province provinceOwner;
    public List<Building> buildings = new List<Building>();
    public AudioClip clickSound;

    public void OnPointerDown()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && owner)
        {
            CountryManager.instance.windowProvince.target = provinceOwner;
            CountryManager.instance.windowProvince.targetCountry = owner;
            CountryManager.instance.windowProvince.gameObject.SetActive(true);
            CountryManager.instance.windowProvince.OnClicked();
        }
    }

    public void OnPointerEnter()
    {
        CountryManager.instance.GetComponent<AudioSource>().PlayOneShot(clickSound);

        transform.position = new Vector3(transform.position.x, transform.position.y + 2);

        transform.SetAsLastSibling();
        GetComponent<Image>().color = CountryManager.instance.orange;
    }

    public void OnPointerExit()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (owner == CountryManager.instance.playerCountry)
        {
            transform.position = new Vector3(provinceOwner.transform.position.x, provinceOwner.transform.position.y + 2);
        }
        else
        {
            transform.position = new Vector3(provinceOwner.transform.position.x, provinceOwner.transform.position.y);
        }

        if (owner)
        {
            GetComponent<Image>().color = owner.countryColor;
        }
        else
        {
            GetComponent<Image>().color = CountryManager.instance.niceGray;
        }
    }

    #region IClickables
    public Image GetImage()
    {
        return GetComponent<Image>();
    }

    public bool IsHolding()
    {
        return true;
    }

    public bool IsProvince()
    {
        return false;
    }
    #endregion
}
