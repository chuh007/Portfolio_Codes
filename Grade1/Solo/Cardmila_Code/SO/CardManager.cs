using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst {  get; private set; }
    private void Awake() =>Inst = this;

    public List<Item> deckList;
    [SerializeField] private ItemSO ItemSO;
    [SerializeField] private List<UICardScr> handCard;
    [SerializeField] private List<UICardScr> deathCard;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardMother;
    [SerializeField] private RectTransform cardLeft;
    [SerializeField] private RectTransform cardRight;
    [SerializeField] private RectTransform doMovePos;
    public static bool iscatch=false;
    private PlayerStat playerStat;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI falledTxt;
    

    public Item DrowCard()
    {
        Item card = deckList[0];
        deckList.RemoveAt(0);
        return card;
    }
    void SetupCardBuffer()
    {
        deckList = new List<Item>();
        for(int i = 0; i< ItemSO.item.Length;i++)
        {
            Item card = ItemSO.item[i];
            for (int j = 0; j < card.count; j++)
            {
                deckList.Add(card);
            }
        }
        for (int i = 0; i < deckList.Count; i++)
        {
            int rand = Random.Range(i,deckList.Count);
            Item temp = deckList[i];
            deckList[i] = deckList[rand];
            deckList[rand] = temp;
        }
    }
    private void Start()
    {
        SetupCardBuffer();
        cardMother = GameObject.Find("CardMother").transform;
    }
    public void CardReRord()
    {
        playerStat = PlayerStat.Instance;
        if(playerStat.PlayerCost < playerStat.PlayerMaxCost )
        {
            StartCoroutine(Wow());
            return;
        }
        playerStat.PlayerCost -= playerStat.PlayerMaxCost;
        costText.text = playerStat.PlayerCost+"/"+playerStat.PlayerMaxCost;
        for (int i = 0; i < deathCard.Count;)
        {
            Item a = deathCard[0].card;
            deckList.Add(a);
            Destroy(deathCard[0].gameObject);
            deathCard.Remove(deathCard[0]);
            
        }
        for (int i = 0; i < deckList.Count; i++)
        {
            int rand = Random.Range(i, deckList.Count);
            Item temp = deckList[i];
            deckList[i] = deckList[rand];
            deckList[rand] = temp;
        }
    }
    private IEnumerator Wow()
    {
        falledTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        falledTxt.gameObject.SetActive(false);
    }
    public void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    AddCard();
        //}
    }
    public void AddCard()
    {
        if (deckList.Count == 0)
        {
            Debug.Log("덱이 없는 레후");
            return;
        }
        var cardObj = Instantiate(cardPrefab, cardMother);
        var card = cardObj.GetComponent<UICardScr>();
        card.order = handCard.Count;
        card.SetUp(DrowCard());
        handCard.Add(card);
        CardAlignment();
    }
    public void UseSuccessCard(UICardScr card)
    {
        handCard.Remove(card);
        deathCard.Add(card);
        card._cardLine.EndLine();
        iscatch = false;
        CardAlignment();
        PRS w = new PRS(doMovePos.position, Quaternion.identity,new Vector3(1,1,1));
        card.MoveTransform(w, true,0.5f);
    }
    


    void CardAlignment()
    {
        List<PRS> orignCardPRSs = new List<PRS>();
        orignCardPRSs = RoundCard(cardLeft, cardRight, handCard.Count, 0.5f);

        var targetCards = handCard;
        for (int i = 0; i < handCard.Count; i++)
        {
            var targetCard = targetCards[i];
            targetCard.OriginPRS = orignCardPRSs[i];
            targetCard.MoveTransform(targetCard.OriginPRS, true, 0.7f);
        }
    }
    List<PRS> RoundCard(RectTransform leftRe, RectTransform rightRe, int cardCount, float h)
    {
        float[] cardLerps = new float[cardCount];
        List<PRS> result = new List<PRS>(cardCount);
        switch (cardCount)
        {
            case 1:
                cardLerps = new float[] { 0.5f };
                break;
            case 2:
                cardLerps = new float[] { 0.27f, 0.73f };
                break;
            case 3:
                cardLerps = new float[] { 0.1f, 0.5f, 0.9f };
                break;
            default:
                float interval = 1f / (cardCount - 1);
                for (int i = 0; i < cardCount; i++)
                {
                    cardLerps[i] = interval * i;
                }
                break;
        }
        for (int i = 0; i < cardCount; i++)
        {
            

            var targetpos = Vector3.Lerp(leftRe.anchoredPosition, rightRe.anchoredPosition, cardLerps[i]);
            var targetrot = Quaternion.identity;
            float curve = Mathf.Sqrt(Mathf.Pow(h, 2) - Mathf.Pow(cardLerps[i] - 0.5f, 2));
            targetpos.y += curve;
            targetrot = Quaternion.Slerp(leftRe.rotation, rightRe.rotation, cardLerps[i]);
            result.Add(new PRS(targetpos, targetrot, new Vector3(1, 1, 1)));
        }
        return result;
    }
    public void ReroedScene()
    {
        deathCard.Clear();
        handCard.Clear();
        deckList.Clear();
        for (int i = 0; i < ItemSO.item.Length; i++)
        {
            Item card = ItemSO.item[i];
            for (int j = 0; j < card.count; j++)
            {
                deckList.Add(card);
            }
        }
        for (int i = 0; i < deckList.Count; i++)
        {
            int rand = Random.Range(i, deckList.Count);
            Item temp = deckList[i];
            deckList[i] = deckList[rand];
            deckList[rand] = temp;
        }
    }
}
