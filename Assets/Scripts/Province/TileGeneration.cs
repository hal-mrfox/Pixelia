using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine;


public class TileGeneration : MonoBehaviour
{
    public List<Holding> tiles;

    public void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            tiles.Add(transform.GetChild(i).GetComponent<Holding>());
        }
    }

    [Button]
    public void GenerateTerrain()
    {
        
    }
}
