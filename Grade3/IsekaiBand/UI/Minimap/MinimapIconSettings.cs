using UnityEngine;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal struct MinimapIconSettings
    {
        public bool ShowSpriteIcons;
        public LayerMask LayerMask;
        public Vector2 IconSize;
        public float RefreshInterval;
        public bool HideOutsideView;
        public bool ClipToMinimap;
        public bool ShowWeaponIndicators;
        public LayerMask WeaponLayerMask;
        public float WeaponIconOffset;
        public float WeaponArrowOffset;
        public Vector2 WeaponArrowSize;
        public Color WeaponArrowColor;
    }
}
