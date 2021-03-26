using System.Collections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class Slider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform handle;
    public RectTransform background;
    bool clicking;
    public TextMeshProUGUI textValue;
    public float value;
    public bool leftStopper;
    [ShowIf("leftStopper")]
    public float lStopperValue;
    public bool rightStopper;
    [ShowIf("rightStopper")]
    public float rStopperValue;

    public UnityEvent<float> onValueChanged;

    public void OnPointerDown(PointerEventData eventData)
    {
        clicking = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicking = false;
    }

    public void Update()
    {
        if (clicking)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(background, Input.mousePosition, null, out Vector2 position);
            position.x = Mathf.Clamp(position.x, background.rect.xMin + handle.rect.width / 2, background.rect.xMax - handle.rect.width / 2);
            handle.anchoredPosition = new Vector2(position.x, handle.anchoredPosition.y);
            float value = Mathf.InverseLerp(background.rect.xMin + handle.rect.width / 2, background.rect.xMax - handle.rect.width / 2, position.x);
            onValueChanged?.Invoke(value);

            if (textValue)
            {
                textValue.text = value.ToString("0.00");
            }
        }
    }
}
