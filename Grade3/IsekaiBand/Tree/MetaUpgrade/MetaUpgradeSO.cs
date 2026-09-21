using Chuh007Lib.StatSystem;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Tree.Upgrade;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Tree.MetaUpgrade
{
    // 영구적 강화요소
    // 기본딜증, 방어력, 최대체력증가, 회복, 쿨감, 공격 범위, 투사체 속도, 지속 시간, 투사체 수, 이속, 자석, 행운, 추가 경치, 골드
    [CreateAssetMenu(fileName = "MetaUpgrade_", menuName = "SO/Tree/Upgrade/Meta Upgrade", order = 0)]
    public class MetaUpgradeSO : BaseUpgradeSO
    {
        /// <summary>
        /// 중복 불가능
        /// </summary>
        public int id;
        public MetaUpgradeType type;
        public float value;
        public bool isPercent;
        public override void Apply()
        {
            Bus<TreeUpgradeEvent>.Raise(new TreeUpgradeEvent(type, this));
        }
    }
}
