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
    internal static class PermanentUpgradeTreeValidation
    {
        private const int ExpectedRowCount = 30;
        private const int ExpectedHealRewardCount = 5;
        private const int ExpectedNoteMilestoneCount = 4;
        private const float HealRewardValue = 0.1f;

        public static void ValidateNodeSpecs(NodeSpec[] nodeSpecs)
        {
            if (nodeSpecs.Select(spec => spec.Key).Distinct().Count() != nodeSpecs.Length)
                throw new System.InvalidOperationException("Permanent upgrade tree contains duplicate node keys.");

            HashSet<string> keys = nodeSpecs.Select(spec => spec.Key).ToHashSet();
            if (nodeSpecs.SelectMany(spec => spec.Children).Any(child => !keys.Contains(child)))
                throw new System.InvalidOperationException("Permanent upgrade tree contains an unknown child key.");

            int[] rows = nodeSpecs.Select(spec => spec.UiRow).Distinct().OrderBy(row => row).ToArray();
            if (rows.Length != ExpectedRowCount || rows[0] != 0 || rows[^1] != ExpectedRowCount - 1)
                throw new System.InvalidOperationException($"Permanent upgrade tree must cover rows 0-{ExpectedRowCount - 1}.");

            NodeSpec[] noteMilestones = nodeSpecs
                .Where(spec => spec.CostCurrency == TreeNodeCostCurrency.Note)
                .ToArray();
            if (noteMilestones.Length != ExpectedNoteMilestoneCount
                || noteMilestones.Any(spec => spec.Cost != 1 || spec.Children.Length == 0))
            {
                throw new System.InvalidOperationException(
                    $"Permanent upgrade tree must contain exactly {ExpectedNoteMilestoneCount} pass-through note milestones costing 1 note each.");
            }

            UpgradeSpec[] healRewards = nodeSpecs
                .Select(spec => spec.Upgrade)
                .Where(upgrade => upgrade.Type == MetaUpgradeType.Heal)
                .ToArray();
            if (healRewards.Length != ExpectedHealRewardCount
                || healRewards.Any(upgrade => !Mathf.Approximately(upgrade.Value, HealRewardValue)))
            {
                throw new System.InvalidOperationException(
                    $"Permanent upgrade tree must contain exactly {ExpectedHealRewardCount} heal rewards worth {HealRewardValue} each.");
            }
        }
    }
}
