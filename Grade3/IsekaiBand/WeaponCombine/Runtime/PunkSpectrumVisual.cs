using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class PunkSpectrumVisual : MonoBehaviour
    {

        private readonly PunkSpectrumLines _lines = new();
        private readonly PunkSpectrumPainter _painter = new();
        private float _drawnTime = float.NaN;
        private float _drawnRadius;
        private Color _drawnColor;
        private float _drawnSpikeTime;
        private bool _spikesChanged;
        private LineRenderer _drawnRing;
        private Transform _target;
        private float _baseRadius;
        private Color _color;
        private float[] _spikeAngles = System.Array.Empty<float>();
        private float _spikeRadius;
        private float _spikeDuration;
        private float _spikeTimeRemaining;

        public void InitOrUpdate(Transform target, float baseRadius, Color color)
        {
            _target = target;
            _baseRadius = Mathf.Max(0.1f, baseRadius);
            _color = color;
            _lines.Create(transform);
            FollowTarget();
            DrawSpectrum();
        }

        public void SetDamageSpikes(float[] angles, float spikeRadius, float duration)
        {
            _spikeAngles = angles ?? System.Array.Empty<float>();
            _painter.SetSpikes(_spikeAngles);
            _spikesChanged = true;
            _spikeRadius = Mathf.Max(_baseRadius, spikeRadius);
            _spikeDuration = Mathf.Max(0.05f, duration);
            _spikeTimeRemaining = _spikeDuration;
        }

        private void Update()
        {
            if (_target == null || !_target.gameObject.activeInHierarchy)
            {
                Destroy(gameObject);
                return;
            }

            _spikeTimeRemaining = Mathf.Max(0f, _spikeTimeRemaining - Time.deltaTime);
            FollowTarget();
            DrawSpectrum();
        }

        private void FollowTarget()
        {
            transform.position = _target != null ? _target.position : transform.position;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        private void OnDestroy() => _lines.Dispose();

        private void DrawSpectrum()
        {
            if (!_spikesChanged && _drawnTime.Equals(Time.time) && _drawnRadius.Equals(_baseRadius)
                && _drawnColor.Equals(_color) && _drawnSpikeTime.Equals(_spikeTimeRemaining)
                && _drawnRing == _lines.InnerRing) return;
            _painter.Draw(_lines, _baseRadius, _color,
                _spikeAngles, _spikeRadius, _spikeDuration, _spikeTimeRemaining);
            _drawnTime = Time.time;
            _drawnRadius = _baseRadius;
            _drawnColor = _color;
            _drawnSpikeTime = _spikeTimeRemaining;
            _spikesChanged = false;
            _drawnRing = _lines.InnerRing;
        }
    }
}
