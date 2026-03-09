using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using TMPro;

namespace _01Scripts.UI
{
    public class QTEObject : MonoBehaviour
    {
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private Image outlineImg;
        [SerializeField] private RectTransform inputRect;
        [SerializeField] private TextMeshProUGUI text;

        public event Action<QTEObject> OnFail;
        public event Action<QTEObject> OnSuccess;
        
        private RectTransform _rectTrm => transform as RectTransform;

        private CanvasGroup _canvasGroup;
        private bool _isEnd = false;
        
        private void Awake()
        {
            canvasRect = transform.parent.GetComponent<RectTransform>();
            float x = canvasRect.rect.size.x / 4;
            float y = canvasRect.rect.size.y / 4;
            _rectTrm.anchoredPosition = new Vector2(Random.Range(-x, x), Random.Range(-y, y));
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            outlineImg.fillAmount -= Time.deltaTime;
            inputRect.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
            if (outlineImg.fillAmount <= 0 && !_isEnd) IsQTESuccess();
        }

        public bool IsQTESuccess()
        {
            if (outlineImg.fillAmount is < 0.5f and > 0.25f)
            {
                _rectTrm.DOScale(1.5f, 0.2f).OnComplete(()=>
                {
                    text.text = "성공";
                    _rectTrm.DOScale(0.5f, 0.2f);
                    _canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
                    {
                        _rectTrm.DOKill();
                        _canvasGroup.DOKill();
                        Destroy(gameObject);
                        // OnSuccess?.Invoke(this);
                    });
                });
                _isEnd = true;
                return true;
            }

            _rectTrm.DOScale(1.5f, 0.5f);
            text.color = Color.red;
            text.text = "실패";
            _canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                _rectTrm.DOKill();
                _canvasGroup.DOKill();
                OnFail?.Invoke(this);
            });
            _isEnd = true;
            return false;
        }
    }
}