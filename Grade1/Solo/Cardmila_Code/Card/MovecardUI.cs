using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovecardUI : MonoBehaviour, IPointerClickHandler
{
    public static bool isSeeArrow;
    public static bool isUseMove;
    public event Action<bool> CanMove;
    public int plcanMoveCount = 5;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] CardLine cardLine;
    private RectTransform rectTransform;

    private void Awake()
    {
        plcanMoveCount = 5;
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        TurnManager turn = TurnManager.Instance;
        turn.MyTurnstartEvent += MyTurnS;
        text.text = $"{plcanMoveCount}/5";
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1)&&isSeeArrow)
        {
            isUseMove = false;
            EndLineWow();
            CanMove.Invoke(isSeeArrow);
            isSeeArrow = false;
            plcanMoveCount++;
            text.text = $"{plcanMoveCount}/5";
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TurnManager.isMyTurn && plcanMoveCount > 0 &&!isSeeArrow && CardManager.iscatch==false)
        {
            isUseMove = true;
            cardLine.CardLineWow(rectTransform);
            CanMove.Invoke(isSeeArrow);
            isSeeArrow = true;
            plcanMoveCount--;
            text.text = $"{plcanMoveCount}/5";
        }
    }
    public void MyTurnS()
    {
        if (plcanMoveCount < 5)
        {
            plcanMoveCount = 5;
        }
        text.text = $"{plcanMoveCount}/5";
    }
    public void EndLineWow()
    {
        cardLine.EndLine();
    }
}
