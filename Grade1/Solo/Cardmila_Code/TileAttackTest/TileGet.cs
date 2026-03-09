using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGet : MonoBehaviour
{
    [SerializeField] private GameObject wow1;
    [SerializeField] private GameObject wow2;
    [SerializeField] EnemyStat enemyStatWow;
    [SerializeField] EnemyStat enemyStatWow2;
    [SerializeField] BossStat bossStatWow;
    [SerializeField] BossStat bossStatWow2;
    [SerializeField] private PlayerStat playerStat;
    CardManager cardm;
    private CostTextSet costText;
    private void Awake()
    {
        cardm = CardManager.Inst;
        playerStat = FindObjectOfType<PlayerStat>();
        costText = FindObjectOfType<CostTextSet>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (wow1 != null)
            {
                wow2 = collision.gameObject;
            }
            else
            {
                wow1 = collision.gameObject;
            }

            if (collision.GetComponent<EnemyStat>() != null && enemyStatWow != null)
            {
                enemyStatWow2 = wow2.GetComponent<EnemyStat>();
            }
            if (collision.GetComponentInParent<BossStat>() != null && enemyStatWow != null)
            {
                bossStatWow2 = wow2.GetComponentInParent<BossStat>();
            }
            if (collision.GetComponent<EnemyStat>() != null)
            {
                enemyStatWow = wow1.GetComponent<EnemyStat>();
            }
            if (collision.GetComponentInParent<BossStat>() != null)
            {
                bossStatWow = wow1.GetComponentInParent<BossStat>();
            }
            

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(wow1 == null)
        {
            wow2 = null;
            enemyStatWow2 = null;
            bossStatWow2 = null;
        }
        wow1 = null;
        enemyStatWow = null;
        bossStatWow = null;
    }
    public int HitThisTile(GameObject wamma)
    {
        if (wow1==null&&wow2==null) return 1;
        UICardScr a = wamma.GetComponent<UICardScr>();
        cardm.UseSuccessCard(a);
        playerStat.PlayerShield += a.card.defence;
        playerStat.PlayerCost -= a.card.cost;
        playerStat.PlayerCost += a.card.CureCostCount;
        if (enemyStatWow != null)
        {
            enemyStatWow.EnemyHP -= a.card.attack;
        }
        if(enemyStatWow == null&& enemyStatWow2 != null)
        {
            enemyStatWow.EnemyHP-=a.card.attack;
        }
        if (bossStatWow != null)
        {
            bossStatWow.EnemyHP -= a.card.attack;
        }
        if (bossStatWow == null&&bossStatWow2!=null)
        {
            bossStatWow2.EnemyHP -= a.card.attack;
        }
        costText.SetText(playerStat.PlayerCost,playerStat.PlayerMaxCost);
        
        return 0;
    }
}
