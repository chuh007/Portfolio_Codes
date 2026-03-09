using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public GameObject pl;
    private SpriteRenderer spr;
    private Animator animator;
    TurnManager turnManager;
    EnemyManager enemyManager;
    [SerializeField] int maxCanMoveCount = 3;
    [SerializeField] Vector2 enemyGamJiRange;
    private int nowMoveCount = 0;
    private bool canAttack = false;
    private EnemyStat enemyStat;
    private BulletPull bulletPull;

    private void Awake()
    {
        bulletPull = BulletPull.Instance;
        spr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyManager = EnemyManager.Instance;
        turnManager = TurnManager.Instance;
        turnManager.EnemyTurnStartEvent += EnemyTurnStartEvent;
        enemyStat = GetComponent<EnemyStat>();

    }
    private void OnEnable()
    {
        pl = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnDestroy()
    {
        turnManager.EnemyTurnStartEvent -= EnemyTurnStartEvent;
    }
    private void Update()
    {
       
        Collider2D [] col = Physics2D.OverlapBoxAll(transform.position, enemyGamJiRange,0);
        foreach(Collider2D col2d in col)
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
    private void EnemyTurnStartEvent()
    {
        
        StartCoroutine(EnemyMoveEvent());
    }
    private IEnumerator EnemyMoveEvent()
    {
        nowMoveCount = 0;
        while (nowMoveCount<maxCanMoveCount)
        {
            bool ismove = false;
            spr.flipX = pl.transform.position.x < transform.position.x;
            if (canAttack)
            {
                PlayerStat wow = pl.GetComponent<PlayerStat>();
                wow.PlayerShield -= enemyStat.enemyAttack;
                Debug.Log(wow.PlayerHp);
                Debug.Log(wow.PlayerShield);
                GameObject g = bulletPull.GetBulletInPool(transform,pl.transform);
                Bullet bullet = g.GetComponent<Bullet>();
                bullet.DoMovePl(pl.transform);
                //transform.DOMove(pl.transform.position, 0.5f).SetLoops(2, LoopType.Yoyo);
                break;
            }
            if (pl.transform.position.x > transform.position.x &&ismove == false)
            {
                bool canmove = true;
                Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x+1,transform.position.y), new Vector2(1,1), 0);
                foreach (Collider2D col2d in col)
                {
                    if (col2d.gameObject.CompareTag("Player")||col2d.gameObject.CompareTag("Enemy"))
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
                    animator.SetBool("Ismove", true);
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
                    if (col2d.gameObject.CompareTag("Player") || col2d.gameObject.CompareTag("Enemy"))
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
                    animator.SetBool("Ismove", true);
                    transform.DOMoveX(transform.position.x - 1, 0.25f);
                    ismove = true;
                }
            }
            if (pl.transform.position.y > transform.position.y && ismove == false)
            {
                bool canmove = true;
                Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y+1), new Vector2(1, 1), 0);
                foreach (Collider2D col2d in col)
                {
                    if (col2d.gameObject.CompareTag("Player") || col2d.gameObject.CompareTag("Enemy"))
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
                    animator.SetBool("Ismove", true);
                    transform.DOMoveY(transform.position.y + 1, 0.25f);
                    ismove = true;
                }
            }
            if (pl.transform.position.y < transform.position.y && ismove == false)
            {
                bool canmove = true;
                Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y+1), new Vector2(1, 1), 0);
                foreach (Collider2D col2d in col)
                {
                    if (col2d.gameObject.CompareTag("Player") || col2d.gameObject.CompareTag("Enemy"))
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
                    animator.SetBool("Ismove", true);
                    transform.DOMoveY(transform.position.y - 1, 0.25f);
                    ismove = true;
                }
            }
            nowMoveCount++;
            yield return new WaitForSeconds(0.25f);
            animator.SetBool("Ismove", false);
            yield return new WaitForSeconds(0.25f);
        }
        //Debug.Log("나는야 에너미 무브 반복이 종료되다.");
        enemyManager.EnemygakgakEnd();

    }

}
