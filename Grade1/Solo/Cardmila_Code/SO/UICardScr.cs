using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UICardScr : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    //[SerializeField] Image cardImage;
    [SerializeField] Image skillImagewow;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI eFText;
    [SerializeField] TextMeshProUGUI costText;
    RectTransform rectTransform;
    PlayerStat PlayerStat;

    public Item card;
    //public PRS OrignPRS;
    private PRS _originPRS;
    private PRS _savePRS;
    public PRS OriginPRS
    {
        get => _originPRS;
        set => _originPRS = value;
    }
    public CardLine _cardLine;
    private TileAttackWow _tileAttackWow;
    private void Awake()
    {
        PlayerStat = PlayerStat.Instance;
        _tileAttackWow = FindObjectOfType<TileAttackWow>();
        _cardLine = GetComponentInChildren<CardLine>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void SetUp(Item card)
    {
        if (card == null)
            return;
        this.card = card;
        skillImagewow.sprite = this.card.image;
        //cardImage.sprite = this.card.sprite;
        nameText.text = this.card.cardName;
        eFText.text = this.card.cardText;
        costText.text = this.card.cost.ToString();
        
    }
    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        _savePRS = prs;
        if (useDotween)
        {
            rectTransform.DOMove(prs.pos, dotweenTime);
            rectTransform.DORotateQuaternion(prs.rot, dotweenTime);
        }
        else
        {
            rectTransform.position = prs.pos;
            rectTransform.rotation = prs.rot;
            rectTransform.position = prs.scale;
        }
    }



    public int order;
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CancelUse();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button ==PointerEventData.InputButton.Left&&CardManager.iscatch==false && MovecardUI.isSeeArrow==false)
        {
            if (PlayerStat.PlayerCost >= card.cost)
            {
                _tileAttackWow.CardAttackUse(gameObject);
                if (CardManager.iscatch) return;
                CardManager.iscatch = true;
                _cardLine.CardLineWow(rectTransform);

            }

        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardManager.iscatch == false)
        {
            rectTransform.position += new Vector3(0, 50, 0);
            rectTransform.rotation = Quaternion.identity;
            rectTransform.localScale = new Vector3(1.5f, 1.5f, 1);
            transform.SetAsLastSibling();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardManager.iscatch) return;
        rectTransform.position = _savePRS.pos;
        rectTransform.rotation = _savePRS.rot;
        rectTransform.localScale = Vector3.one;
        transform.SetSiblingIndex(order);
    }
    public void CancelUse()
    {
        CardManager.iscatch = false;
        _cardLine.EndLine();
        rectTransform.position = _savePRS.pos;
        rectTransform.rotation = _savePRS.rot;
        rectTransform.localScale = Vector3.one;
        transform.SetSiblingIndex(order);
    }
}
