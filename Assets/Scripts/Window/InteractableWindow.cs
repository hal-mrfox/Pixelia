using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class InteractableWindow : MonoBehaviour
{
    public List<InteractableWindow> subWindows;
    public void CloseWindow()
    {
        gameObject.SetActive(false);

        //setting all subwindows to inactive
        for (int i = 0; i < subWindows.Count; i++)
        {
            //subWindows[i].gameObject.SetActive(false);
        }
    }
    public virtual void Update()
    {
        
    }
}
