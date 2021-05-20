using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProvinceManager : MonoBehaviour
{
    public Province.ProvinceResource hoveredResource;
    public Province.ProvinceResource selectedResource;
    public Province selectedProvince;
    public int selectedHoldingValue;
    public int selectedBuildingValue;
    public int selectedResourceValue;
    public Building selectedBuilding;

    public static ProvinceManager instance;

    public GraphicRaycaster raycaster;

    public EventSystem eventSystem;

    public bool clickableQ;

    public static IClickable selectedClickable;

    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            Raycast()?.OnPointerDown();
        }

        IClickable clickable = Raycast();

        if (clickable != selectedClickable)
        {
            if (selectedClickable != null)
            {
                clickableQ = false;

                selectedClickable.OnPointerExit();

                selectedClickable = null;
            }

            if (clickable != null)
            {
                clickableQ = true;

                clickable.OnPointerEnter();
                selectedClickable = clickable;
            }
        }
    }

    public IClickable Raycast()
    {
        PointerEventData pointer = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        raycaster.Raycast(pointer, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent(out IClickable image))
            {
                Vector2 point = Input.mousePosition;

                point -= (Vector2)result.gameObject.transform.position;
                point += new Vector2(image.GetImage().sprite.texture.width * 0.5f, image.GetImage().sprite.texture.height * 0.5f);
                Color color = image.GetImage().sprite.texture.GetPixel((int)point.x, (int)point.y);
                if (color.a > 0)
                {
                    return image;
                }
            }
        }

        return null;
    }
}
