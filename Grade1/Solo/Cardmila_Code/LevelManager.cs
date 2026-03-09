using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public UnityEvent LevelUp;
    public UnityEvent LevelUpEnd;
    public Item[] item;
    public ItemSO itemSO;
    [SerializeField] private CardManager cardManager;

    private void Awake()
    {
        item = itemSO.item;
    }
    
    public void LevelUpWow()
    {
        LevelUp?.Invoke();
        int a = Random.Range(0, item.Length);
    }
    public void EndLevelUI()
    {
        LevelUpEnd?.Invoke();
    }
}
