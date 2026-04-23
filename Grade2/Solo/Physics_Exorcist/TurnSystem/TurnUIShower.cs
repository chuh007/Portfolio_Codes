using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01Scripts.Core.EventSystem;
using Chuh007Lib.Dependencies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01Scripts.TurnSystem
{
    public class TurnUIShower : MonoBehaviour
    {
        [Header("Temp")] 
        [SerializeField] private List<EntityTurnComponent> turns;
        
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO turnEventChannel;
        [SerializeField] private TextMeshProUGUI[] turnTexts;
        [SerializeField] private Image[] turnImages;
        [Inject] private TurnManager _turnManager;
        
        private Queue<ITurnActor> _turnActors = new();

        private void Awake()
        {
            uiChannel.AddListener<FadeCompleteEvent>(HandleStartSet);
            turnEventChannel.AddListener<TurnEndEvent>(HandleTurnEndEvent);
            turns = turns.OrderBy((actor) => actor.Speed).ToList();
        }


        private void OnDestroy()
        {
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleStartSet);
            turnEventChannel.RemoveListener<TurnEndEvent>(HandleTurnEndEvent);

        }
        
        private void HandleStartSet(FadeCompleteEvent evt)
        {
            StartCoroutine(SetData());
        }

        private IEnumerator SetData()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            _turnActors = new Queue<ITurnActor>(_turnManager.ActionOrders);
            for (int i = 0; i < 5; i++)
            {
                var actor = _turnActors.Dequeue();
                turnTexts[i].text = actor.Name;
                turnImages[i].color = Color.white;
                turnImages[i].sprite = actor.Icon;
            }
        }
        
        private void HandleTurnEndEvent(TurnEndEvent evt)
        {
            _turnActors = new Queue<ITurnActor>(_turnManager.ActionOrders);
            for (int i = 0; i < 5; i++)
            {
                var actor = _turnActors.Dequeue();
                turnTexts[i].text = actor.Name;
                turnImages[i].color = Color.white;
                turnImages[i].sprite = actor.Icon;
            }
        }
    }
}
