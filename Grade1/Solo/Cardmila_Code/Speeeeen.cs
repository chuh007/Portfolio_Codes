using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Speeeeen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    private void Start()
    {
        transform.DOScale(transform.localScale*1.5f,2f).SetLoops(-1,LoopType.Yoyo);
        transform.DORotate(new Vector3(0, 0, -360), 8, RotateMode.LocalAxisAdd).SetLoops(-1,LoopType.Yoyo);
    }
}
