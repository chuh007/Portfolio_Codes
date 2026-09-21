using _Work.CHUH.Code.EntityPlus.Effect;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    [CreateAssetMenu(fileName = "StunEffect", menuName = "SO/Effect/StunEffect")]
    public class StunEffectSO : AbstractEffectDataSO, IDurationEffect
    {
        public static void ApplyTo(Collider2D target, float duration)
        {
            if (target == null || duration <= 0f) return;

            Entity entity = target.GetComponent<Entity>() ?? target.GetComponentInParent<Entity>();
            ApplyTo(entity, duration);
        }

        public static void ApplyTo(Entity target, float duration)
        {
            if (target == null || target.IsDead || duration <= 0f) return;

            target.AddEffect(CreateRuntime(duration));
        }

        public void ActiveEffect(Entity target)
        {
            if (target == null || target.IsDead) return;

            var runtime = target.GetComponent<StunEffectRuntime>();
            if (runtime == null)
                runtime = target.gameObject.AddComponent<StunEffectRuntime>();

            runtime.BeginStun();
        }

        public void UnActiveEffect(Entity target)
        {
            if (target != null && target.TryGetComponent<StunEffectRuntime>(out var runtime))
                runtime.EndStun();

            if ((hideFlags & HideFlags.DontSave) != 0)
                Destroy(this);
        }

        private static StunEffectSO CreateRuntime(float duration)
        {
            var effect = CreateInstance<StunEffectSO>();
            effect.effectName = "Stun";
            effect.duration = Mathf.Max(0.01f, duration);
            effect.stackPolicy = StackPolicy.Independent;
            effect.maxStack = 1;
            effect.hideFlags = HideFlags.DontSave;
            return effect;
        }
    }
}
