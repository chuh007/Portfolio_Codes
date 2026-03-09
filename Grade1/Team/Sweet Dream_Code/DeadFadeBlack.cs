using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DeadFadeBlack : MonoBehaviour
{
    [SerializeField] private Image FadeImage;


    public void ImageBlackFade()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(FadeImage.DOFade(1, 0.7f)).
            AppendInterval(0.3f).
            OnComplete(() =>FadeImage.DOFade(0, 0.5f));
    }
}
