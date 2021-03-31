using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public AudioSource sound;
    public Image image;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (interactable)
        {
            if (buttonType == ButtonType.click)
            {
                image.color = clicked;
                sound.Play();
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    image.color = clicked;
                }
                onClick?.Invoke();
            }
            else if(buttonType == ButtonType.toggle)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    toggled = !toggled;
                    if (toggled)
                    {
                        image.color = clicked;
                    }
                    else
                    {
                        image.color = hovering;
                    }
                }
            }
        }
    }
    public void OnPointerUp(PointerEventData eventData)
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
        sound.Play();
        if (lower)
        {
            sound.pitch = 1.6f;
            sound.Play();
        }
        else
        {
            sound.pitch = 2f;
            sound.Play();
        }

        if (image != null)
        {
            image.color = hovering;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
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
        hover = false;
    }

}
