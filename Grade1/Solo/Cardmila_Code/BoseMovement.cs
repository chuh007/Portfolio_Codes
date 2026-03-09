using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoseMovement : MonoBehaviour
{
    public GameObject pl;
    TurnManager turnManager;
    EnemyManager enemyManager;
    [SerializeField] int maxCanMoveCount = 5;
    [SerializeField] Vector2 enemyGamJiRange;
    private int nowMoveCount = 0;
    private bool canAttack = false;
    private BossStat bossStat;
    [SerializeField] BoseAnimator bossani;
    [SerializeField] SpriteRenderer spr;
    private void Awake()
    {
        enemyManager = EnemyManager.Instance;
        turnManager = TurnManager.Instance;
        turnManager.EnemyTurnStartEvent += BoseTurnStartEvent;
        bossStat = GetComponent<BossStat>();
        enemyManager.PushEnemy(gameObject);
    }
    
    private void BoseTurnStartEvent()
    {
        StartCoroutine(BossActive());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
    private void Update()
    {
        Collider2D[] col = Physics2D.OverlapBoxAll(transform.position, enemyGamJiRange, 0);
        foreach (Collider2D col2d in col)
        {
            if (col2d.gameObject.CompareTag("Player"))
            {
                canAttack = true;
                break;
            }
            else
            {
                canAttack = false;
            }
        }
    }
    private IEnumerator BossActive()
    {
        nowMoveCount = 0;
        while (nowMoveCount < maxCanMoveCount)
        {
            spr.flipX = pl.transform.position.x < transform.position.x;
            bool ismove = false;
            if (canAttack)
            {
                PlayerStat wow = pl.GetComponent<PlayerStat>();
                bossani.AttackAni();
                wow.PlayerShield -= bossStat.enemyAttack;
                Debug.Log(wow.PlayerHp);
                Debug.Log(wow.PlayerShield);
                break;
            }
            if (pl.transform.position.x > transform.position.x && ismove == false)
            {
                bool canmove = true;
                Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + 1, transform.position.y), new Vector2(1, 1), 0);
                foreach (Collider2D col2d in col)
                {
                    if (col2d.gameObject.CompareTag("Player"))
                    {
                        canmove = false;
                        break;
                    }
                    else
                    {
                        canmove = true;
                    }
                }
                if (canmove)
                {
                    bossani.MoveAnim();
                    transform.DOMoveX(transform.position.x + 1, 0.25f);
                    ismove = true;
                }


            }
            if (pl.transform.position.x < transform.position.x && ismove == false)
            {
                bool canmove = true;
                Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x - 1, transform.position.y), new Vector2(1, 1), 0);
                foreach (Collider2D col2d in col)
                {
                    if (col2d.gameObject.CompareTag("Player"))
                    {
                        canmove = false;
                        break;
                    }
                    else
                    {
                        canmove = true;
                    }
                }
                if (canmove)
                {
                    bossani.MoveAnim();
                    transform.DOMoveX(transform.position.x - 1, 0.25f);
                    ismove = true;
                }
            }
            if (pl.transform.position.y > transform.position.y && ismove == false)
            {
                bool canmove = true;
                Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y + 1), new Vector2(1, 1), 0);
                foreach (Collider2D col2d in col)
                {
                    if (col2d.gameObject.CompareTag("Player"))
                    {
                        canmove = false;
                        break;
                    }
                    else
                    {
                        canmove = true;
                    }
                }
                if (canmove)
                {
                    bossani.MoveAnim();
                    transform.DOMoveY(transform.position.y + 1, 0.25f);
                    ismove = true;
                }
            }
            if (pl.transform.position.y < transform.position.y && ismove == false)
            {
                bool canmove = true;
                Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y + 1), new Vector2(1, 1), 0);
                foreach (Collider2D col2d in col)
                {
                    if (col2d.gameObject.CompareTag("Player"))
                    {
                        canmove = false;
                        break;
                    }
                    else
                    {
                        canmove = true;
                    }
                }
                if (canmove)
                {
                    bossani.MoveAnim();
                    transform.DOMoveY(transform.position.y - 1, 0.25f);
                    ismove = true;
                }
            }
            nowMoveCount++;
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("나는야 에너미 무브 반복이 종료되다.");
        enemyManager.EnemygakgakEnd();
    }
    
}
