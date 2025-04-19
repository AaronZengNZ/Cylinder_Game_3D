using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverScript : MonoBehaviour
{
    public Outline hoverOutline;

    void Start()
    {
        hoverOutline.enabled = false;
    }

    public void Hover()
    {
        hoverOutline.enabled = true;
    }

    public void UnHover()
    {
        hoverOutline.enabled = false;
    }
}
