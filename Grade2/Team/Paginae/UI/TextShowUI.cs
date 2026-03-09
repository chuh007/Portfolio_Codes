using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.UI
{
    public class TextShowUI : MonoBehaviour
    {
        [SerializeField] private GameObject textPrefab;

        private void Awake()
        {
            Bus<TextEvent>.OnEvent += HandleTextPrint;
        }

        private void OnDestroy()
        {
            Bus<TextEvent>.OnEvent -= HandleTextPrint;
        }

        private void HandleTextPrint(TextEvent evt)
        {
            GameObject obj = Instantiate(textPrefab, transform);
            TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
            text.text = evt.Text;
            obj.transform.DOMoveY(Camera.main.pixelHeight - 150f, 0.4f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                text.DOFade(0, 0.5f).OnComplete(() => Destroy(obj));
            });
        }
    }
}