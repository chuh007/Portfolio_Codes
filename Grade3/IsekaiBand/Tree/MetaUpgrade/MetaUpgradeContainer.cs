using System.Collections.Generic;
using System.Linq;
using _Code.LCH._02.Scripts.Level;
using _Work.CHUH.Code.Core.Events;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Tree.MetaUpgrade
{
    // 선택한 영구 강화를 보관하고, 현재/다음 플레이어에게 적용하는 컨테이너
    public class MetaUpgradeContainer : MonoBehaviour
    {
        public static MetaUpgradeContainer Instance;

        private readonly List<MetaUpgradeSO> _metaUpgradeData = new List<MetaUpgradeSO>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Bus<TreeUpgradeEvent>.OnEvent += HandleAddMetaUpgrade;
                return;
            }

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance != this) return;

            Bus<TreeUpgradeEvent>.OnEvent -= HandleAddMetaUpgrade;
            Instance = null;
        }

        private void HandleAddMetaUpgrade(TreeUpgradeEvent evt)
        {
            AddMetaUpgrade(evt.Type, evt.UpgradeSO);
        }

        public void AddMetaUpgrade(MetaUpgradeType type, MetaUpgradeSO data)
        {
            if (data == null || _metaUpgradeData.Any(upgrade => upgrade != null && upgrade.id == data.id))
                return;

            _metaUpgradeData.Add(data);
            TryApplyUpgrade(data);
        }

        /// <summary>
        /// 새 플레이어가 준비됐을 때 보관 중인 강화를 모두 적용한다.
        /// </summary>
        public void ApplyUpgrade()
        {
            foreach (MetaUpgradeSO data in _metaUpgradeData)
            {
                TryApplyUpgrade(data);
            }
        }

        private static void TryApplyUpgrade(MetaUpgradeSO data)
        {
            if (data == null || ExperienceManager.Instance == null ||
                ExperienceManager.Instance.PlayerTransform == null)
                return;

            IMetaUpgradeable target = ExperienceManager.Instance.PlayerTransform
                .GetComponentInChildren<IMetaUpgradeable>(true);
            target?.ApplyUpgrade(data.type, data.id, data.value, data.isPercent);
        }
    }
}
