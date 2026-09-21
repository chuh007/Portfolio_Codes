using System.Collections.Generic;
using System.Linq;
using _Work.CHUH.Code.Tree.MetaUpgrade;
using _Work.CHUH.Code.Tree.UI;
using _Work.CHUH.Code.Tree.Upgrade;
using UnityEditor;
using UnityEngine;

using static _Work.CHUH.Code.Tree.Editor.PermanentUpgradeTreeIcons;
using static _Work.CHUH.Code.Tree.Editor.PermanentUpgradeTreeSpec;

namespace _Work.CHUH.Code.Tree.Editor
{
    internal static class PermanentUpgradeTreeSpec
    {
        public static NodeSpec N(string key, string fileName, string displayName, int cost, int uiRow, int uiOrder,
            string iconGuid, string[] children, UpgradeSpec upgrade,
            TreeNodeCostCurrency costCurrency = TreeNodeCostCurrency.Coin)
        {
            return new NodeSpec(key, fileName, displayName, cost, costCurrency, uiRow, uiOrder, iconGuid, children, upgrade);
        }

        public static string[] C(params string[] children) => children;

        public static UpgradeSpec U(MetaUpgradeType type, float value, bool isPercent, string description)
        {
            return new UpgradeSpec(type, value, isPercent, description);
        }

        public static int P(int baseCost) => baseCost * 2;

        public sealed class NodeSpec
        {
            public readonly string Key;
            public readonly string FileName;
            public readonly string DisplayName;
            public readonly int Cost;
            public readonly TreeNodeCostCurrency CostCurrency;
            public readonly int UiRow;
            public readonly int UiOrder;
            public readonly string IconGuid;
            public readonly string[] Children;
            public readonly UpgradeSpec Upgrade;

            public Vector2 GraphPosition => new Vector2(UiRow * 300f, (UiOrder - 1) * 240f);

            public NodeSpec(string key, string fileName, string displayName, int cost,
                TreeNodeCostCurrency costCurrency, int uiRow, int uiOrder, string iconGuid,
                string[] children, UpgradeSpec upgrade)
            {
                Key = key;
                FileName = fileName;
                DisplayName = displayName;
                Cost = cost;
                CostCurrency = costCurrency;
                UiRow = uiRow;
                UiOrder = uiOrder;
                IconGuid = iconGuid;
                Children = children;
                Upgrade = upgrade;
            }
        }

        public readonly struct UpgradeSpec
        {
            public readonly MetaUpgradeType Type;
            public readonly float Value;
            public readonly bool IsPercent;
            public readonly string Description;

            public UpgradeSpec(MetaUpgradeType type, float value, bool isPercent, string description)
            {
                Type = type;
                Value = value;
                IsPercent = isPercent;
                Description = description;
            }
        }
    }
}
