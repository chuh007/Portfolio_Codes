using System;
using Chipmunk.GameEvents;
using UnityEngine;
using Work.CHUH._01Scripts.Enemies.Wave.GameEvents;
using Work.CHUH._01Scripts.Event;

namespace Work.CHUH._01Scripts.Resources
{
    public class ResourcesController : MonoBehaviour
    {
        [SerializeField] private CostDataSO goldData;
        [SerializeField] private CostDataSO gemData;
        [SerializeField] private int waveAwardGem = 5;

        [SerializeField]
        private void Awake()
        {
            EventBus<WaveEndEvent>.OnEvent += HandleWaveEnd;
            EventBus<EnemyDeadEvent>.OnEvent += HandleEnemyDead;
        }

        private void HandleWaveEnd(WaveEndEvent evt)
        {
            if (evt.EndReason == WaveEndEvent.Reason.AllEnemiesDefeated)
            {
                gemData.Value.Value += waveAwardGem;
            }
        }

        private void HandleEnemyDead(EnemyDeadEvent evt)
        {
            goldData.Value.Value += evt.gold;
        }
    }
}