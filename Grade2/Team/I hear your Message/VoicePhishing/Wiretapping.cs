using A.Work.CHUH._01.Script.Call;
using Ami.BroAudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class Wiretapping : Call
    {
        public void Play()
        {
            PlayLoop();
        }

        private void PlayLoop()
        {
            var src = BroAudio.Play(_audioId);
            src.OnEnd(_ =>
            {
                if (!_isEnd)
                    PlayLoop();
            });
        }
    }
}