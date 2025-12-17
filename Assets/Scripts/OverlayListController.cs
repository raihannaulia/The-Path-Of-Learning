using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OverlayListController : MonoBehaviour
{
    public List<GameObject> overlayObjects;

    void Start()
    {
        // Show all overlays at start
        foreach (var obj in overlayObjects)
        {
            obj.SetActive(false);
        }
    }

    public void ShowOverlay(int index)
    {
        for (int i = 0; i < overlayObjects.Count; i++)
        {
            overlayObjects[i].SetActive(i == index);
        }
    }
}

