using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpown : MonoBehaviour
{
    [SerializeField] Transform[] array;
    TurnManager turnManager;
    [SerializeField] List<GameObject> Enemys;
    private EnemyManager enemyManager;
    private void Start()
    {
        int a = Random.Range(0, array.Length);
        GameObject g = Instantiate(Enemys[0], array[a].transform);
        enemyManager.PushEnemy(g);
    }
    private void Awake()
    {
        enemyManager = EnemyManager.Instance;
        turnManager = TurnManager.Instance;
        turnManager.EnemyTurnStartEvent += EnemySpownWow;
    }

    private void EnemySpownWow()
    {
        int rE = Random.Range(0,Enemys.Count);
        int a = Random.Range(0, array.Length);
        GameObject g = Instantiate(Enemys[rE], array[a].transform);
        enemyManager.PushEnemy(g);
    }
}
