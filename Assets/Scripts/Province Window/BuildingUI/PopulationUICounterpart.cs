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

        if (PopCP != null)
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

        if (PopCP != null)
        {
            buildingUI.provinceWindow.RefreshWindow();
            GetComponent<Image>().SetNativeSize();
            if (GetComponent<Image>().sprite == null)
            {
                GetComponent<RectTransform>().sizeDelta = squareSize;
            }
        }
    }
}
