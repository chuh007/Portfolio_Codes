using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class BonusCardGet : MonoBehaviour, IPointerClickHandler
{
    public Item[] item;
    public ItemSO itemSO;
    private Item choseCard;
    [SerializeField] Image skillImagewow;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI eFText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] LevelManager levelManager;
    private void Awake()
    {
        item = itemSO.item;
    }
    public void SetCard()
    {
        int a = Random.Range(0, item.Length);
        choseCard = item[a];
        skillImagewow.sprite = item[a].image;
        nameText.text = item[a].cardName;
        eFText.text = item[a].cardText;
        costText.text = item[a].cost.ToString();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        CardManager.Inst.deckList.Add(choseCard);
        levelManager.EndLevelUI();
    }
}
