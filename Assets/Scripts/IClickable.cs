using UnityEngine.EventSystems;
public interface IClickable
{
    UnityEngine.UI.Image GetImage();
    void OnPointerEnter();
    void OnPointerExit();
    void OnPointerDown();
    bool IsProvince();
    bool IsHolding();
}