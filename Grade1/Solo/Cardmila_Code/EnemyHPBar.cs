using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    public EnemyMove enemyMove;
    private Vector3 enemyTransform;
    private void Update()
    {
        enemyTransform = Camera.main.WorldToScreenPoint(enemyMove.transform.position);
        transform.position = enemyTransform + new Vector3(0, -50, 0);
    }
}
