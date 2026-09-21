namespace _Work.CHUH.Code.Tree.MetaUpgrade
{
    public interface IMetaUpgradeable
    {
        public void ApplyUpgrade(MetaUpgradeType type, int id, float value, bool isPercent);
    }
}