using UnityEngine;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal sealed class MinimapPlayerIcon
    {
        private Sprite _playerIconSprite;
        private readonly int _playerLayer = LayerMask.NameToLayer("Player");
        private readonly int _playerDashingLayer = LayerMask.NameToLayer("PlayerDashing");

        public static LayerMask EnsureDefaultLayerMask(LayerMask iconLayerMask)
        {
            if (iconLayerMask.value != 0) return iconLayerMask;

            int playerLayer = LayerMask.NameToLayer("Player");
            int playerDashingLayer = LayerMask.NameToLayer("PlayerDashing");
            int itemChestLayer = LayerMask.NameToLayer("ItemChest");
            int weaponLayer = LayerMask.NameToLayer("Weapon");

            if (playerLayer >= 0)
                iconLayerMask |= 1 << playerLayer;

            if (playerDashingLayer >= 0)
                iconLayerMask |= 1 << playerDashingLayer;

            if (itemChestLayer >= 0)
                iconLayerMask |= 1 << itemChestLayer;

            if (weaponLayer >= 0)
                iconLayerMask |= 1 << weaponLayer;

            return iconLayerMask;
        }

        public Sprite GetIconSprite(Transform owner, SpriteRenderer renderer)
        {
            if (IsPlayerIconOwner(owner))
            {
                if (_playerIconSprite == null && owner.gameObject.layer == _playerLayer)
                    _playerIconSprite = renderer.sprite;

                if (_playerIconSprite != null)
                    return _playerIconSprite;
            }

            return renderer.sprite;
        }

        public bool IsPlayerIconOwner(Transform owner)
        {
            if (owner == null) return false;

            int ownerLayer = owner.gameObject.layer;
            return ownerLayer == _playerLayer || ownerLayer == _playerDashingLayer;
        }
    }
}
