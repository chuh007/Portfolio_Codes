using System.Collections.Generic;
using UnityEngine;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    [CreateAssetMenu(fileName = "PhishingData", menuName = "SO/Call/VoicePhishingDataList", order = 0)]
    public class PhishingDataList : ScriptableObject
    {
        public List<VoicePhishingSO> voicePhishingData;
    }
}