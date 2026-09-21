using System;
using _Work.CHUH.Code.Core;
using _Work.CHUH.Code.Core.Events;
using Chuh007Lib.Bus;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Combat.DamageText
{
    public class DamageTextManager : MonoBehaviour
    {
        [SerializeField] private PoolManagerSO poolManagerSO;
        [SerializeField] private PoolItemSO damageTextSO;
        [SerializeField] private float scatterRadius = 0.5f;
        [SerializeField, Min(0)] private int maxVisibleTexts = 100;
        [SerializeField] private DamageTextGradient neutralGradient = new DamageTextGradient(
            Color.white,
            new Color(0.43f, 0.43f, 0.43f, 1f));
        [SerializeField] private DamageTextGradient advantageGradient = new DamageTextGradient(
            new Color(1f, 1f, 0.75f, 1f),
            new Color(1f, 0.75f, 0.1f, 1f));
        [SerializeField] private DamageTextGradient disadvantageGradient = new DamageTextGradient(
            new Color(0.55f, 0.55f, 0.55f, 1f),
            new Color(0.18f, 0.18f, 0.18f, 1f));
        [SerializeField, Min(0f)] private float neutralScale = 1f;
        [SerializeField, Min(0f)] private float advantageScale = 1f;
        [SerializeField, Min(0f)] private float disadvantageScale = 0.75f;

        public bool ShowDamageText;

        private int _activeTextCount;
        
        private void Awake()
        {
            Bus<EnemyHitEvent>.OnEvent += HandleEnemyHit;
        }

        private void OnDestroy()
        {
            Bus<EnemyHitEvent>.OnEvent -= HandleEnemyHit;
        }

        private void HandleEnemyHit(EnemyHitEvent evt)
        {
            if (!ShowDamageText) return;
            // Keep the gameplay random sequence unchanged when a visual is skipped.
            Vector3 offset = (Vector3)Random.insideUnitCircle * scatterRadius;
            if (_activeTextCount >= maxVisibleTexts) return;

            DamageText text = poolManagerSO.Pop(damageTextSO) as DamageText;
            if (text == null)
            {
                Debug.LogError($"pool error : Cannot Pop DamageText Using {damageTextSO}", this);
                return;
            }

            text.TrackDisplay(this);
            _activeTextCount++;
            text.transform.position = evt.Pos + offset;
            GetTextStyle(evt.DamageTypeMultiplier, out DamageTextGradient gradient, out float scale);
            text.SetText(evt.Damage, gradient, scale);
        }

        internal void ReleaseText()
        {
            _activeTextCount--;
        }

        private void GetTextStyle(float damageTypeMultiplier, out DamageTextGradient gradient, out float scale)
        {
            if (damageTypeMultiplier > 1f)
            {
                gradient = advantageGradient;
                scale = advantageScale;
                return;
            }

            if (damageTypeMultiplier < 1f)
            {
                gradient = disadvantageGradient;
                scale = disadvantageScale;
                return;
            }

            gradient = neutralGradient;
            scale = neutralScale;
        }
    }
}
