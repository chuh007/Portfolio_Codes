using _01Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace _01Scripts.Players
{
    public class PlayerBattleCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private ParticleSystem blockParticle;
        [SerializeField] private ParticleSystem blockEffectParticle;
        [SerializeField] private ParticleSystem healEffectParticle;
        [SerializeField] private ParticleSystem attackUpEffectParticle;
        
        public void Initialize(Entity entity)
        {
            blockParticle.gameObject.SetActive(false);
            blockEffectParticle.gameObject.SetActive(false);
            healEffectParticle.gameObject.SetActive(false);
            attackUpEffectParticle.gameObject.SetActive(false);
        }

        public void BlockStart()
        {
            blockParticle.gameObject.SetActive(true);
            blockParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            blockParticle.Play();
        }

        public void BlockEffect()
        {
            blockEffectParticle.gameObject.SetActive(true);
            blockEffectParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            blockEffectParticle.Play();
        }

        public void PlayHealEffect()
        {
            healEffectParticle.gameObject.SetActive(true);
            healEffectParticle.Play();
            DOVirtual.DelayedCall(0.5f, () => healEffectParticle.Stop());
        }

        public void AttackUpEffect()
        {
            attackUpEffectParticle.gameObject.SetActive(true);
            attackUpEffectParticle.Play();
            DOVirtual.DelayedCall(0.5f, () => attackUpEffectParticle.Stop());
        }
        
        public void BlockEnd()
        {
            blockParticle.gameObject.SetActive(false);
        }
    }
}