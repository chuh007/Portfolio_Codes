using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private TextMeshProUGUI text3;
    float f;
    int i;
    private void FixedUpdate()
    {
        f += Time.fixedDeltaTime;
        i = (int)f;
    }
    public void Gameover()
    {
        text.text = "GAME OVER";
        text1.color = Color.red;
        text1.text = "경과 턴 : "+TurnManager.TurnCount;
        text2.text = "처치한 적 : "+EnemyManager.EnemyKillCount;
        text3.text = "생존 시간 : "+i+"초";
    }
    public void GameClear()
    {
        text.text = "GAME CLEAR";
        text.color = Color.green;
        text1.text = "경과 턴 : " + TurnManager.TurnCount;
        text2.text = "처치한 적 : " + EnemyManager.EnemyKillCount;
        text3.text = "걸린 시간 : " + i + "초";
    }
}
