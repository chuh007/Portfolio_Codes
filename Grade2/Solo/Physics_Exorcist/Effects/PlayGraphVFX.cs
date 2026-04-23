using System;
using Assets.Bocch16Lib.ObjectPool.RunTime;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.VFX;

namespace Blade.Effects
{
    public class PlayGraphVFX : MonoBehaviour, IPlayableVFX
    {
        [field: SerializeField] public string VFXName { get; private set; }
        [SerializeField] private bool isOnPosition;
        [SerializeField] private VisualEffect[] effects;
        
        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            if(isOnPosition == false)
                transform.SetPositionAndRotation(position, rotation);
            
            foreach(VisualEffect effect in effects)
                effect.Play();
                
        }

        public void StopVFX()
        {
            foreach(VisualEffect effect in effects)
                effect.Stop();
        }

        private void OnValidate()
        {
            if(string.IsNullOrEmpty(VFXName) == false)
                gameObject.name = VFXName;
        }
    }
}