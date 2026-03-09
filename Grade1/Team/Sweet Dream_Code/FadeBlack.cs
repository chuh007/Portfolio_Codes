using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeBlack : MonoBehaviour
{
    private Image _image;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    public void FadeAndNextScene(bool inOut)
    {
        Sequence sequence = DOTween.Sequence();
        if (inOut)
        {
            sequence.Append(_image.DOFade(1, 1.6f)).AppendInterval(0.5f);
            sequence.OnComplete(() =>
            {
                KillDot();
                GameManager.Instance.NextSceneLoad();
            });
        }
        else
        {
            sequence.Append(_image.DOFade(0, 1.6f));
        }
    }
    public void Fade(bool inOut)
    {
        Sequence sequence = DOTween.Sequence();
        if (inOut)
        {
            _image.color = new Color(0, 0, 0, 0);
            sequence.Append(_image.DOFade(1, 1.6f));
        }
        else
        {
            _image.color = new Color(0, 0, 0, 1);
            sequence.Append(_image.DOFade(0, 1.6f));
        }
    }

    public void KillDot()
    {
        DOTween.KillAll();
    }
}
