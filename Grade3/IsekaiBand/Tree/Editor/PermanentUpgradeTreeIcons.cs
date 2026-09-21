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
    internal static class PermanentUpgradeTreeIcons
    {
        public const string ExpIcon = "1e51203732164d0aa48f776b6e96f062";
        public const string DamageIcon = "abd8ee68fa994cbf865d5e98f9bd7bc8";
        public const string HealthIcon = "694eb8fcea9540c393d10ec8f8d1106e";
        public const string MoveIcon = "6e3b8907328349c3916931f8538b2fb4";
        public const string DefenceIcon = "d5171418a9e64892a0d34af5f608c2c4";
        public const string ProjectileIcon = "c0b393513f9c4f2ebd5a351060fd90af";

        public const string CooldownIcon = "e0dbc3eee06242a0a42f62bbd9595230";
        public const string HealIcon = "da74df0f633a4404ad60e6153b499617";
        public const string AttackRangeIcon = "9f41d204e9424906bef3741bfc422cbe";
        public const string ProjectileSpeedIcon = "df09857cf5fe4a6c952457d2057ed23a";
        public const string ProjectileRuntimeIcon = "da9b1b66b4e54b58a0429e84467ee4c4";
        public const string MagnetIcon = "39836a4bd88d41d0b8e6880267de8526";
        public const string LuckIcon = "96e55bc237e54635952018a1eb1144de";
        public const string GoldIcon = "d8def157a4b2431c95fa5445a5d7be6b";
        public const string RerollIcon = "7b074697282b453bb6233be958612d8d";
        public static Sprite LoadSprite(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
        }

        public static string IconGuidByUpgradeType(MetaUpgradeType type)
        {
            return type switch
            {
                MetaUpgradeType.Damage => DamageIcon,
                MetaUpgradeType.Defence => DefenceIcon,
                MetaUpgradeType.MaxHealth => HealthIcon,
                MetaUpgradeType.Heal => HealIcon,
                MetaUpgradeType.Cooldown => CooldownIcon,
                MetaUpgradeType.AttackRange => AttackRangeIcon,
                MetaUpgradeType.ProjectileSpeed => ProjectileSpeedIcon,
                MetaUpgradeType.ProjectileRuntime => ProjectileRuntimeIcon,
                MetaUpgradeType.ProjectileCount => ProjectileIcon,
                MetaUpgradeType.MoveSpeed => MoveIcon,
                MetaUpgradeType.Magnet => MagnetIcon,
                MetaUpgradeType.Luck => LuckIcon,
                MetaUpgradeType.GrowEXP => ExpIcon,
                MetaUpgradeType.MoreGold => GoldIcon,
                MetaUpgradeType.RerollCount => RerollIcon,
                _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
