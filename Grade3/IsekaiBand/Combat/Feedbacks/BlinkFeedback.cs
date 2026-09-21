using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Feedbacks
{
    public class BlinkFeedback : Feedback, IPlayerLoopItem
    {
        private const float BossMinimumInterval = 0.2f;

        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private float delaySeconds;
        [SerializeField] private float blinkValue;
        
        private readonly int _blinkShaderParam = Shader.PropertyToID("_BlinkValue");
        private Material _material;
        private bool _scheduled;
        private bool _blinking;
        private bool _destroyed;
        private bool _hasBlinkProperty;
        private float _elapsed;
        private float _blinkDuration;
        private int _initialFrame;
        private bool _isBoss;
        private float _nextAllowedBossBlinkTime;

        private void Awake()
        {
            _isBoss = GetComponentInParent<Enemy>()?.IsBoss == true;
        }
        
        public override void CreateFeedback()
        {
            if (_isBoss)
            {
                if (Time.time < _nextAllowedBossBlinkTime) return;
                _nextAllowedBossBlinkTime = Time.time + BossMinimumInterval;
            }

            _blinking = true;
            _elapsed = 0f;
            _blinkDuration = (float)System.TimeSpan.FromSeconds(delaySeconds).TotalSeconds;
            _initialFrame = Time.frameCount;
            SetBlinkValue(blinkValue);
            if (!_scheduled)
            {
                _scheduled = true;
                PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, this);
            }
        }

        bool IPlayerLoopItem.MoveNext()
        {
            if (_destroyed || !_blinking)
            {
                _scheduled = false;
                return false;
            }
            // UniTask.Delay와 같은 시간 배율 및 첫 프레임 처리를 유지한다.
            if (_elapsed == 0f && _initialFrame == Time.frameCount) return true;
            _elapsed += Time.deltaTime;
            if (_elapsed < _blinkDuration) return true;
            FinishFeedback();
            _scheduled = false;
            return false;
        }

        public override void FinishFeedback()
        {
            _blinking = false;
            SetBlinkValue(0);
        }

        private void OnDestroy()
        {
            _destroyed = true;
            _blinking = false;
            if (_material != null) Destroy(_material);
        }

        private void SetBlinkValue(float value)
        {
            Material material = ResolveMaterial();
            if (material != null && _hasBlinkProperty)
                material.SetFloat(_blinkShaderParam, value);
        }

        private Material ResolveMaterial()
        {
            if (targetRenderer == null)
                return null;

            if (_material != null && targetRenderer.sharedMaterial == _material)
                return _material;
            Material material = targetRenderer.material;
            if (_material != null && _material != material)
                Destroy(_material);

            _material = material;
            _hasBlinkProperty = material != null && material.HasProperty(_blinkShaderParam);
            return _material;
        }
    }
}
