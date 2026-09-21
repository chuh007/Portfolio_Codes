using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Enemies.Boss;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoBossPose
    {
        private readonly PianoBossRuntime _runtime;
        private const int Phase2Index = 1;
        private Enemy _owner;
        private Boss _boss;
        private EntityMover _mover;
        private Rigidbody2D _rigidbody;
        private PianoKeySoundPlayer _keySoundPlayer;
        private Sprite _fallbackPhase1Sprite;
        public Enemy Owner => _owner;
        public Boss Boss => _boss;
        public PianoKeySoundPlayer KeySoundPlayer => _keySoundPlayer;

        public PianoBossPose(PianoBossRuntime runtime) => _runtime = runtime;

        public void EnsureBound(Enemy owner)
        {
            if (owner == null)
                return;

            _owner = owner;
            _boss = owner as Boss ?? owner.GetComponent<Boss>();
            _mover = owner.GetComponentInChildren<EntityMover>();
            _rigidbody = owner.GetComponent<Rigidbody2D>();
            _keySoundPlayer = owner.GetComponent<PianoKeySoundPlayer>();

            if (_runtime.VisualRenderer == null)
                _runtime.VisualRenderer = owner.GetComponentInChildren<SpriteRenderer>();

            if (_runtime.VisualRoot == null && _runtime.VisualRenderer != null)
                _runtime.VisualRoot = _runtime.VisualRenderer.transform;

            if (_fallbackPhase1Sprite == null && _runtime.VisualRenderer != null)
                _fallbackPhase1Sprite = _runtime.VisualRenderer.sprite;

            ApplyPhaseSprite();
        }

        public void ApplyPhaseSprite()
        {
            if (_runtime.VisualRenderer == null)
                return;

            Sprite sprite = ResolvePhaseSprite();
            if (sprite != null && _runtime.VisualRenderer.sprite != sprite)
                _runtime.VisualRenderer.sprite = sprite;
        }

        private Sprite ResolvePhaseSprite()
        {
            Sprite defaultSprite = _runtime.Phase1Sprite != null ? _runtime.Phase1Sprite : _fallbackPhase1Sprite;
            if (_boss != null && _boss.CurrentPhaseIndex >= Phase2Index)
                return _runtime.Phase2Sprite != null ? _runtime.Phase2Sprite : defaultSprite;

            return defaultSprite;
        }

        public void LockMovement()
        {
            if (_mover != null)
            {
                _mover.StopImmediately();
                _mover.CanManualMove = false;
            }

            if (_rigidbody != null)
                _rigidbody.linearVelocity = Vector2.zero;
        }

        public void SnapToAnchor()
        {
            if (!_runtime.LockToArenaTop)
                return;

            Rect bounds = ResolveArenaBounds();
            _runtime.transform.position = new Vector3(bounds.center.x, bounds.yMax - _runtime.TopOffset, _runtime.transform.position.z);
        }

        private Rect ResolveArenaBounds()
        {
            StageHelper helper = StageHelper.Instance;
            if (helper != null && helper.IsBossArenaActive)
                return new Rect(helper.BossArenaCenter - helper.BossArenaSize * 0.5f, helper.BossArenaSize);

            Vector2 center = _owner != null && _owner.target != null
                ? _owner.target.transform.position
                : (Vector2)_runtime.transform.position;
            Vector2 size = Vector2.Max(Vector2.one, _runtime.FallbackArenaSize);
            return new Rect(center - size * 0.5f, size);
        }
    }
}
