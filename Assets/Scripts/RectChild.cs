using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pigeon
{
    public class RectChild : RectSizeButton
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            rectTransform.SetAsLastSibling();
        }
    }
}
