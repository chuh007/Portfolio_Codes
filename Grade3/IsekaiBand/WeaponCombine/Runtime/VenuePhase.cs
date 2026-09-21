using System;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class VenuePhase
    {
        private const float VenueDuration = 7f;
        private const float VenueDowntime = 5f;
        private const float VenueTickInterval = 0.2f;
        private const float VenueVisualInterval = 0.4f;
        private readonly Action _begin;
        private readonly Action _damage;
        private readonly Action _draw;
        private readonly Action<float> _tickInside;
        private readonly Action _end;
        private bool _venueActive;
        private float _phaseTimer;
        private float _venueTickTimer;
        private float _venueVisualTimer;

        public VenuePhase(Action begin, Action damage, Action draw, Action<float> tickInside, Action end)
        {
            _begin = begin;
            _damage = damage;
            _draw = draw;
            _tickInside = tickInside;
            _end = end;
        }

        public void Tick(float deltaTime, bool isPermanent, float intervalMultiplier = 1f)
        {
            if (!_venueActive)
            {
                _phaseTimer -= deltaTime / intervalMultiplier;
                if (_phaseTimer <= 0f)
                    Begin(isPermanent, intervalMultiplier);
                return;
            }

            _phaseTimer -= deltaTime;
            _venueTickTimer += deltaTime;
            _venueVisualTimer += deltaTime;

            float tickInterval = VenueTickInterval * intervalMultiplier;
            while (_venueTickTimer >= tickInterval)
            {
                _venueTickTimer -= tickInterval;
                _damage();
            }

            while (_venueVisualTimer >= VenueVisualInterval)
            {
                _venueVisualTimer -= VenueVisualInterval;
                _draw();
            }

            _tickInside(deltaTime);

            if (isPermanent || _phaseTimer > 0f)
                return;

            _venueActive = false;
            _phaseTimer = VenueDowntime;
            _end();
        }

        private void Begin(bool isPermanent, float intervalMultiplier)
        {
            _venueActive = true;
            _phaseTimer = isPermanent ? float.PositiveInfinity : VenueDuration;
            _venueTickTimer = VenueTickInterval * intervalMultiplier;
            _venueVisualTimer = VenueVisualInterval;
            _begin();
        }
    }
}
