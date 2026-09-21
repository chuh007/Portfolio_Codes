using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class OrthodoxRockVenue
    {
        private const float VenueDuration = 7f;
        private const float VenueDowntime = 5f;
        private const float VenueTickInterval = 0.2f;
        private const float VenueVisualInterval = 0.4f;
        private readonly OrthodoxRockBandAttack _source;
        private readonly OrthodoxRockStats _stats;
        private readonly Collider2D[] _hits;
        private readonly System.Action<Collider2D, Vector3, float, float> _damageEnemy;
        private bool _venueActive;
        private float _phaseTimer;
        private float _venueTickTimer;
        private float _venueVisualTimer;
        private VenueGroundVisual _venueGroundVisual;

        public OrthodoxRockVenue(OrthodoxRockBandAttack source, OrthodoxRockStats stats,
            Collider2D[] hits, System.Action<Collider2D, Vector3, float, float> damageEnemy)
        {
            _source = source;
            _stats = stats;
            _hits = hits;
            _damageEnemy = damageEnemy;
        }

        public void Tick(float deltaTime)
        {
            if (!_venueActive)
            {
                _phaseTimer -= deltaTime / _source.ScaleCommonInterval(1f);
                if (_phaseTimer <= 0f)
                    BeginVenue();
                return;
            }

            _phaseTimer -= deltaTime;
            if (_phaseTimer <= 0f)
            {
                _venueActive = false;
                _phaseTimer = VenueDowntime;
                ReleaseVenueGround();
                return;
            }

            _venueTickTimer += deltaTime;
            _venueVisualTimer += deltaTime;

            RunAtInterval(ref _venueTickTimer, _source.ScaleCommonInterval(VenueTickInterval), DamageVenue);
            RunAtInterval(ref _venueVisualTimer, VenueVisualInterval, DrawVenue);
        }

        private void BeginVenue()
        {
            _venueActive = true;
            _phaseTimer = VenueDuration;
            _venueTickTimer = _source.ScaleCommonInterval(VenueTickInterval);
            _venueVisualTimer = VenueVisualInterval;
            ReleaseVenueGround();
            _venueGroundVisual = VenueGroundVisual.Create(
                _source.CombinationType,
                _source.OwnerTransform,
                _source.ScaleCommonRange(_stats.VenueRadius));
            DrawVenue();
            BandRuntimeVisuals.SpawnSignatureBurst(
                _source.Position,
                _source.ScaleCommonRange(_stats.VenueRadius),
                new Color(1f, 0.28f, 0.08f, 0.58f),
                new Color(1f, 0.8f, 0.18f, 0.68f),
                12,
                15f,
                0.72f,
                52);
        }

        public void ReleaseVenueGround()
        {
            if (_venueGroundVisual != null)
                _venueGroundVisual.Release();
            _venueGroundVisual = null;
        }

        private void DamageVenue()
        {
            int count = Physics2D.OverlapCircle(_source.Position, _source.ScaleCommonRange(_stats.VenueRadius), _source.TargetContactFilter, _hits);
            float damage = _stats.VenueDamagePerSecond * VenueTickInterval;
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy")) continue;

                _damageEnemy(hit, _source.Position, damage, _source.ScaleCommonRange(_stats.VenueRadius));
                var slowable = hit.GetComponent<ISlowable>() ?? hit.GetComponentInParent<ISlowable>();
                slowable?.ApplySlow(Mathf.Clamp01(_stats.SlowMultiplier), _source.ScaleCommonSlowDuration(VenueTickInterval + 0.1f));
            }
        }

        private void DrawVenue()
        {
            _venueGroundVisual?.SetRadius(_source.ScaleCommonRange(_stats.VenueRadius));
            BuildVisualEffect.SpawnCircle(
                _source.Position, _source.ScaleCommonRange(_stats.VenueRadius),
                new Color(0.92f, 0.16f, 0.08f, 0.22f), VenueVisualInterval + 0.08f, 44);
            BuildVisualEffect.SpawnCircle(
                _source.Position, _source.ScaleCommonRange(_stats.VenueRadius) * 0.88f,
                new Color(1f, 0.72f, 0.12f, 0.14f), VenueVisualInterval + 0.08f, 43, true);
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
