using _01Scripts.Entities;
using Unity.Cinemachine;
using UnityEngine;

namespace _01Scripts.Players
{
    public class PlayerCamShaker : MonoBehaviour, IEntityComponent
    {
        private CinemachineImpulseSource _impulseSource;
        
        public void Initialize(Entity entity)
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void ShakeCam()
        {
            _impulseSource.GenerateImpulse();
        }
    }
}