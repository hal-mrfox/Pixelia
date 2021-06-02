using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryManager : MonoBehaviour
{
    public static MilitaryManager instance;
    public Unit unitPrefab;
    public Unit selectedUnit;

    public Holding hoveringHolding;

    public void Awake()
    {
        instance = this;
    }
}
