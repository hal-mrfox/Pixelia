﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pigeon
{
    public class ColorButton : Button
    {
        [Space(10f)]
        public Graphic mainGraphic;

        Color defaultColor;
        [SerializeField] Color hoverColor;
        [SerializeField] Color clickColor;
        HSVColor defaultHSV, hoverHSV, clickHSV;

        public Color DefaultColor
        {
            get => defaultColor;
            set
            {
                defaultColor = value;
                defaultHSV = value;
            }
        }

        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                hoverColor = value;
                hoverHSV = value;
            }
        }

        public Color ClickColor
        {
            get => clickColor;
            set
            {
                clickColor = value;
                clickHSV = value;
            }
        }

        protected void Reset()
        {
            if (!mainGraphic)
            {
                mainGraphic = GetComponent<Graphic>();
            }
        }

        public override void Awake()
        {
            base.Awake();

            defaultColor = mainGraphic.color;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (ignoreEvents && eventData != null)
            {
                return;
            }

            hovering = true;
            SetToNull(hoverCoroutine);
            hoverCoroutine = AnimateThickness(hoverColor, hoverSpeed);
            StartCoroutine(hoverCoroutine);

            OnHoverEnter?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (ignoreEvents && eventData != null)
            {
                return;
            }

            if (clicking)
            {
                clicking = false;
                OnClickUp?.Invoke();
            }

            hovering = false;
            SetToNull(hoverCoroutine);
            hoverCoroutine = AnimateThickness(defaultColor, hoverSpeed);
            StartCoroutine(hoverCoroutine);

            OnHoverExit?.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            //print(eventData?.position);
            if (ignoreEvents && eventData != null)
            {
                return;
            }

            clicking = true;
            SetToNull(hoverCoroutine);
            hoverCoroutine = AnimateThickness(clickColor, clickSpeed);
            StartCoroutine(hoverCoroutine);

            OnClickDown?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (ignoreEvents && eventData != null)
            {
                return;
            }

            clicking = false;
            SetToNull(hoverCoroutine);
            if (hovering)
            {
                hoverCoroutine = AnimateThickness(hoverColor, clickSpeed);
            }
            StartCoroutine(hoverCoroutine);

            OnClickUp?.Invoke();
        }

        IEnumerator AnimateThickness(Color targetColor, float speed)
        {
            Color initialColor = mainGraphic.color;

            float time = 0f;

            while (time < 1f)
            {
                time += speed * Time.deltaTime;
                if (time > 1f)
                {
                    time = 1f;
                }

                mainGraphic.color = Color.LerpUnclamped(initialColor, targetColor, easingFunctionClick.Invoke(time));

                yield return null;
            }
        }
    }
}