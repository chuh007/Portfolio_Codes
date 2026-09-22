using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    public abstract class AbstractEffectDataSO : ScriptableObject
    {
        public string effectId;     // 같은 이펙트 비교용 (SO 인스턴스 달라도 같은 효과면 같은 id)
        public string effectName;   // 표시용 이름
        public float duration = 10;
        [TextArea] public string effectDescription;

        // Stack 설정
        public StackPolicy stackPolicy = StackPolicy.Refresh;
        public int maxStack = 1;
    }

    public enum StackPolicy
    {
        Independent, // 따로 쌓임 (기존 동작)
        Refresh,     // Duration만 갱신
        Stack,       // Stack 증가 + Duration 갱신
        Ignore       // 이미 있으면 무시
    }
}