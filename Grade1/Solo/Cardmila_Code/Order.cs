using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] Renderer[] backRenderers;
    [SerializeField] Renderer[] middleRenderers;
    [SerializeField] string sortingLayerNamewow;
    private int origenOrder;

    public void SetOrigenOrder(int orOrder)
    {
        this.origenOrder = orOrder;
        SetOrder(orOrder);
    }
    public void SetTopOrder(bool isTop)
    {
        SetOrder(isTop ? 100 : origenOrder);
    }
    public void SetOrder(int ord)
    {
        int mOrder = ord * 10;

        foreach (Renderer r in backRenderers)
        {
            r.sortingLayerName = sortingLayerNamewow;
            r.sortingOrder = mOrder;
        }
        foreach (Renderer r in middleRenderers)
        {
            r.sortingLayerName = sortingLayerNamewow;
            r.sortingOrder = mOrder + 1;
        }
    }
}
