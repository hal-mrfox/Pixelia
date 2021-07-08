using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class BattlefieldUI : MonoBehaviour
{
    public Battlefield bF;
    public Tile tilePrefab;
    public GridLayoutGroup battlefieldLayout;
    public Tile[,] tiles;

    [Button]
    public void Refresh()
    {
        tiles = new Tile[bF.battlefield.GetLength(0), bF.battlefield.GetLength(1)];

    }
}
