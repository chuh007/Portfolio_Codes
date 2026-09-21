using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Tree.MetaUpgrade;
using Chuh007Lib.Bus;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Serialization;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Item
{
    // 중간 보스 잡으면 나오는 상자
    // 보상 단계와 줍는 순간의 행운에 따라 1~5개를 추첨한다.
    // 여기서는 이게 몇개인지만 지정하고, 실제로 업그레이드 뜨는거는 Bus로 위임한다.
    public class UpgradeChest : MonoBehaviour, IPoolable
    {
        [FormerlySerializedAs("upgradeCount")]
        [SerializeField, Range(1, 5)] private int rewardLevel = 1;
        [Tooltip("행운 1당 보상 개수가 한 개 늘어날 때마다 추가되는 가중치 비율")]
        [SerializeField, Min(0f)] private float luckWeightPerPoint = 0.15f;
        [SerializeField] private LayerMask collectorLayer = (1 << 6) | (1 << 11);
        [SerializeField] private bool deactivateOnOpen = true;

        private int _rewardLevel;
        private bool _isOpened;

        private void OnEnable()
        {
            ResetItem();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isOpened) return;
            if ((collectorLayer.value & (1 << other.gameObject.layer)) == 0) return;

            Open(other);
        }

        public void SetRewardLevel(int level)
        {
            _rewardLevel = Mathf.Clamp(level, 1, 5);
        }

        private static int GetLuckLevel(Collider2D collector)
        {
            var builds = collector.GetComponentInParent<PlayerCommonBuildCompo>();
            return builds != null
                ? builds.LuckLevel
                : Mathf.RoundToInt(MetaUpgradeRuntime.GetValue(collector, MetaUpgradeType.Luck));
        }
        
        private void Open(Collider2D collector)
        {
            if (_isOpened) return;

            _isOpened = true;
            int upgradeCount = UpgradeChestRewardCalculator.Roll(
                _rewardLevel,
                GetLuckLevel(collector),
                luckWeightPerPoint,
                Random.value);
            Bus<UpgradeChestEvent>.Raise(new UpgradeChestEvent(upgradeCount));

            if (deactivateOnOpen)
                _myPool.Push(this);
        }
        
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }

        private Pool _myPool;
        public void ResetItem()
        {
            _rewardLevel = Mathf.Clamp(rewardLevel, 1, 5);
            _isOpened = false;
        }
        
        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }
    }
}
