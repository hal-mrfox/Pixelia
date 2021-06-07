using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum MapModes { Nations, Terrain }

[CreateAssetMenu(fileName = "MapModeManager", menuName = "MapModeManager")]
public class MapModeManager : ScriptableObject
{
    public MapModes mapMode;

    [Button]
    public void ChangeMapMode()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < CountryManager.instance.provinces.Count; i++)
            {
                CountryManager.instance.provinces[i].RefreshProvinceColors();
            }
        }
    }
}
