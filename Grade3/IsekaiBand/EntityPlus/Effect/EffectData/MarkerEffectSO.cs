using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    /// <summary>
    /// 상태 마커 전용 이펙트. 행동 없이 존재 여부(스택)만 추적.
    /// 침묵, 방어 분쇄 등 EntityEffectController.HasEffect / GetStackCount 로 참조.
    /// </summary>
    [CreateAssetMenu(fileName = "MarkerEffect", menuName = "SO/Effect/MarkerEffect")]
    public class MarkerEffectSO : AbstractEffectDataSO { }
}
