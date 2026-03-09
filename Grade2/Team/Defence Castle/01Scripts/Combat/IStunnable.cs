using DG.Tweening;

namespace Work.CHUH._01Scripts.Combat
{
    public interface IStunnable
    {
        public bool IsStun { get; }
        public Tween StunTween { get; }
        public float CurrentStunTime { get; }
        public void Stun(float stunTime);
    }
}