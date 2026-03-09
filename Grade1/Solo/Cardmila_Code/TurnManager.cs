using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TurnManager : Singleton<TurnManager>
{
    public static bool isMyTurn = true;
    public static bool isYourturn = false;
    public event Action MyTurnEndEvent;
    public event Action EnemyTurnEndEvent;
    public event Action MyTurnstartEvent;
    public event Action EnemyTurnStartEvent;
    public static int TurnCount = 0;
    public GameObject boss;
    public Image TurnImage;
    public TextMeshProUGUI WhoTurn;
    public TextMeshProUGUI TurnCountTxt;
    private void Awake()
    {
        isMyTurn = true;
        isYourturn = false;
        TurnCount = 0;
    }
    public void MyTurnStart()
    {
        TurnCount++;
        isMyTurn = true;
        StartCoroutine(TurnTxtSet());
        isYourturn = false;
        MyTurnstartEvent?.Invoke();
    }
    private IEnumerator TurnTxtSet()
    {
        TurnImage.gameObject.SetActive(true);
        TurnImage.DOFade(1, 0.5f);
        WhoTurn.DOFade(1, 0.5f);
        TurnCountTxt.DOFade(1, 0.5f);
        WhoTurn.text = "나의 차례";
        TurnCountTxt.text = TurnCount + "턴 경과";
        yield return new WaitForSeconds(1);
        TurnImage.DOFade(0, 1);
        WhoTurn.DOFade(0, 1);
        TurnCountTxt.DOFade(0, 1);
        yield return new WaitForSeconds(1);
        TurnImage.gameObject.SetActive(false);
    }
    public void MyTurnEnd()
    {
        if (isMyTurn&&CardManager.iscatch==false&&MovecardUI.isSeeArrow==false)
        {
            if (TurnCount > 49)
            {
                boss.SetActive(true);
            }
            MyTurnEndEvent?.Invoke();
             EnemyTurnStart();
        }
    }
    public void EnemyTurnStart()
    {
        isMyTurn = false;
        isYourturn = true;
        EnemyTurnStartEvent?.Invoke();
        
    }
    public void EnemyTurnEnd()
    {
        if (isYourturn)
        {
            EnemyTurnEndEvent?.Invoke();
            MyTurnStart();
        }
    }
    
}
