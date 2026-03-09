using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Item
{
    public string cardName;
    public string cardText;
    public int attack;
    public int defence;
    public int cost;
    public int DrowCount;
    public int CureCostCount;
    public int nextTurnCost;
    public Sprite image;
    public int count;
    public bool isattcak;
}

[CreateAssetMenu(fileName ="ItemSO", menuName ="Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] item;
}
