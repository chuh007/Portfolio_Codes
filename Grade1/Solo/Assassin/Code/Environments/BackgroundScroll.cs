using System;
using UnityEngine;

namespace Code.Environments
{
    public class BackgroundScroll : MonoBehaviour
    {
        [SerializeField] private float _parallaxOffset;
        private SpriteRenderer _spriteRenderer;
        private Material _backgroundMaterial;

        private float _currentScroll;
        private float _ratio;
        private Transform _mainCamTrm;
        private float _beforeXPosition;
        
        private readonly int _offsetHash = Shader.PropertyToID("_Offset");

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _backgroundMaterial = _spriteRenderer.material;
            _currentScroll = 0;
            _ratio = 1f / _spriteRenderer.bounds.size.x;
            
            _mainCamTrm = Camera.main.transform;
        }

        private void Start()
        {
            _beforeXPosition = _mainCamTrm.position.x;
        }

        private void Update()
        {
            float delta = _mainCamTrm.position.x - _beforeXPosition;
            
            _beforeXPosition = _mainCamTrm.position.x;
            _currentScroll += delta * _parallaxOffset * _ratio;
            
            _backgroundMaterial.SetVector(_offsetHash, new Vector2(_currentScroll, 0));
        }
    }
}