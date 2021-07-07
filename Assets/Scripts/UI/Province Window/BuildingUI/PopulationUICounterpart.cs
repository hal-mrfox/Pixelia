using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopulationUICounterpart : ButtonSound
{
    public WindowProvince provinceWindow;
    public Population popCP;
    public enum UIType { job, home, list }
    public UIType uIType;

    public TextMeshProUGUI popName;

    #region Pop List Stuff
    [BoxGroup("Pop List")]
    public Image popTierUIStrip;
    [BoxGroup("Pop List")]
    public Image jobStatus;
    [BoxGroup("Pop List")]
    public Image homeStatus;
    #endregion
    #region Housing
    public Image[] hungerPips;
    public Image[] moodPips;
    public ResourceConsuming[] resources;

    [System.Serializable]
    public class ResourceConsuming
    {
        public Image resource;
        public Image progress;
        public GameObject gameObject;

        public ResourceConsuming(Image resource, Image progress, GameObject consuming)
        {
            this.resource = resource;
            this.progress = progress;
            this.gameObject = consuming;
        }
    }
    #endregion

    bool clicking;
    GameObject previewPop;

    Vector2Int squareSize = new Vector2Int(10, 10);
    public override void OnPointerDown(PointerEventData eventData)
    {
        #region fuck shit
        if (buildingUI == null)
        {
            if (interactable)
            {
                if (buttonType == ButtonType.click)
                {
                    audioSource.PlayOneShot(sound);
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        image.color = clicked;
                    }
                    onClick?.Invoke();
                }
                else if (buttonType == ButtonType.toggle)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        toggled = !toggled;
                        if (toggled)
                        {
                            image.color = clicked;
                            buildingUI.ToggleBuilding(toggled);
                        }
                        else
                        {
                            image.color = hovering;
                            buildingUI.ToggleBuilding(toggled);
                        }
                        audioSource.PlayOneShot(sound);
                    }
                }
            }
        }
        else
        {
            if (interactable && buildingUI.provinceWindow.targetCountry == CountryManager.instance.playerCountry)
            {
                if (buttonType == ButtonType.click)
                {
                    audioSource.PlayOneShot(sound);
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        image.color = clicked;
                    }
                    onClick?.Invoke();
                }
                else if (buttonType == ButtonType.toggle)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        toggled = !toggled;
                        if (toggled)
                        {
                            image.color = clicked;
                            buildingUI.ToggleBuilding(toggled);
                        }
                        else
                        {
                            image.color = hovering;
                            buildingUI.ToggleBuilding(toggled);
                        }
                        audioSource.PlayOneShot(sound);
                    }
                }
            }
        }
        #endregion

        #region Drag and Drop
        if (Input.GetKeyDown(KeyCode.Mouse0) && popCP && uIType != UIType.list)
        {
            buildingUI.provinceWindow.movingPop = popCP;
            if (uIType == UIType.job)
            {
                provinceWindow.job = true;
            }
            else
            {
                provinceWindow.job = false;
            }
            clicking = true;

            previewPop = new GameObject("Preview Pop");

            previewPop.AddComponent<Image>();
            previewPop.GetComponent<RectTransform>().sizeDelta = new Vector2(squareSize.x + 2, squareSize.y + 2);
            previewPop.GetComponent<Image>().raycastTarget = false;
            previewPop.GetComponent<Image>().color = CountryManager.instance.niceGreen;
            //if (GetComponent<Image>().sprite)
            //{
            //    previewPop.AddComponent<Image>();
            //    previewPop.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
            //    previewPop.GetComponent<Image>().SetNativeSize();
            //    previewPop.GetComponent<RectTransform>().sizeDelta = new Vector2(previewPop.GetComponent<RectTransform>().sizeDelta.x + 2, previewPop.GetComponent<RectTransform>().sizeDelta.y + 2);
            //}
            //else
            //{
            ////}
            previewPop.transform.SetParent(provinceWindow.transform);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && popCP && uIType == UIType.list)
        {
            provinceWindow.movingPop = popCP;
            if (uIType == UIType.job)
            {
                provinceWindow.job = true;
            }
            else
            {
                provinceWindow.job = false;
            }
            clicking = true;

            previewPop = new GameObject("Preview Pop");

            previewPop.AddComponent<Image>();
            previewPop.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x + 2, GetComponent<RectTransform>().sizeDelta.y + 2);

            previewPop.GetComponent<Image>().raycastTarget = false;
            previewPop.GetComponent<Image>().color = new Color(GetComponent<PopulationUICounterpart>().normal.r, GetComponent<PopulationUICounterpart>().normal.g, GetComponent<PopulationUICounterpart>().normal.b, 0.5f);
            previewPop.transform.SetParent(provinceWindow.transform);
        }
        #endregion

        #region Unassign Pops
        if (Input.GetKeyDown(KeyCode.Mouse1) && popCP && !provinceWindow.movingPop)
        {
            if (uIType == UIType.home)
            {
                popCP.home.housedPops.Remove(popCP);
                popCP.home = null;
                popCP = null;
            }

            if (uIType == UIType.job)
            {
                popCP.job.workingPops.Remove(popCP);
                popCP.transform.SetParent(popCP.job.holding.transform);
                popCP.job.holding.unemployedPops.Add(popCP);
                popCP.job = null;
                popCP = null;
            }

            provinceWindow.target.RefreshProvinceValues();
            provinceWindow.targetCountry.Refresh();
            provinceWindow.RefreshWindow();
        }
        #endregion
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (popCP)
        {
            provinceWindow.DropPop();

            clicking = false;

            #region Drag and Drop
            Destroy(previewPop);
            #endregion

            provinceWindow.movingPop = null;
        }
    }

    #region highlighting
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (popCP)
        {
            provinceWindow.HighlightPop(popCP, uIType);
        }

        //if (popCP)
        //{
        //    if (uIType == UIType.home)
        //    {
        //        if (popCP.job)
        //        {
        //            GetJob().SetActive(true);
        //        }

        //        if (popCP.home)
        //        {
        //            GetHome().SetActive(false);
        //        }
        //    }
        //    else
        //    {
        //        if (popCP.job)
        //        {
        //            GetJob().SetActive(false);
        //        }

        //        if (popCP.home)
        //        {
        //            GetHome().SetActive(true);
        //        }
        //    }
        //}
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (popCP)
        {
            provinceWindow.UnHighlightPops(popCP, uIType);
        }

        //if (popCP)
        //{
        //    if (popCP.job)
        //    {
        //        GetJob().SetActive(false);
        //    }

        //    if (popCP.home)
        //    {
        //        GetHome().SetActive(false);
        //    }
        //}
    }

    GameObject GetJob()
    {
        int jobHolding = provinceWindow.target.holdings.IndexOf(popCP.job.holding);
        int jobBuilding = provinceWindow.target.holdings[jobHolding].buildings.IndexOf(popCP.job);
        int jobPop = provinceWindow.target.holdings[jobHolding].buildings[jobBuilding].workingPops.IndexOf(popCP);
        var jobPopIconsGameObject = provinceWindow.holdings[jobHolding].buildings[jobBuilding].jobPopIcons[jobPop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

        return jobPopIconsGameObject;
    }

    GameObject GetHome()
    {
        int homeHolding = provinceWindow.target.holdings.IndexOf(popCP.home.holding);
        int homeBuilding = provinceWindow.target.holdings[homeHolding].buildings.IndexOf(popCP.home);
        int homePop = provinceWindow.target.holdings[homeHolding].buildings[homeBuilding].housedPops.IndexOf(popCP);
        var homePopIconsGameObject = provinceWindow.holdings[homeHolding].buildings[homeBuilding].housedPopIcons[homePop].GetComponent<PopulationUICounterpart>().highlight.gameObject;

        return homePopIconsGameObject;
    }
    #endregion

    public void Refresh()
    {
        popName.text = popCP.popName;

        for (int i = 0; i < hungerPips.Length; i++)
        {
            if (i < popCP.hunger)
            {
                hungerPips[i].color = Resources.Load<UIManager>("UIManager").popWhite;
            }
            else
            {
                hungerPips[i].color = Resources.Load<UIManager>("UIManager").popGray;
            }
        }

        for (int i = 0; i < moodPips.Length; i++)
        {
            if (i < popCP.mood)
            {
                moodPips[i].color = Resources.Load<UIManager>("UIManager").popWhite;
            }
            else
            {
                moodPips[i].color = Resources.Load<UIManager>("UIManager").popGray;
            }
        }

        for (int i = 0; i < resources.Length; i++)
        {
            resources[i].resource.sprite = popCP.needs[i].resource.icon;
            resources[i].progress.GetComponent<Image>().fillAmount = popCP.needs[i].progress * 0.10f;
        }
    }

    public void Update()
    {
        if (clicking && popCP)
        {
            previewPop.transform.position = Input.mousePosition;
        }
    }
}
