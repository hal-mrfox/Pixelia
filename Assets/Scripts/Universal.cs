using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universal : MonoBehaviour
{
    public static Universal instance;

    public static Population popPrefab;

    public Population popPrefabSetter;

    public const float basePopGrowth = 2.5f;

    public AnimationCurve PopulationMultiplierGraph;

    public void Awake()
    {
        instance = this;
        popPrefab = popPrefabSetter;
    }
}
