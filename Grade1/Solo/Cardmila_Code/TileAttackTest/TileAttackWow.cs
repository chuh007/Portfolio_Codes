using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class TileAttackWow : MonoBehaviour
{
    [SerializeField] CostTextSet costTextSet;
    float maxDis = 20f;
    Vector3 mousePos;
    Camera cam;
    [SerializeField] private int isAttackCardCanUse = 0;
    UICardScr useCard;
    GameObject card;
    [SerializeField] PlayerStat playerStat;
    CardManager cardManager;
    private void Awake()
    {
        cardManager = CardManager.Inst;
        cam = Camera.main;
    }
    public void CardAttackUse(GameObject wow)
    {
        card = wow;
        useCard = wow.GetComponent<UICardScr>();
        if (useCard.card.isattcak)
        {
            isAttackCardCanUse = 1;
        }
        else
        {
            isAttackCardCanUse= 2;
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&&CardManager.iscatch==true)
        {
            if(isAttackCardCanUse == 1)
            {
                //Debug.Log("SS");
                mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, transform.forward, maxDis);
                Debug.DrawRay(mousePos, transform.forward * 10, Color.red, 0.3f);
                if (hit)
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.CompareTag("Tile"))
                    {
                        //Debug.Log("공격 카드임");
                        TileGet um = hit.transform.GetComponent<TileGet>();
                        Debug.Log(um); // 됨
                        int a = um.HitThisTile(card);
                        isAttackCardCanUse = a;
                    }
                }

            }
            if (isAttackCardCanUse == 2)
            {
                mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, transform.forward, maxDis);
                Debug.DrawRay(mousePos, transform.forward * 10, Color.red, 0.3f);
                if (hit)
                {
                    //Debug.Log("레이가맞음");
                    if (hit.transform.CompareTag("Tile"))
                    {
                        //Debug.Log("그것은 공격이 아닌");
                        playerStat.PlayerShield += useCard.card.defence;
                        playerStat.PlayerCost -= useCard.card.cost;
                        playerStat.PlayerCost += useCard.card.CureCostCount;
                        DrowCardUICard.canDrowCount += useCard.card.DrowCount;
                        costTextSet.SetText(playerStat.PlayerCost, playerStat.PlayerMaxCost);
                        isAttackCardCanUse = 0;

                        cardManager.UseSuccessCard(useCard);
                    }
                }
            }
        }
        
    }
}
