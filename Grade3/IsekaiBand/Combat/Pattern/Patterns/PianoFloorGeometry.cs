using _Code.LCH._02.Scripts.Combat;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoFloorGeometry
    {
        private readonly PianoBossRuntime _runtime;
        private const float BoundsRefreshDistance = 0.4f;
        private Rect _arenaBounds;
        private DamageData _floorDamage;
        private LayerMask _floorTargetMask;
        private Sprite _keySprite;
        private bool _phase2FloorActive;
        public bool IsActive => _phase2FloorActive;
        public Rect Bounds => _arenaBounds;
        public DamageData Damage => _floorDamage;
        public LayerMask TargetMask => _floorTargetMask;
        public Sprite KeySprite => _keySprite;

        public PianoFloorGeometry(PianoBossRuntime runtime) => _runtime = runtime;

        public void EnsurePhase2Floor(
            Rect bounds,
            Sprite keySprite,
            DamageData floorDamage,
            LayerMask targetMask)
        {
            _floorDamage = floorDamage;
            _floorTargetMask = targetMask;
            _keySprite = keySprite != null ? keySprite : _runtime.SpriteFactory.GetSolidSprite();
            Rect floorBounds = ExpandPianoBounds(bounds);

            bool needsRebuild = !_phase2FloorActive
                                || _runtime.FloorVisuals.Root == null
                                || Vector2.Distance(_arenaBounds.center, floorBounds.center) > BoundsRefreshDistance
                                || Vector2.Distance(_arenaBounds.size, floorBounds.size) > BoundsRefreshDistance;

            _arenaBounds = floorBounds;
            _phase2FloorActive = true;

            if (needsRebuild)
            {
                _runtime.FloorVisuals.DestroyFloor();
                _runtime.FloorVisuals.BuildPianoFloor(floorBounds);
            }
        }

        public int GetKeyIndex(Vector2 worldPosition)
        {
            if (!_phase2FloorActive || _arenaBounds.width <= 0f)
                return 0;

            float t = Mathf.InverseLerp(_arenaBounds.xMin, _arenaBounds.xMax, worldPosition.x);
            return Mathf.Clamp(Mathf.FloorToInt(t * _runtime.KeyCount), 0, _runtime.KeyCount - 1);
        }

        public Rect GetKeyRect(int keyIndex)
        {
            int safeIndex = Mathf.Clamp(keyIndex, 0, _runtime.KeyCount - 1);
            float width = _arenaBounds.width / _runtime.KeyCount;
            return new Rect(_arenaBounds.xMin + width * safeIndex, _arenaBounds.yMin, width, _arenaBounds.height);
        }

        private Rect ExpandPianoBounds(Rect bounds)
        {
            int baseCount = _runtime.BaseKeyCount;
            float keyWidth = bounds.width > 0f ? bounds.width / baseCount : 1f;
            float extraWidth = keyWidth * _runtime.ExtraKeyCount;
            float bottomExtraHeight = Mathf.Max(0f, _runtime.BottomExtension);
            float topExtraHeight = Mathf.Max(0f, _runtime.TopExtension);
            return new Rect(
                bounds.xMin - extraWidth,
                bounds.yMin - bottomExtraHeight,
                bounds.width + extraWidth * 2f,
                bounds.height + bottomExtraHeight + topExtraHeight);
        }
    }
}
