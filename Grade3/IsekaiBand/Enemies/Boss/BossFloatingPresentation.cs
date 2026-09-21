using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    public sealed class BossFloatingPresentation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform shadowTransform;
        [SerializeField] private SpriteRenderer shadowRenderer;

        [Header("Float")]
        [SerializeField, Min(0f)] private float floatAmplitude = 0.12f;
        [SerializeField, Min(0.01f)] private float cycleDuration = 1.8f;
        [SerializeField, Min(0f)] private float positionStep = 0.01f;

        [Header("Shadow At Highest Point")]
        [SerializeField, Range(0f, 1f)] private float shadowScaleRatio = 0.82f;
        [SerializeField, Range(0f, 1f)] private float shadowAlphaRatio = 0.55f;

        private SpriteRenderer _visualRenderer;
        private Vector3 _baseVisualPosition;
        private Vector3 _baseShadowPosition;
        private Vector3 _baseShadowScale;
        private Color _baseShadowColor;
        private float _baseVisualAlpha;
        private float _elapsedTime;
        private bool _baseStateCached;

        private void Awake()
        {
            CacheBaseState();
        }

        private void OnEnable()
        {
            CacheBaseState();
            _elapsedTime = 0f;
            ApplyPresentation(0f);
        }

        private void LateUpdate()
        {
            _elapsedTime += Time.deltaTime;
            float phase = _elapsedTime / cycleDuration * Mathf.PI * 2f;
            ApplyPresentation(Mathf.Sin(phase));
        }

        private void OnDisable()
        {
            RestoreBaseState();
        }

        private void CacheBaseState()
        {
            if (_baseStateCached)
                return;

            _visualRenderer = GetComponent<SpriteRenderer>();
            _baseVisualPosition = transform.localPosition;
            _baseVisualAlpha = _visualRenderer != null ? _visualRenderer.color.a : 1f;

            if (shadowTransform != null)
            {
                _baseShadowPosition = shadowTransform.localPosition;
                _baseShadowScale = shadowTransform.localScale;
            }

            if (shadowRenderer != null)
                _baseShadowColor = shadowRenderer.color;

            _baseStateCached = true;
        }

        private void ApplyPresentation(float wave)
        {
            float heightOffset = SnapPosition(wave * floatAmplitude);
            float normalizedHeight = (wave + 1f) * 0.5f;

            transform.localPosition = _baseVisualPosition + Vector3.up * heightOffset;

            if (shadowTransform != null)
            {
                float visualScaleY = transform.localScale.y;
                float shadowCompensation = Mathf.Abs(visualScaleY) > Mathf.Epsilon
                    ? heightOffset / visualScaleY
                    : 0f;

                shadowTransform.localPosition = _baseShadowPosition - Vector3.up * shadowCompensation;

                float scaleRatio = Mathf.Lerp(1f, shadowScaleRatio, normalizedHeight);
                shadowTransform.localScale = new Vector3(
                    _baseShadowScale.x * scaleRatio,
                    _baseShadowScale.y * scaleRatio,
                    _baseShadowScale.z);
            }

            if (shadowRenderer != null)
            {
                float fadeRatio = GetCurrentFadeRatio();
                Color shadowColor = _baseShadowColor;
                shadowColor.a *= Mathf.Lerp(1f, shadowAlphaRatio, normalizedHeight) * fadeRatio;
                shadowRenderer.color = shadowColor;
            }
        }

        private float GetCurrentFadeRatio()
        {
            if (_visualRenderer == null || _baseVisualAlpha <= Mathf.Epsilon)
                return 1f;

            return Mathf.Clamp01(_visualRenderer.color.a / _baseVisualAlpha);
        }

        private float SnapPosition(float position)
        {
            return positionStep > Mathf.Epsilon
                ? Mathf.Round(position / positionStep) * positionStep
                : position;
        }

        private void RestoreBaseState()
        {
            if (!_baseStateCached)
                return;

            transform.localPosition = _baseVisualPosition;

            if (shadowTransform != null)
            {
                shadowTransform.localPosition = _baseShadowPosition;
                shadowTransform.localScale = _baseShadowScale;
            }

            if (shadowRenderer != null)
                shadowRenderer.color = _baseShadowColor;
        }
    }
}
