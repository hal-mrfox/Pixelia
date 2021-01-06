using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Army : MonoBehaviour, IClickable
{
    [Range(0, 10)] public int armyLevel;
    [Range(0, 10)] public int damageLevel;
    [Range(0, 10)] public int defenseLevel;
    //[Range(0, 10)] public int range;

    public GameObject selectedOutline;
    public GameObject movementArrow;
    public GameObject armyStats;

    public CountryValues countryValues;
    public ArmyManager armyManager;

    public Province.Countries armyCountry;
    public ArmyTypes.ArmyType armyType;

    public ProvinceManager provinceManager;

    [Range(10, 200)] public int armyRange;

    public bool selected;

    public Image GetImage()
    {
        return GetComponent<Image>();
    }

    public bool IsProvince()
    {
        return false;
    }

    public void OnPointerDown()
    {
        if (countryValues.playerCountry == armyCountry)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                selected = !selected;
                //selectedOutline.SetActive(selected);

                if (countryValues.selectedArmy != null)
                {
                    countryValues.selectedArmy.selected = false;
                    countryValues.selectedArmy = null;
                }

                if (selected)
                {
                    countryValues.selectedArmy = this;
                    //provinceManager.line.gameObject.SetActive(true);
                }
                else
                {
                    countryValues.selectedArmy = null;
                }
            }
        }
    }

    public void OnPointerEnter()
    {
        
    }

    public void OnPointerExit()
    {
        
    }

    public void Start()
    {
        provinceManager = FindObjectOfType<ProvinceManager>();

        countryValues = FindObjectOfType<CountryValues>();

        GetComponent<Image>().color = countryValues.countries[(int)countryValues.playerCountry].countryColor;

        armyCountry = countryValues.playerCountry;
    }

    public void Update()
    {
        if (selected)
        {
            //draw line
            Vector2 armyPos = transform.localPosition;
            Vector2 mousePos = transform.parent.InverseTransformPoint(Input.mousePosition);
            
            //provinceManager.line.sizeDelta = new Vector2(Vector2.Distance(armyPos, mousePos), provinceManager.line.sizeDelta.y);
            //provinceManager.line.anchoredPosition = (armyPos + mousePos) * 0.5f;
            //provinceManager.line.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(mousePos.y - armyPos.y, mousePos.x - armyPos.x) * 180 / Mathf.PI);
            //
            ////show stats
            //armyStats.SetActive(true);
            //
            ////while selected
            //if (provinceManager.clickableQ && provinceManager.line.rect.width < armyRange)
            //{
            //    provinceManager.line.GetComponent<Image>().color = Color.green;
            //}
            //else
            //{
            //    provinceManager.line.GetComponent<Image>().color = Color.red;
            //}
        }
        else
        {
            armyStats.SetActive(false);
        }

        if (countryValues.selectedArmy == this)
        {
            selectedOutline.SetActive(true);
        }
        if (countryValues.selectedArmy != this)
        {
            selectedOutline.SetActive(false);
        }

        //if (Input.GetKeyDown(KeyCode.Mouse1) && selected && ProvinceManager.selectedClickable != null && ProvinceManager.selectedClickable.IsProvince() && provinceManager.line.rect.width < armyRange)
        //{
        //    selected = false;
        //    gameObject.transform.position = Input.mousePosition;
        //
        //    selectedOutline.SetActive(false);
        //    provinceManager.line.gameObject.SetActive(false);
        //
        //
        //    countryValues.selectedArmy = null;
        //}
    }
}
