using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryManager : MonoBehaviour
{
    public static MilitaryManager instance;
    public Unit unitPrefab;

    public void Awake()
    {
        instance = this;
    }
}
