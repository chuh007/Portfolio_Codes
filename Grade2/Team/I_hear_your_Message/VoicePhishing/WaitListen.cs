using Ami.BroAudio;
using UnityEngine;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class WaitListen : Call
    {
        public void PlaySound()
        {
            BroAudio.Play(_audioId);
        }
    }
}