using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Visual
{
    public class PooledParticleVFX : MonoBehaviour, IPoolable, IPooledVFX
    {
        [SerializeField] private PoolItemSO poolItem;
        [SerializeField] private bool rotateToDirection;

        private Pool _pool;
        private ParticleSystem[] _particles;
        private Vector3 _baseScale;
        private bool _isPlaying;

        public PoolItemSO PoolItem => poolItem;

        private void Awake()
        {
            _particles = GetComponentsInChildren<ParticleSystem>(true);
            _baseScale = transform.localScale;
        }

        public void ResetItem()
        {
            EnsureParticles();
            _isPlaying = false;

            foreach (var particle in _particles)
            {
                if (particle == null) continue;
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
        }

        public void Play(Vector3 position, float scale, Vector2 direction)
        {
            EnsureParticles();

            transform.position = position;
            transform.localScale = _baseScale * scale;
            transform.rotation = GetDirectionRotation(direction);
            _isPlaying = true;

            foreach (var particle in _particles)
            {
                if (particle == null) continue;
                particle.Clear(true);
                particle.Play(true);
            }
        }

        private Quaternion GetDirectionRotation(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.001f)
                direction = Vector2.right;

            if (rotateToDirection)
                return Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));

            return Quaternion.Euler(0f, direction.x < 0f ? 180f : 0f, 0f);
        }

        private void Update()
        {
            if (!_isPlaying) return;

            EnsureParticles();

            foreach (var particle in _particles)
            {
                if (particle != null && particle.IsAlive(true))
                    return;
            }

            _isPlaying = false;
            _pool?.Push(this);
        }

        private void EnsureParticles()
        {
            if (_particles is { Length: > 0 }) return;

            _particles = GetComponentsInChildren<ParticleSystem>(true);
            if (_baseScale == Vector3.zero)
                _baseScale = transform.localScale;
        }
    }
}
