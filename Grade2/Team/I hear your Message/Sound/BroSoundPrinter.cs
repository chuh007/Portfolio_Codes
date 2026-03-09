using Ami.BroAudio;
using UnityEngine;

namespace A.Work.CHUH._01.Script.Sound
{
    public class BroSoundPrinter : MonoBehaviour
    {
        [SerializeField] private SoundID sound;

        public void PlaySound()
        {
            BroAudio.Play(sound);
        }
    }
}