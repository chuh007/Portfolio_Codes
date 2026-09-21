using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Resource;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Item
{
    public class CoinItem : ItemBase
    {
        [SerializeField] private int coinAmount = 1;

        protected override void OnCollect()
        {
            if (SupplyManager.Instance == null)
            {
                Debug.LogWarning("[CoinItem] ResourceManager is missing in scene.");
                return;
            }

            SupplyManager.Instance.AddCoin(coinAmount);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.CoinPickup,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));
        }
    }
}
