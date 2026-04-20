using A.Work.CHUH._01.Script.Call;
using Ami.BroAudio;
using Scripts.Chatting.ChatSO;
using UnityEngine;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    [CreateAssetMenu(fileName = "VoicePhishing", menuName = "SO/Call/VoicePhishing", order = 0)]
    public class VoicePhishingSO : ScriptableObject
    {
        public SoundID voiceId;
        public bool isFraud;
        public CallSO sender;
        public CallSO recipient;
        [Header("JudgeValue")] 
        public int value;
        public MessageGradeSO gradeSO;
        [TextArea] public string description;
    }
}