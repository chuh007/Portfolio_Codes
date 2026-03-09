using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    [SerializeField] MovecardUI move;
    [SerializeField] moveTilewow tile1;
    [SerializeField] moveTilewow tile2;
    [SerializeField] moveTilewow tile3;
    [SerializeField] moveTilewow tile4;
    private void Awake()
    {
        move.CanMove+=ArrowSetSee;
    }
    public void ArrowSetSee(bool wow)
    {
        if (wow==false)
        {
            tile1.CheckAndSetActive();
            tile2.CheckAndSetActive();
            tile3.CheckAndSetActive();
            tile4.CheckAndSetActive();
        }
        else
        {
            tile1.CancelUse();
            tile2.CancelUse();
            tile3.CancelUse();
            tile4.CancelUse();
        }
    }
}
