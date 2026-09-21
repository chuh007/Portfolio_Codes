using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Visual
{
    public class PooledAnimationVFX : MonoBehaviour, IPoolable, IPooledVFX
    {
        [SerializeField] private PoolItemSO poolItem;
        [SerializeField] private Animator animator;
        [SerializeField] private float returnDelay = 1f;
        [SerializeField] private bool rotateToDirection;
        [SerializeField, Min(1)] private int playCount = 1;
        [SerializeField] private bool randomizeDirectionOnRepeat;

        private Pool _pool;
        private Vector3 _baseScale;
        private Vector2 _playDirection = Vector2.right;
        private float _remainingTime;
        private float _durationMultiplier = 1f;
        private int _remainingPlayCount;
        private bool _isPlaying;

        public PoolItemSO PoolItem => poolItem;

        private void Awake()
        {
            EnsureAnimator();
            _baseScale = transform.localScale;
        }

        public void ResetItem()
        {
            _remainingTime = 0f;
            _playDirection = Vector2.right;
            _durationMultiplier = 1f;
            _remainingPlayCount = 0;
            _isPlaying = false;

            if (animator != null)
                animator.speed = 1f;
        }

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
        }

        public void Play(Vector3 position, float scale, Vector2 direction)
        {
            Play(position, scale, direction, 1f);
        }

        public void Play(Vector3 position, float scale, Vector2 direction, float durationMultiplier)
        {
            EnsureAnimator();

            transform.position = position;
            transform.localScale = _baseScale * scale;
            _playDirection = direction;
            _durationMultiplier = Mathf.Max(0.01f, durationMultiplier);
            _remainingPlayCount = Mathf.Max(1, playCount);
            _isPlaying = true;

            PlayOnce(direction);
        }

        private void EnsureAnimator()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        private float RestartAnimator(out float stateLength)
        {
            stateLength = 0f;
            if (animator == null)
                return Mathf.Max(0.01f, returnDelay);

            animator.enabled = true;
            animator.speed = 1f;
            animator.Rebind();
            animator.Update(0f);

            if (animator.layerCount == 0)
                return Mathf.Max(0.01f, returnDelay);

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.fullPathHash == 0)
                return Mathf.Max(0.01f, returnDelay);

            stateLength = stateInfo.length;
            animator.Play(stateInfo.fullPathHash, 0, 0f);
            animator.Update(0f);
            return Mathf.Max(Mathf.Max(0.01f, returnDelay), stateLength);
        }

        private void PlayOnce(Vector2 direction)
        {
            transform.rotation = GetDirectionRotation(direction);

            float baseDuration = RestartAnimator(out float stateLength);
            int totalPlayCount = Mathf.Max(1, playCount);
            if (totalPlayCount > 1)
            {
                _remainingTime = Mathf.Max(0.01f, baseDuration * _durationMultiplier / totalPlayCount);
                if (animator != null && stateLength > Mathf.Epsilon)
                    animator.speed = stateLength / _remainingTime;
            }
            else
            {
                _remainingTime = baseDuration * _durationMultiplier;
                if (animator != null)
                    animator.speed /= _durationMultiplier;
            }
        }

        private static Vector2 GetRandomDirection()
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
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

            _remainingTime -= Time.deltaTime;
            if (_remainingTime > 0f) return;

            _remainingPlayCount--;
            if (_remainingPlayCount > 0)
            {
                if (randomizeDirectionOnRepeat)
                    _playDirection = GetRandomDirection();

                PlayOnce(_playDirection);
                return;
            }

            _isPlaying = false;
            _pool?.Push(this);
        }
    }
}
