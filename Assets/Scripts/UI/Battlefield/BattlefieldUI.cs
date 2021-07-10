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
    public Tile[,] tiles = new Tile[0, 0];

    [Button]
    public void Refresh()
    {
        //clearing
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Destroy(tiles[x, y].gameObject);
            }
        }

        //instantiating
        tiles = new Tile[bF.battlefield.GetLength(0), bF.battlefield.GetLength(1)];
        battlefieldLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(bF.battlefield.GetLength(0) * 20, bF.battlefield.GetLength(1) * 20);
        for (int y = 0; y < bF.battlefield.GetLength(0); y++)
        {
            for (int x = 0; x < bF.battlefield.GetLength(1); x++)
            {
                tiles[x, y] = Instantiate(tilePrefab, battlefieldLayout.transform);
                if (bF.battlefield[x, y] != null)
                {
                    tiles[x, y].icon.sprite = Resources.Load<UnitManager>("UnitManager").units[(int)bF.battlefield[x, y].occupyingUnit.unitType].icon;
                    tiles[x, y].icon.gameObject.SetActive(true);
                }
                else
                {
                    tiles[x, y].icon.gameObject.SetActive(false);
                }
            }
        }
    }
}
