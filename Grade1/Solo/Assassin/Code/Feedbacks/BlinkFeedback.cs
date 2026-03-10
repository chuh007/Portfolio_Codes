using System;
using System.Collections;
using UnityEngine;

namespace Code.Feedbacks
{
    public class BlinkFeedback : Feedback
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private float delaySeconds;
        [SerializeField] private float blinkValue;
        
        private readonly int _blinkShaderParam = Shader.PropertyToID("_BlinkValue");
        private Material _material;
        private bool _isFinished;
        private Coroutine _delayCoroutine = null;

        private void Awake()
        {
            _material = targetRenderer.material;
        }

        private IEnumerator ResetAfterDelay()
        {
            _isFinished = false;
            yield return new WaitForSeconds(delaySeconds);
            
            if(_isFinished == false)
                FinishFeedback();
        }

        public override void CreateFeedback()
        {
            _material.SetFloat(_blinkShaderParam, blinkValue);
            _delayCoroutine = StartCoroutine(ResetAfterDelay());
        }

        public override void FinishFeedback()
        {
            if (_delayCoroutine != null)
            {
                StopCoroutine(_delayCoroutine);
            }

            _isFinished = true;
            _material.SetFloat(_blinkShaderParam, 0);
        }
    }
}