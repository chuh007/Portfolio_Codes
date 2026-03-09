using System.Collections.Generic;
using System.Linq;
using _01Scripts.Core.EventSystem;
using _01Scripts.Players;
using Chuh007Lib.Dependencies;
using DG.Tweening;
using UnityEngine;

namespace _01Scripts.TurnSystem
{
    [Provide]
    public class TurnManager : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO turnEventChannel;
        [SerializeField] private Player player;

        private List<ITurnActor> _turnActors = new();
        public Queue<ITurnActor> ActionOrders = new();
        private bool _isBattleEnd = false;
        
        private void Awake()
        {
            uiChannel.AddListener<FadeCompleteEvent>(HandleFadeCompleteEvent);
            turnEventChannel.AddListener<TurnEndEvent>(HandleTurnEndEvent);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleFadeCompleteEvent);
            turnEventChannel.RemoveListener<TurnEndEvent>(HandleTurnEndEvent);
        }

        private void HandleFadeCompleteEvent(FadeCompleteEvent obj)
        {
            _turnActors = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITurnActor>()
                .OrderByDescending(actor => actor.Speed)
                .ToList();
            SelectNextTurn();
        }
        
        private void HandleTurnEndEvent(TurnEndEvent obj)
        {
            SelectNextTurn();
        }

        private void SelectNextTurn()
        {
            if (ActionOrders.Count <= 5) FillTurnQueue();
            DOVirtual.DelayedCall(0.15f, () =>
            {
                var actor = ActionOrders.Dequeue();
                var evt = TurnEvents.TurnStartEvent;
                turnEventChannel.RaiseEvent(evt);
                actor.TurnAction();
            });
        }

        private void FillTurnQueue()
        {
            int threshold = _turnActors[0].Speed;
            while (ActionOrders.Count < 10)
            {
                var readyActors = _turnActors
                    .Where(a => a.ActionValue >= threshold)
                    .ToList();

                if (readyActors.Count == 0)
                {
                    foreach (var actor in _turnActors)
                        actor.ActionValue += actor.Speed;

                    continue;
                }

                var executionActor = readyActors
                    .OrderByDescending(a => a.ActionValue)
                    .ThenByDescending(a => a.Speed)
                    .First();

                executionActor.ActionValue -= threshold;
                ActionOrders.Enqueue(executionActor);
            }
        }
    }
}