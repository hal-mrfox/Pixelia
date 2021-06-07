using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeRoute : MonoBehaviour
{
    public Resource resource;
    public int amount;
    public int cap;
    public Holding destination;
    public float progress;
    public float progressGain;
    public bool continuous;
    public bool moving;
}
