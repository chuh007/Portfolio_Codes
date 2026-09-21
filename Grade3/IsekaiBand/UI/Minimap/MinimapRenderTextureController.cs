using UnityEngine;

namespace _Work.CHUH.Code.UI.Minimap
{
    [DisallowMultipleComponent]
    public class MinimapRenderTextureController : MonoBehaviour
    {
        private const int DefaultCullingMask = (1 << 3) | (1 << 6) | (1 << 11) | (1 << 13) | (1 << 14);
        private const int DefaultIconLayerMask = (1 << 6) | (1 << 11) | (1 << 13) | (1 << 14);
        private const int DefaultOffscreenWeaponLayerMask = 1 << 14;

        [Header("Follow Target")]
        [SerializeField] private Transform target;
        [SerializeField] private bool autoFindPlayer = true;

        [Header("Camera")]
        [SerializeField] private Camera minimapCamera;
        [SerializeField] private bool createCameraIfMissing = true;
        [SerializeField] private float cameraZ = -10f;
        [SerializeField] private Vector3 cameraEulerAngles = Vector3.zero;
        [SerializeField] private LayerMask cullingMask = DefaultCullingMask;

        [Header("Render Texture")]
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField, Min(1f)] private float viewRadius = 50f;
        [SerializeField] private Color backgroundColor = Color.black;

        [Header("Sprite Icons")]
        [SerializeField] private bool showSpriteIcons = true;
        [SerializeField] private LayerMask iconLayerMask = DefaultIconLayerMask;
        [SerializeField] private RectTransform iconRoot;
        [SerializeField] private Vector2 iconSize = new Vector2(50f, 50f);
        [SerializeField, Min(0.02f)] private float iconRefreshInterval = 0.25f;
        [SerializeField] private bool hideIconsOutsideView = true;
        [SerializeField] private bool clipIconsToMinimap;

        [Header("Offscreen Weapon")]
        [SerializeField] private bool showOffscreenWeaponIndicators = true;
        [SerializeField] private LayerMask offscreenWeaponLayerMask = DefaultOffscreenWeaponLayerMask;
        [SerializeField, Min(0f)] private float offscreenWeaponIconOffset = 38f;
        [SerializeField] private float offscreenWeaponArrowOffset = 10f;
        [SerializeField] private Vector2 offscreenWeaponArrowSize = new Vector2(22f, 22f);
        [SerializeField] private Color offscreenWeaponArrowColor = Color.white;

        private MinimapSpriteIcons _icons;

        private void Awake()
        {
            _icons = new MinimapSpriteIcons(transform);
            iconLayerMask = MinimapPlayerIcon.EnsureDefaultLayerMask(iconLayerMask);
            target = MinimapCameraRig.ResolveTarget(target, autoFindPlayer);
            minimapCamera = MinimapCameraRig.ResolveCamera(this, minimapCamera, createCameraIfMissing);
            ApplyCameraSettings();
            MinimapIconFactory.EnsureRoot(transform, ref iconRoot, ReadIconSettings());
            _icons.Refresh(iconRoot, ReadIconSettings());
            MinimapCameraRig.Follow(minimapCamera, target, cameraZ, cameraEulerAngles);
        }

        private void LateUpdate()
        {
            if (target == null)
                target = MinimapCameraRig.ResolveTarget(target, autoFindPlayer);

            MinimapCameraRig.Follow(minimapCamera, target, cameraZ, cameraEulerAngles);
            _icons.Update(minimapCamera, ref iconRoot, ReadIconSettings());
        }

        private void OnValidate()
        {
            viewRadius = Mathf.Max(1f, viewRadius);
            iconSize.x = Mathf.Max(1f, iconSize.x);
            iconSize.y = Mathf.Max(1f, iconSize.y);
            iconRefreshInterval = Mathf.Max(0.02f, iconRefreshInterval);
            offscreenWeaponIconOffset = Mathf.Max(0f, offscreenWeaponIconOffset);
            offscreenWeaponArrowSize.x = Mathf.Max(1f, offscreenWeaponArrowSize.x);
            offscreenWeaponArrowSize.y = Mathf.Max(1f, offscreenWeaponArrowSize.y);

            if (minimapCamera != null)
                ApplyCameraSettings();
        }

        private void ApplyCameraSettings()
        {
            MinimapCameraRig.Apply(minimapCamera, viewRadius, backgroundColor, cullingMask, renderTexture);
        }

        private MinimapIconSettings ReadIconSettings()
        {
            return new MinimapIconSettings
            {
                ShowSpriteIcons = showSpriteIcons,
                LayerMask = iconLayerMask,
                IconSize = iconSize,
                RefreshInterval = iconRefreshInterval,
                HideOutsideView = hideIconsOutsideView,
                ClipToMinimap = clipIconsToMinimap,
                ShowWeaponIndicators = showOffscreenWeaponIndicators,
                WeaponLayerMask = offscreenWeaponLayerMask,
                WeaponIconOffset = offscreenWeaponIconOffset,
                WeaponArrowOffset = offscreenWeaponArrowOffset,
                WeaponArrowSize = offscreenWeaponArrowSize,
                WeaponArrowColor = offscreenWeaponArrowColor
            };
        }
    }
}
