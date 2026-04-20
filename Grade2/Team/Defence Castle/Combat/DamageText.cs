using System;
using Chipmunk.GameEvents;
using Chuh007Lib.ObjectPool.RunTime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Work.CHUH._01Scripts.Event;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace Work.CHUH._01Scripts.Combat
{
    public class DamageText : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshPro damageText;
        [SerializeField] private float lifeTime = 1f;
        [SerializeField] private AnimationCurve alphaCurve;
        
        private float _currentLifeTime;
        
        public void SetText(float damage)
        {
            damageText.text = ((int)damage).ToString();
            _currentLifeTime = 0f;
        }

        private void Update()
        {
            _currentLifeTime += Time.deltaTime;
            float normalizedTime = _currentLifeTime / lifeTime;
            
            float y = 1f - Mathf.Pow(1f - normalizedTime, 3);
            
            float alpha = alphaCurve.Evaluate(normalizedTime);
            Color color = damageText.color;
            color.a = alpha;
            damageText.color = color;
            
            damageText.transform.position = transform.position + new Vector3(normalizedTime, y, 0f);
            if (normalizedTime >= 1f)
                _myPool.Push(this);
        }

        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        
        private Pool _myPool;
        
        public void ResetItem()
        {
            damageText.text = string.Empty;
        }
        
        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }
    }
}