using _Work.CHUH.Code.Tree.MetaUpgrade;
using _Work.CHUH.Code.Tree.Upgrade;
using Chuh007Lib.Bus;

namespace _Work.CHUH.Code.Core.Events
{
    public struct TreeUpgradeEvent : IEvent
    {
        public MetaUpgradeType Type;
        public MetaUpgradeSO UpgradeSO;
        
        public TreeUpgradeEvent(MetaUpgradeType type, MetaUpgradeSO upgradeSO)
        {
            Type = type;
            UpgradeSO = upgradeSO;
        }
    }
}