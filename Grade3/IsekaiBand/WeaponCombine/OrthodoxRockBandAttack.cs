using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class OrthodoxRockBandAttack : CombineWeaponBase
    {
        private const float LightningInterval = 0.38f;
        private const float ShockwaveInterval = 1.2f;
        private const float RockVolleyInterval = 7f;
        private readonly OrthodoxRockStats _stats = new();
        private readonly BandRuntimeObjects _shockwaves = new();
        private readonly BandRuntimeObjects _rocks = new();
        private readonly OrthodoxRockVenue _venue;
        private readonly OrthodoxRockProjectiles _projectiles;
        private readonly OrthodoxRockLightning _lightning;
        private PlayerMovementCompo _movement;
        private Rigidbody2D _ownerRb;
        private Vector2 _facing = Vector2.right;
        private float _rockVolleyTimer = RockVolleyInterval;
        private float _lightningTimer = LightningInterval;
        private float _shockwaveTimer = ShockwaveInterval;

        public override CombineWeaponType CombinationType => CombineWeaponType.OrthodoxRockBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Vocal;

        internal Vector3 Position => OwnerPosition;

        public OrthodoxRockBandAttack()
        {
            var hits = new Collider2D[128];
            System.Action<Collider2D, Vector3, float, float> damage =
                (hit, position, amount, radius) => DamageEnemy(hit, position, amount, radius);
            _venue = new OrthodoxRockVenue(this, _stats, hits, damage);
            _projectiles = new OrthodoxRockProjectiles(this, _stats, _rocks, _shockwaves);
            _lightning = new OrthodoxRockLightning(this, _stats, hits, damage);
        }

        public override void Init(
            PlayerBasicAttackDataSo data,
            Transform ownerTransform,
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
            _shockwaves.RemoveDestroyed(includeInactive: false);
            _rocks.RemoveDestroyed();
            _rockVolleyTimer += deltaTime;
            _lightningTimer += deltaTime;
            _shockwaveTimer += deltaTime;
            RunAtInterval(ref _rockVolleyTimer, ScaleCommonInterval(RockVolleyInterval), _projectiles.FireRockVolley);
            RunAtInterval(ref _lightningTimer, ScaleCommonInterval(LightningInterval), _lightning.StrikeLightning);
            RunAtInterval(ref _shockwaveTimer, ScaleCommonInterval(ShockwaveInterval), _projectiles.FireShockwave);

            _venue.Tick(deltaTime);
        }

        public override void Dispose()
        {
            _shockwaves.DestroyAll();
            _rocks.ReturnToPool();
            _venue.ReleaseVenueGround();
            base.Dispose();
        }

        protected override void OnAttack()
        {
        }

        internal void HitShockwave(Collider2D hit, Vector3 center)
            => DamageEnemy(hit, center, _stats.ShockwaveDamage, ScaleCommonRange(_stats.VenueRadius), _stats.ShockwaveKnockback);

        internal void HitRock(Collider2D hit, Vector3 position, float damage, float radius)
        {
            DamageEnemy(hit, position, damage, radius);
        }

        internal void ProjectileEnded(OrthodoxRockProjectile projectile)
        {
            if (projectile != null)
                _rocks.Remove(projectile.gameObject);
        }

        private void RefreshFacing()
        {
            if (_movement != null && _movement.LastMoveDirection.sqrMagnitude > 0.001f)
                _facing = _movement.LastMoveDirection.normalized;
            else if (_ownerRb != null && _ownerRb.linearVelocity.sqrMagnitude > 0.01f)
                _facing = _ownerRb.linearVelocity.normalized;
        }

        private static void RunAtInterval(ref float timer, float interval, System.Action action)
        {
            while (timer >= interval)
            {
                timer -= interval;
                action();
            }
        }
    }
}
