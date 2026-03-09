using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    TurnManager turnManager;
    List<GameObject> enemys;
    [SerializeField] private EnemySpown enemySpown;
    public static int EnemyKillCount= 0;
    int a = 1;
    private void Awake()
    {
        enemys = new List<GameObject>();
        turnManager = TurnManager.Instance;
    }
    public void PushEnemy(GameObject wow)
    {
        a = 1;
        enemys.Add(wow);
    }
    public void PopEnemy(GameObject enemyObj)
    {
        EnemyKillCount++;
        a--;
        enemys.Remove(enemyObj);
    }
    public void EnemygakgakEnd()
    {
        a++;
        if (enemys.Count > a)
        {
            
            
        }
        else
        {
            turnManager.EnemyTurnEnd();

        }
    }
}
