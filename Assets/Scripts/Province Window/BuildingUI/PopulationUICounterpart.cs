using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopulationUICounterpart : ButtonSound
{
    public Population PopCP;
    public bool work;

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
            if (interactable && buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
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

        buildingUI.provinceWindow.movingPop = PopCP;
        buildingUI.provinceWindow.job = work;
        clicking = true;

        #region Drag and Drop
        if (Input.GetKeyDown(KeyCode.Mouse0) && PopCP)
        {
            previewPop = new GameObject("Preview Pop");

            if (GetComponent<Image>().sprite)
            {
                previewPop.AddComponent<Image>();
                previewPop.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
                previewPop.GetComponent<Image>().SetNativeSize();
                previewPop.GetComponent<RectTransform>().sizeDelta = new Vector2(previewPop.GetComponent<RectTransform>().sizeDelta.x + 2, previewPop.GetComponent<RectTransform>().sizeDelta.y + 2);
            }
            else
            {
                previewPop.AddComponent<Image>();
                previewPop.GetComponent<RectTransform>().sizeDelta = new Vector2(squareSize.x + 2, squareSize.y + 2);
            }
            previewPop.GetComponent<Image>().raycastTarget = false;
            previewPop.GetComponent<Image>().color = GetComponent<Image>().color;
            previewPop.transform.SetParent(buildingUI.provinceWindow.transform);
        }
        #endregion
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        #region duimb
        if (buildingUI == null)
        {
            if (interactable)
            {
                if (hover)
                {
                    image.color = hovering;
                }
                else
                {
                    image.color = normal;
                }
            }
        }
        else
        {
            if (interactable && buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
            {
                if (hover)
                {
                    image.color = hovering;
                }
                else
                {
                    image.color = normal;
                }
            }
        }
        #endregion

        buildingUI.provinceWindow.DropPop();
        buildingUI.provinceWindow.movingPop = null;
        clicking = false;

        #region Drag and Drop
        Destroy(previewPop);
        #endregion
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        #region shit code
        if (buildingUI == null)
        {
            hover = true;
            if (lower)
            {
                audioSource.pitch = 1.7f;
                audioSource.PlayOneShot(sound);
            }
            else
            {
                audioSource.pitch = 2f;
                audioSource.PlayOneShot(sound);
            }

            if (image != null)
            {
                image.color = hovering;
            }
        }
        else
        {
            if (buildingUI.provinceWindow.target == CountryManager.instance.playerCountry)
            {
                hover = true;
                if (lower)
                {
                    audioSource.pitch = 1.7f;
                    audioSource.PlayOneShot(sound);
                }
                else
                {
                    audioSource.pitch = 2f;
                    audioSource.PlayOneShot(sound);
                }

                if (image != null)
                {
                    image.color = hovering;
                }
            }
        }
        #endregion

        if (PopCP)
        {
            buildingUI.provinceWindow.HighlightPop(work, PopCP);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        #region shitcode 2 electric boogaloo
        if (!toggled)
        {
            if (image != null)
            {
                image.color = normal;
            }
        }
        else
        {
            if (image != null)
            {
                image.color = clicked;
            }
        }

        if (buildingUI != null && highlight != null)
        {
            highlight.gameObject.SetActive(false);
        }

        hover = false;
        #endregion

        if (PopCP)
        {
            buildingUI.provinceWindow.RefreshWindow();
            GetComponent<Image>().SetNativeSize();
            if (!GetComponent<Image>().sprite)
            {
                GetComponent<RectTransform>().sizeDelta = squareSize;
            }
        }
    }

    public void Update()
    {
        if (clicking && PopCP)
        {
            previewPop.transform.position = Input.mousePosition;
        }
    }
}
