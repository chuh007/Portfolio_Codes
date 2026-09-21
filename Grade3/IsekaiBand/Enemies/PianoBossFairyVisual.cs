using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    /// <summary>
    /// Animation events still drive the ranged-enemy attack, while this component keeps
    /// the piano-fairy silhouette and gives the summoned variant a hostile presentation.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PianoBossFairyVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private Sprite fairySprite;
        [SerializeField] private Color hostileTint = new(1f, 0.18f, 0.12f, 1f);
        [SerializeField, Min(0f)] private float bobAmplitude = 0.08f;
        [SerializeField, Min(0f)] private float bobFrequency = 2.4f;
        [SerializeField, Range(0f, 0.5f)] private float pulseAmount = 0.12f;
        [SerializeField] private bool centerSpriteOnTransform = true;

        private Vector3 _restLocalPosition;
        private Vector3 _restLocalScale;
        private float _phaseOffset;

        private void Awake()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<SpriteRenderer>();

            _restLocalPosition = transform.localPosition;
            _restLocalScale = transform.localScale;
            _phaseOffset = Random.value * Mathf.PI * 2f;
        }

        private void OnEnable()
        {
            ApplyVisual(0f);
        }

        private void LateUpdate()
        {
            ApplyVisual(Time.time * bobFrequency + _phaseOffset);
        }

        private void ApplyVisual(float phase)
        {
            if (targetRenderer == null)
                return;

            if (fairySprite != null)
                targetRenderer.sprite = fairySprite;

            float alpha = targetRenderer.color.a;
            float pulse = 1f + Mathf.Sin(phase) * pulseAmount;
            targetRenderer.color = new Color(
                Mathf.Clamp01(hostileTint.r * pulse),
                Mathf.Clamp01(hostileTint.g * pulse),
                Mathf.Clamp01(hostileTint.b * pulse),
                alpha);

            Vector3 visualScale = _restLocalScale
                                  * (1f + Mathf.Sin(phase * 0.5f) * pulseAmount * 0.25f);
            transform.localScale = visualScale;

            Vector3 centerOffset = Vector3.zero;
            if (centerSpriteOnTransform && targetRenderer.sprite != null)
            {
                Vector3 scaledSpriteCenter = Vector3.Scale(targetRenderer.sprite.bounds.center, visualScale);
                centerOffset = transform.localRotation * scaledSpriteCenter;
            }

            transform.localPosition = _restLocalPosition
                                      - centerOffset
                                      + Vector3.up * (Mathf.Sin(phase) * bobAmplitude);
        }
    }
}
