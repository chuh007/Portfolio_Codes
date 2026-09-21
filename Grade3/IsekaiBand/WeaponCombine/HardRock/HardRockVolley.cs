using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class HardRockVolley
    {
        private const int RockBurstCount = 3;
        private const float RockBurstInterval = 0.08f;
        private readonly HardRockBandAttack _source;
        private readonly HardRockStats _stats;
        private readonly BandRuntimeObjects _runtimeObjects;
        private readonly Queue<(Vector2 Direction, int Count)> _pendingRockShots = new();
        private PlayerMovementCompo _movement;
        private Rigidbody2D _ownerRb;
        private Vector2 _facing = Vector2.right;
        private float _rockBurstTimer;

        public HardRockVolley(HardRockBandAttack source, HardRockStats stats, BandRuntimeObjects runtimeObjects)
        {
            _source = source;
            _stats = stats;
            _runtimeObjects = runtimeObjects;
        }

        public void Init(Transform ownerTransform)
        {
            _movement = ownerTransform != null ? ownerTransform.GetComponent<PlayerMovementCompo>() : null;
            _ownerRb = ownerTransform != null ? ownerTransform.GetComponent<Rigidbody2D>() : null;
        }

        public void Clear()
        {
            _pendingRockShots.Clear();
            _rockBurstTimer = 0f;
        }

        public void EnqueueBurst()
        {
            Vector2 direction = ResolveFireDirection();
            bool wasEmpty = _pendingRockShots.Count == 0;
            int rockCount = _source.CountProjectiles(RockBurstCount);
            int burstCount = Mathf.Min(RockBurstCount, rockCount);
            // 추가 탄은 기존 점사에 나눠 담아 점사 시간과 대기열 길이를 유지한다.
            for (int i = 0; i < burstCount; i++)
            {
                int count = rockCount / burstCount + (i < rockCount % burstCount ? 1 : 0);
                _pendingRockShots.Enqueue((direction, count));
            }

            if (wasEmpty)
                _rockBurstTimer = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (_pendingRockShots.Count == 0)
            {
                _rockBurstTimer = 0f;
                return;
            }

            _rockBurstTimer -= deltaTime;
            if (_rockBurstTimer > 0f) return;

            var shot = _pendingRockShots.Dequeue();
            for (int i = 0; i < shot.Count; i++)
            {
                float angle = (i - (shot.Count - 1) * 0.5f) * 10f;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * shot.Direction;
                SpawnRock(direction);
            }
            _rockBurstTimer = _source.ScaleCommonInterval(RockBurstInterval);
        }

        private void SpawnRock(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject obj = ProjectilePool.Pop(
                "HardRockProjectile",
                _source.Position,
                Quaternion.Euler(0f, 0f, angle));
            if (obj == null)
                return;

            obj.name = "HardRockProjectile";
            obj.transform.SetPositionAndRotation(_source.Position, Quaternion.Euler(0f, 0f, angle));
            obj.transform.localScale = Vector3.one * 4.65f;
            ProjectileRenderLayer.ApplyTo(obj);

            var projectile = obj.GetComponent<HardRockProjectile>() ?? obj.AddComponent<HardRockProjectile>();
            projectile.Init(_source, direction, _stats.RockSpeed, _source.ScaleCommonRange(_stats.RockRange));
            _runtimeObjects.Add(obj);
        }

        private Vector2 ResolveFireDirection()
        {
            Transform target = ManualTargetingService.FindPriorityOrNearest(_source.Position, _source.ScaleCommonRange(_stats.RockRange));
            if (target != null)
                return ((Vector2)target.position - (Vector2)_source.Position).normalized;
            return _facing.sqrMagnitude > 0.001f ? _facing.normalized : Vector2.right;
        }

        public void RefreshFacing()
        {
            if (_movement != null && _movement.LastMoveDirection.sqrMagnitude > 0.001f)
                _facing = _movement.LastMoveDirection.normalized;
            else if (_ownerRb != null && _ownerRb.linearVelocity.sqrMagnitude > 0.01f)
                _facing = _ownerRb.linearVelocity.normalized;
        }
    }
}
