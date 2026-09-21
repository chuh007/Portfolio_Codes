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
    public static class PermanentUpgradeTreeBuilder
    {
        [MenuItem("Tools/Tree System/Rebuild Permanent Upgrade Tree")]
        public static void Rebuild()
        {
            NodeSpec[] nodeSpecs = PermanentUpgradeTreeCatalog.NodeSpecs;
            PermanentUpgradeTreeValidation.ValidateNodeSpecs(nodeSpecs);
            PermanentUpgradeTreeAssets.EnsureFolders();

            Dictionary<string, TreeNodeDataSO> nodes = new Dictionary<string, TreeNodeDataSO>();
            int upgradeId = 20001;

            foreach (NodeSpec spec in nodeSpecs)
            {
                TreeNodeDataSO node = PermanentUpgradeTreeAssets.LoadOrCreate<TreeNodeDataSO>($"{PermanentUpgradeTreeAssets.NodeRoot}/{spec.FileName}.asset");
                node.nodeID = $"permanent.{spec.Key}";
                node.nodeName = spec.DisplayName;
                node.description = spec.Upgrade.Description;
                node.costCurrency = spec.CostCurrency;
                node.cost = spec.Cost;
                node.graphPosition = spec.GraphPosition;
                node.uiRow = spec.UiRow;
                node.uiOrder = spec.UiOrder;
                node.icon = LoadSprite(IconGuidByUpgradeType(spec.Upgrade.Type));
                node.upgrades = PermanentUpgradeTreeAssets.BuildUpgrade(spec, upgradeId++);
                nodes.Add(spec.Key, node);
                EditorUtility.SetDirty(node);
            }

            foreach (NodeSpec spec in nodeSpecs)
            {
                TreeNodeDataSO node = nodes[spec.Key];
                node.childNodes = spec.Children.Select(key => nodes[key]).ToList();
                EditorUtility.SetDirty(node);
            }

            TreeNodeDatabaseSO database = AssetDatabase.LoadAssetAtPath<TreeNodeDatabaseSO>(PermanentUpgradeTreeAssets.DatabasePath);
            database.treeNodes = nodeSpecs.Select(spec => nodes[spec.Key]).ToList();
            EditorUtility.SetDirty(database);

            PermanentUpgradeTreeAssets.UpdateTreeUiPrefab(nodes["root"]);
            PermanentUpgradeTreeAssets.EnsureUpgradeContainer();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = database;
            Debug.Log($"[PermanentUpgradeTreeBuilder] Rebuilt {nodes.Count} one-effect nodes.");
        }
    }
}
