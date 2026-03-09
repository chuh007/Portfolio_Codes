using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer image;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text eFTMP;
    [SerializeField] TMP_Text costTMP;


    public Item card;
    public PRS orignPRS;

    public void SetUp(Item card)
    {
        if (card == null)
            return;
        this.card = card;
        image.sprite = this.card.image;
        nameTMP.text = this.card.cardName;
        eFTMP.text = this.card.cardText;
        costTMP.text = this.card.cost.ToString();
    }

    public void MoveTransfrom(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.position = prs.scale;
        }
    }
}
