using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
public class BeliefsManager : MonoBehaviour
{
    public static BeliefsManager instance;

    public List<Religion> religions;
    public List<Culture> cultures;
    public List<Ideology> ideologies;
    public List<Nationality> nationalities;

    [System.Serializable]
    public class Religion
    {
        public string name;
    }
    [System.Serializable]
    public class Culture
    {
        public string name;
    }
    [System.Serializable]
    public class Ideology
    {
        public string name;
    }
    [System.Serializable]
    public class Nationality
    {
        public string name;
    }

    public void Awake()
    {
        instance = this;
    }
}
