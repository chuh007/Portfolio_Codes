using _Work.CHUH.Code.EntityPlus.Effect.EffectData;
using Chuh007Lib.Entities.Entities;

namespace _Work.CHUH.Code.EntityPlus.Effect
{
    public interface IEffectHandler
    {
        void AddEffect(AbstractEffectDataSO effectData);
        void AddEffect(AbstractEffectDataSO effectData, Entity source);
    }
}