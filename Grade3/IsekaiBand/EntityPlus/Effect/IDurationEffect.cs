using Chuh007Lib.Entities.Entities;

namespace _Work.CHUH.Code.EntityPlus.Effect
{
    // 스텟 적용하는 등의 들어오면 작동하고, 시간 끝나면 끄는 그런 식
    public interface IDurationEffect
    {
        public void ActiveEffect(Entity target);
        public void UnActiveEffect(Entity target);
    }
}