using A.Work.CHUH._01.Script.Call;
using Ami.BroAudio;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class Call : MonoBehaviour
    {
        [Header("sender")]
        [SerializeField] protected TextMeshProUGUI senderPhoneText;
        [SerializeField] protected Image senderIcon;
        [Header("recipient")]
        [SerializeField] protected TextMeshProUGUI recipientPhoneText;
        [SerializeField] protected Image recipientIcon;
        [Header("Buttons")]
        [SerializeField] protected Button listenButton;
        [SerializeField] protected Button exitButton;

        protected SoundID _audioId;
        protected bool _isEnd = false;
        
        public void Setup(CallSO senderData, CallSO recipientData, SoundID source)
        {
            senderPhoneText.text = senderData.phoneNumber;
            senderIcon.sprite = senderData.icon;
            recipientPhoneText.text = recipientData.phoneNumber;
            recipientIcon.sprite = recipientData.icon;
            _audioId = source;
        }
        
        public void StopSound()
        {
            _isEnd = true;
            BroAudio.Stop(_audioId);
        }
    }
}