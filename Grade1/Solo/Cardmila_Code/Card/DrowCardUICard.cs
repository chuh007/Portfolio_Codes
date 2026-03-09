using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrowCardUICard : MonoBehaviour, IPointerClickHandler
{
    CardManager cardM;
    public static int canDrowCount = 5;
    [SerializeField] private TextMeshProUGUI text;
    private void Awake()
    {
        canDrowCount = 5;
        TurnManager turn = TurnManager.Instance;
        turn.MyTurnstartEvent +=isCanDrow;
        turn.MyTurnEndEvent +=isCantDrow;
        cardM = FindObjectOfType<CardManager>();
    }
    private void Start()
    {
        text.text = canDrowCount.ToString();
    }
    private void Update()
    {
        text.text = canDrowCount.ToString();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TurnManager.isMyTurn && canDrowCount > 0)
        {
            cardM.AddCard();
            canDrowCount--;
            text.text = canDrowCount.ToString();
        }
    }
    public void isCanDrow()
    {
        canDrowCount++;
        text.text = canDrowCount.ToString();
    }
    private void isCantDrow()
    {
        canDrowCount = 0;
    }
}
