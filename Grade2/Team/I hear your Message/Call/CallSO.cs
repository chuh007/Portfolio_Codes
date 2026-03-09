using UnityEngine;

namespace A.Work.CHUH._01.Script.Call
{
    [CreateAssetMenu(fileName = "Call", menuName = "SO/Call/CallData", order = 0)]
    public class CallSO : ScriptableObject
    {
        public string phoneNumber;
        public Sprite icon;
    }
}