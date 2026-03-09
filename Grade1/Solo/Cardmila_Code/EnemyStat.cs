using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStat : MonoBehaviour
{
    [SerializeField] private int enemyMaxHp = 10;
    [SerializeField] private int enemyHP = 10;
    [SerializeField] private int enemyEXP= 10;
    [SerializeField]private Image hpbar;
    public int enemyAttack = 5;
    EnemyManager enemyManager;
    private EnemyMove enemyMove;
    private GameObject pl;
    private void Awake()
    {
        
        enemyMove = GetComponent<EnemyMove>();
        pl = enemyMove.pl;
        enemyManager = EnemyManager.Instance;
    }
    public int EnemyHP
    {
        get
        {
            return enemyHP;
        }
        set
        {
            enemyHP = value;
            
            if (enemyHP <= 0)
            {
                enemyManager.PopEnemy(this.gameObject);
                Die();
            }
            
            float f = (float)enemyHP / enemyMaxHp;
            hpbar.fillAmount = f;
        }
    }
    private void Die()
    {
        PlayerStat stat = pl.GetComponent<PlayerStat>();
        stat.PlEXPUp(enemyEXP);
        Destroy(gameObject);
    }
}
