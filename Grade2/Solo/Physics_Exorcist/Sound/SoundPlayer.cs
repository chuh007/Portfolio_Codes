using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01Scripts.Sound
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip currentBGM;
        
        private void Start()
        {
            SoundManager.Instance.PlayBGM(currentBGM);
        }

        public void PlaySound(AudioClip clip)
        {
            SoundManager.Instance.PlaySFX(clip);
        }
    }
}