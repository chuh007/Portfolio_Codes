using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class FloatFruit : MonoBehaviour
{
    public UnityEvent EatEvent;

    private Sequence doTweenSequence;
    private SpriteRenderer sprite;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        doTweenSequence = DOTween.Sequence();
        doTweenSequence
            .Append(transform.DOLocalMoveY(transform.localPosition.y+0.05f, 0.5f).SetEase(Ease.InSine))
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().playerInput.RockInput(true);
            doTweenSequence.Kill();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(collision.transform.position, 0.25f))
                .Append(sprite.DOFade(0f,0.5f))
                .OnComplete(() => EatEvent?.Invoke());
        }
    }
}
