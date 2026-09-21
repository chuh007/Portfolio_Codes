using Chuh007Lib.Bus;

namespace _Work.CHUH.Code.Core.Events
{
    public struct UpgradeChestEvent : IEvent
    {
        public readonly int GetUpgradeCount;

        public UpgradeChestEvent(int upgradeCount)
        {
            GetUpgradeCount = upgradeCount < 1 ? 1 : upgradeCount;
        }
    }

}
