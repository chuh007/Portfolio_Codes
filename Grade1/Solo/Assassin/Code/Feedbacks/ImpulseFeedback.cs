using Code.Feedbacks;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Feedbacks
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class ImpulseFeedback : Feedback
    {
        [SerializeField] private float _impulseForce = 0;
        private CinemachineImpulseSource _source;

        private void Awake()
        {
            _source = GetComponent<CinemachineImpulseSource>();
        }
        public override void CreateFeedback()
        {
            _source.GenerateImpulse(_impulseForce);
        }
    }
}

