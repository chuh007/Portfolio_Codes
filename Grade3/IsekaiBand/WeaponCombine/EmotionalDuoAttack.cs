using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class EmotionalDuoAttack : CombineWeaponBase
    {
        private readonly EmotionalDuoStats _stats = new();
        private readonly EmotionalDuoNotes _notes;
        private readonly Collider2D[] _hits = new Collider2D[96];
        private PlayerMovementCompo _movement;
        private Rigidbody2D _ownerRb;
        private Vector2 _facing = Vector2.right;
        private float _vocalTimer;
        private float _noteTimer;

        public override CombineWeaponType CombinationType => CombineWeaponType.EmotionalDuo;
        public override WeaponType PrimaryWeaponType => WeaponType.Vocal;

        internal Vector3 Position => OwnerPosition;

        public EmotionalDuoAttack() => _notes = new EmotionalDuoNotes(this, _stats);

        public override void Init(PlayerBasicAttackDataSo data, Transform ownerTransform,
            System.Func<Vector3, Quaternion, GameObject> spawner)
        {
            base.Init(data, ownerTransform, spawner);
            _movement = ownerTransform != null ? ownerTransform.GetComponent<PlayerMovementCompo>() : null;
            _ownerRb = ownerTransform != null ? ownerTransform.GetComponent<Rigidbody2D>() : null;
        }

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => _stats.Configure(ingredients);

        public override void Tick(float deltaTime)
        {
            RefreshFacing();
            _vocalTimer += deltaTime;
            _noteTimer += deltaTime;

            float tickInterval = ScaleCommonInterval(_stats.VocalTickInterval);
            while (_vocalTimer >= tickInterval)
            {
                _vocalTimer -= tickInterval;
                DamagePermanentCone();
            }

            float noteInterval = ScaleCommonInterval(_stats.NoteInterval);
            while (_noteTimer >= noteInterval)
            {
                _noteTimer -= noteInterval;
                _notes.FireVolley(_facing);
            }
        }

        protected override void OnAttack()
        {
        }

        private void DamagePermanentCone()
        {
            AttackAudio.PlayVocal();
            Vector2 direction = _facing.sqrMagnitude > 0.001f ? _facing.normalized : Vector2.right;
            int count = Physics2D.OverlapCircle(OwnerPosition, ScaleCommonRange(_stats.VocalRange), TargetContactFilter, _hits);
            float halfAngle = _stats.ConeAngle * 0.5f;

            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy")) continue;

                Vector2 toTarget = (Vector2)hit.transform.position - (Vector2)OwnerPosition;
                if (toTarget.sqrMagnitude < 0.001f || Vector2.Angle(direction, toTarget) > halfAngle) continue;
                DamageEnemy(hit, OwnerPosition, _stats.VocalDamage, ScaleCommonRange(_stats.VocalRange));
            }

            BuildVisualEffect.SpawnConeWave(
                OwnerPosition, direction, ScaleCommonRange(_stats.VocalRange), _stats.ConeAngle,
                new Color(1f, 0.36f, 0.66f, 0.3f), duration: 0.18f, sortingOrder: 45);
        }

        private void RefreshFacing()
        {
            if (_movement != null && _movement.LastMoveDirection.sqrMagnitude > 0.001f)
                _facing = _movement.LastMoveDirection.normalized;
            else if (_ownerRb != null && _ownerRb.linearVelocity.sqrMagnitude > 0.01f)
                _facing = _ownerRb.linearVelocity.normalized;
        }
    }
}
