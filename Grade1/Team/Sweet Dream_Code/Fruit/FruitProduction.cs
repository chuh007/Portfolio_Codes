using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class FruitProduction : MonoBehaviour
{
    public UnityEvent EndEvent;

    [SerializeField] private Image fruitImage;
    [SerializeField] private Image backPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _TMIText;

    private void Start()
    {
        _nameText.enabled = false;
        _TMIText.enabled = false;
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }

    public void EatFruit()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(backPanel.DOFade(0.8f, 0.25f))
            .Append(fruitImage.rectTransform.DOScale(1f, 0.25f));
        sequence.OnComplete(() => 
        {
            _nameText.enabled = true;
            TMPDOText(_TMIText, 1f);
        });
    }

    private void TMPDOText(TextMeshProUGUI text, float duration)
    {
        text.maxVisibleCharacters = 0;
        _TMIText.enabled = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(x => text.maxVisibleCharacters = (int)x
            , 0f, text.text.Length, duration).SetEase(Ease.Linear))
            .AppendInterval(1f);
        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            EndEvent?.Invoke();
        });
    }
}
