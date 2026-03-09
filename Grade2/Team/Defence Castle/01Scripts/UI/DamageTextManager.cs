using System;
using Chipmunk.GameEvents;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH._01Scripts.Combat;
using Work.CHUH._01Scripts.Event;
using Random = UnityEngine.Random;

namespace Work.CHUH._01Scripts.UI
{
    public class DamageTextManager : MonoBehaviour
    {
        [SerializeField] private PoolManagerSO poolManagerSO;
        [SerializeField] private PoolItemSO damageTextSO;

        public bool ShowDamageText;
        
        private void Awake()
        {
            EventBus<EntityHitEvent>.OnEvent += HandleEnemyHit;
        }
        
        private void HandleEnemyHit(EntityHitEvent evt)
        {
            if (!ShowDamageText) return;
             DamageText text = poolManagerSO.Pop(damageTextSO) as DamageText;
             Debug.Assert(text != null, $"pool error : Cannot Pop DamageText Using {damageTextSO}");
             text.transform.position = evt.pos + Vector3.up * 1.25f + (Vector3)Random.insideUnitCircle * 0.25f;
             text.SetText(evt.damage);
        }
    }
}