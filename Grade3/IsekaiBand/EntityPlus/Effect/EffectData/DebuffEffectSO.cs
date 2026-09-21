using _Code.LCH._02.Scripts.Card.Build;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    [CreateAssetMenu(fileName = "DebuffEffect", menuName = "SO/Effect/DebuffEffect")]
    public class DebuffEffectSO : AbstractEffectDataSO
    {
        public DebuffType type;
        public float value;
        public float tickDamage;
        public float tickInterval = 1f;
        public bool canSpread;
        public bool isHardCC;

        public override EntityEffect CreateEffect(Entity target, Entity source = null)
        {
            return new DebuffEffect(this, target, source);
        }

        public static DebuffEffectSO CreateRuntime(DebuffType type, float duration, float value)
        {
            var effect = CreateInstance<DebuffEffectSO>();
            effect.type = type;
            effect.effectId = $"Debuff.{type}";
            effect.effectName = type.ToString();
            effect.duration = duration;
            effect.value = value;
            effect.isHardCC = type == DebuffType.Stun;
            effect.stackPolicy = StackPolicy.Stack;
            effect.maxStack = 1;
            effect.hideFlags = HideFlags.HideAndDontSave;
            return effect;
        }
    }
}
