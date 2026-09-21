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
    internal static class PermanentUpgradeTreeAssets
    {
        public const string TreeRoot = "Assets/_Work/CHUH/SO/Tree/PermanentTree";
        public const string NodeRoot = TreeRoot + "/Nodes";
        public const string UpgradeRoot = TreeRoot + "/Upgrades";
        public const string DatabasePath = "Assets/_Work/CHUH/SO/Tree/TreeNodeDatabase.asset";
        public const string TreeUiPrefabPath = "Assets/_Work/CHUH/Prefab/Tree/TreeUIController.prefab";
        public const string TreeManagerPrefabPath = "Assets/_Work/CHUH/Prefab/Tree/TreeManager.prefab";


        public static List<BaseUpgradeSO> BuildUpgrade(NodeSpec node, int upgradeId)
        {
            UpgradeSpec spec = node.Upgrade;
            string path = $"{UpgradeRoot}/{node.FileName}_{spec.Type}.asset";
            MetaUpgradeSO upgrade = LoadOrCreate<MetaUpgradeSO>(path);
            upgrade.id = upgradeId;
            upgrade.type = spec.Type;
            upgrade.value = spec.Value;
            upgrade.isPercent = spec.IsPercent;
            upgrade.effectDescription = spec.Description;
            EditorUtility.SetDirty(upgrade);
            return new List<BaseUpgradeSO> { upgrade };
        }

        public static void UpdateTreeUiPrefab(TreeNodeDataSO rootNode)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(TreeUiPrefabPath);
            try
            {
                TreeUIController controller = root.GetComponentInChildren<TreeUIController>(true);
                SerializedObject serializedController = new SerializedObject(controller);
                serializedController.FindProperty("defaultNodeObject").objectReferenceValue = rootNode;
                serializedController.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, TreeUiPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        public static void EnsureUpgradeContainer()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(TreeManagerPrefabPath);
            try
            {
                if (root.GetComponent<MetaUpgradeContainer>() == null)
                    root.AddComponent<MetaUpgradeContainer>();
                PrefabUtility.SaveAsPrefabAsset(root, TreeManagerPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        public static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        public static void EnsureFolders()
        {
            EnsureFolder("Assets/_Work/CHUH/SO/Tree", "PermanentTree");
            EnsureFolder(TreeRoot, "Nodes");
            EnsureFolder(TreeRoot, "Upgrades");
        }

        public static void EnsureFolder(string parent, string name)
        {
            string path = $"{parent}/{name}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, name);
        }

    }
}
