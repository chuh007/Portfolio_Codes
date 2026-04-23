using _01Scripts.Core.EventSystem;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _01Scripts.Combat.Feedback
{
    public class HitImpactFeedback : Feedback
    {
        [SerializeField] private PoolItemSO impactPoolType;
        [SerializeField] private EntityFeedbackData feedbackData;
        [SerializeField, ColorUsage(true, true)] private Color impactColor;
        [SerializeField] private Vector3 effectScale = new Vector3(2f, 2f, 2f);
        [SerializeField] private GameEventChannelSO spawnChannel;
        
        public override void CreateFeedback()
        {
            var evt = SpawnEvents.SpawnAnimationEffect;
            Vector2 direction = (feedbackData.LastEntityWhoHit.transform.position - transform.position).normalized;
            float zRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion towardRotation = Quaternion.LookRotation(direction, Vector3.up);
            evt.Initializer(impactPoolType, transform.position + (Vector3)(Random.insideUnitCircle * 0.5f) + Vector3.up, towardRotation, effectScale, impactColor);
            spawnChannel.RaiseEvent(evt);
        }
    }
}