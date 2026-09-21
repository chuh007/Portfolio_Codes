using _Code.LCH._02.Scripts.Level;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Combat;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Item
{
    public class HealItem : ItemBase
    {
        [SerializeField] private float healAmount = 30f;
        
        protected override void OnCollect()
        {
            var player = ExperienceManager.Instance?.PlayerTransform;
            if (player == null) return;
            
            var health = player.GetComponent<IHealth>();
            if (health == null) return;

            health.TakeHeal(healAmount);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.HealItemPickup,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));
        }
    }
}
