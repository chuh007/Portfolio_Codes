using _Code.LCH._02.Scripts.Level;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Item
{
    public class MagnetItem : ItemBase
    {
        [SerializeField] private float attractDuration = 0.5f;

        protected override void OnCollect()
        {
            ExperienceManager manager = ExperienceManager.Instance;
            if (manager == null) return;

            manager.AttractAllOrbs(attractDuration);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.MagnetItemPickup,
                SoundType.SFX,
                volumeMultiplier: 2f,
                suppressDuplicateThisFrame: true));
        }
    }
}
