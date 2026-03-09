using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossStat : MonoBehaviour
{
    [SerializeField] private int enemyMaxHp = 10;
    [SerializeField] private int enemyHP = 10;
    [SerializeField] private int enemyEXP = 10;
    [SerializeField] private Image hpbar;
    public int enemyAttack = 5;
    EnemyManager enemyManager;
    private BoseMovement bossMovement;
    private GameObject pl;
    [SerializeField] GameOverManager gameOverManager;
    [SerializeField] private Image gameover;
    private void Awake()
    {

        bossMovement = GetComponent<BoseMovement>();
        pl = bossMovement.pl;
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
        gameover.gameObject.SetActive(true);
        gameOverManager.GameClear();
        Time.timeScale = 0;
    }
}
