using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeStart : MonoBehaviour
{
    [SerializeField] private Image Fade;
    void Start()
    {
        Fade.DOFade(0, 1);
    }
}
