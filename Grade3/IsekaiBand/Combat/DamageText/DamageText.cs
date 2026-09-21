using Chuh007Lib.ObjectPool.RunTime;
using TMPro;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Combat.DamageText
{
    [System.Serializable]
    public struct DamageTextGradient
    {
        public Color topColor;
        public Color bottomColor;

        public DamageTextGradient(Color topColor, Color bottomColor)
        {
            this.topColor = topColor;
            this.bottomColor = bottomColor;
        }

        public VertexGradient ToVertexGradient()
        {
            return new VertexGradient(topColor, topColor, bottomColor, bottomColor);
        }
    }

    public class DamageText : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshPro damageText;
        [SerializeField, Min(0.01f)] private float lifeTime = 1f;
        [SerializeField] private string sortingLayerName = "UI";
        [SerializeField] private int sortingOrder = 100;
        [SerializeField] private float startScale = 0.15f;
        [SerializeField] private float peakScale = 1.35f;
        [SerializeField] private float endScale = 0.9f;
        [SerializeField, Range(0.01f, 0.99f)] private float peakTime = 0.25f;
        [SerializeField, Min(0f)] private float peakHoldTime = 0.25f;

        private float _currentLifeTime;
        private MeshRenderer _meshRenderer;
        private Transform _textTransform;
        private Vector3 _defaultTextLocalPosition;
        private Vector3 _defaultTextLocalScale;
        private Color _defaultColor;
        private VertexGradient _defaultGradient;
        private bool _defaultEnableVertexGradient;
        private float _scaleMultiplier = 1f;
        private readonly char[] _damageCharacters = new char[16];
        private DamageTextManager _displayOwner;

        private void Awake()
        {
            _textTransform = damageText.transform;
            _meshRenderer = damageText.GetComponent<MeshRenderer>();
            _defaultTextLocalPosition = _textTransform.localPosition;
            _defaultTextLocalScale = _textTransform.localScale;
            _defaultColor = damageText.color;
            _defaultGradient = damageText.colorGradient;
            _defaultEnableVertexGradient = damageText.enableVertexGradient;
            _meshRenderer.sortingLayerName = sortingLayerName;
            _meshRenderer.sortingOrder = sortingOrder;
        }
        
        public void SetText(float damage)
        {
            SetText(damage, _defaultGradient, _defaultEnableVertexGradient, 1f);
        }

        public void SetText(float damage, DamageTextGradient gradient, float scaleMultiplier = 1f)
        {
            SetText(damage, gradient.ToVertexGradient(), true, scaleMultiplier);
        }

        private void SetText(float damage, VertexGradient gradient, bool useGradient, float scaleMultiplier)
        {
            int damageValue = (int)damage;
            if (damageValue.TryFormat(_damageCharacters, out int length))
                damageText.SetCharArray(_damageCharacters, 0, length);
            else
                damageText.text = damageValue.ToString();
            damageText.color = Color.white;
            damageText.enableVertexGradient = useGradient;
            damageText.colorGradient = gradient;
            _scaleMultiplier = Mathf.Max(0f, scaleMultiplier);
            _currentLifeTime = 0f;
            ResetTransform();
            ApplyScale(0f, peakTime);
        }

        internal void TrackDisplay(DamageTextManager owner)
        {
            ReleaseDisplay();
            _displayOwner = owner;
        }

        private void OnDisable()
        {
            // Hiding the pool parent does not return its texts to the pool.
            if (!gameObject.activeSelf)
                ReleaseDisplay();
        }

        private void OnDestroy()
        {
            ReleaseDisplay();
        }

        private void ReleaseDisplay()
        {
            if (_displayOwner != null)
                _displayOwner.ReleaseText();
            _displayOwner = null;
        }

        private void Update()
        {
            _currentLifeTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(_currentLifeTime / lifeTime);
            float peakEndTime = Mathf.Min(peakTime + peakHoldTime / lifeTime, 0.99f);
            
            Color color = damageText.color;
            color.a = normalizedTime <= peakEndTime
                ? 1f
                : 1f - (normalizedTime - peakEndTime) / (1f - peakEndTime);
            damageText.color = color;

            ApplyScale(normalizedTime, peakEndTime);
            if (_currentLifeTime >= lifeTime)
                _myPool.Push(this);
        }

        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        
        private Pool _myPool;
        
        public void ResetItem()
        {
            ResetTransform();
            damageText.text = string.Empty;
            _scaleMultiplier = 1f;
            Color color = _defaultColor;
            color.a = 1f;
            damageText.color = color;
            damageText.enableVertexGradient = _defaultEnableVertexGradient;
            damageText.colorGradient = _defaultGradient;
        }
        
        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        private void ResetTransform()
        {
            _textTransform.localPosition = _defaultTextLocalPosition;
            _textTransform.localScale = _defaultTextLocalScale;
        }

        private void ApplyScale(float normalizedTime, float peakEndTime)
        {
            float scale = normalizedTime <= peakTime
                ? startScale + (peakScale - startScale) * (normalizedTime / peakTime)
                : normalizedTime <= peakEndTime
                    ? peakScale
                    : peakScale + (endScale - peakScale) * ((normalizedTime - peakEndTime) / (1f - peakEndTime));

            _textTransform.localScale = _defaultTextLocalScale * (scale * _scaleMultiplier);
        }
    }
}
