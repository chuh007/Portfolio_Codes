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
    public sealed class HardRockBandAttack : CombineWeaponBase
    {
        private readonly HardRockStats _stats = new();
        private readonly BandRuntimeObjects _runtimeObjects = new();
        private readonly HardRockVolley _volley;
        private readonly HardRockFragments _fragments;
        private readonly Collider2D[] _hits = new Collider2D[128];
        private readonly HashSet<int> _shockwaveTargetIds = new();

        public override CombineWeaponType CombinationType => CombineWeaponType.HardRockBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Guitar;

        internal Vector3 Position => OwnerPosition;

        public HardRockBandAttack()
        {
            _volley = new HardRockVolley(this, _stats, _runtimeObjects);
            _fragments = new HardRockFragments(this, _stats, _runtimeObjects);
        }

        public override void Init(
            PlayerBasicAttackDataSo data,
            Transform ownerTransform,
            System.Func<Vector3, Quaternion, GameObject> spawner)
        {
            base.Init(data, ownerTransform, spawner);
            _volley.Init(ownerTransform);
        }

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => _stats.Configure(ingredients, this);

        public override void Tick(float deltaTime)
        {
            _volley.RefreshFacing();
            base.Tick(deltaTime);
            _volley.Tick(deltaTime);
            _runtimeObjects.RemoveDestroyed();
        }

        public override void Dispose()
        {
            _runtimeObjects.ReturnToPool();
            _volley.Clear();
            base.Dispose();
        }

        protected override void OnAttack() => _volley.EnqueueBurst();
        internal void BreakRock(Vector3 position) => _fragments.Spawn(position);

        internal void HitRock(Collider2D hit, Vector3 position)
        {
            DamageEnemy(hit, position, _stats.RockDamage, ScaleCommonRange(_stats.RockRange));
            SpawnBassShockwave(position);
        }

        internal void HitFragment(Collider2D hit, Vector3 position)
            => DamageEnemy(hit, position, _stats.FragmentDamage, ScaleCommonRange(_stats.FragmentRange));

        internal void ProjectileEnded(GameObject projectile)
        {
            if (projectile != null)
                _runtimeObjects.Remove(projectile);
        }

        internal static int GetTargetId(Collider2D hit)
        {
            if (hit == null) return 0;
            Entity entity = hit.GetComponent<Entity>() ?? hit.GetComponentInParent<Entity>();
            return entity != null ? entity.GetInstanceID() : hit.transform.root.GetInstanceID();
        }

        private void SpawnBassShockwave(Vector3 position)
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.DrumShockwave,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));

            Color color = new(0.18f, 0.78f, 1f, 0.66f);
            if (!DrumWaveAnimationEffect.Spawn(position, ScaleCommonRange(_stats.ShockwaveRadius), color, 0.32f, 51))
                BuildVisualEffect.SpawnCircle(position, ScaleCommonRange(_stats.ShockwaveRadius), color, 0.32f, 51, true);

            _shockwaveTargetIds.Clear();
            int count = Physics2D.OverlapCircle(position, ScaleCommonRange(_stats.ShockwaveRadius), TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D candidate = _hits[i];
                if (candidate == null || !candidate.CompareTag("Enemy")) continue;
                if (!ManualTargetingService.IsValid(candidate.transform)) continue;
                if (!_shockwaveTargetIds.Add(GetTargetId(candidate))) continue;

                DamageEnemy(
                    candidate, position, _stats.ShockwaveDamage,
                    ScaleCommonRange(_stats.ShockwaveRadius));
            }

            RaiseImpact(new AttackEventContext(
                this, null, position, Vector2.zero, _stats.ShockwaveDamage,
                ScaleCommonRange(_stats.ShockwaveRadius), ScaleCommonRange(_stats.ShockwaveRadius), 0f, 0f, 0, 0f, 0f));
        }
    }
}
