using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public static WindowManager instance;

    WindowCountry countryWindow;
    WindowProvince provinceWindow;

    public void Awake()
    {
        instance = this;
    }
}
