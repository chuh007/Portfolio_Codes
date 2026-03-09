using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    Vector2 mousePos;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }
    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.SetPosition(1, mousePos);
        
    }
    public void CardLineWow(RectTransform wow)
    {
        
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, Camera.main.ScreenToWorldPoint(wow.position));
    }
    public void EndLine()
    {
        lineRenderer.enabled = false;
    }
}
