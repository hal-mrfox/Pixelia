using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public BuildingUI buildingUI;
    public AudioSource audioSource;
    public AudioClip sound;
    public Image image;
    public Image highlight;
    public enum ButtonType { click, toggle }
    public ButtonType buttonType;
    public bool toggled;
    public bool lower;
    public Color normal;
    public Color hovering;
    public Color clicked;
    public bool interactable;
    public bool hover;

    public UnityEvent onClick;
    public virtual void OnPointerDown(PointerEventData eventData)
    {
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
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
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
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
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
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
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
    }

    //public virtual void Update()
    //{
    //    if (buildingUI != null && hover && buildingUI.provinceW)
    //    {
    //
    //    }
    //}
}
