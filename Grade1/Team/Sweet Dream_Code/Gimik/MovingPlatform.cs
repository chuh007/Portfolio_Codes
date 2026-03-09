using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private bool isCanMoveing;

    [Header("Move Setting(local pos)")]
    [Space]
    [SerializeField] private bool isStartingToStart;
    [Space]

    [SerializeField] private Vector2 moveStartPos;
    [SerializeField] private Vector2 moveEndPos;
    [SerializeField] private float waitTime;
    [SerializeField] private float _duration;

    private Sequence _Sequnce;
    private void Start()
    {
        if (isStartingToStart) Moveing();
    }
    public void Moveing()
    {
        if (!isCanMoveing) return;
        _Sequnce = DOTween.Sequence();
        _Sequnce.AppendInterval(waitTime).
            Append(transform.DOLocalMove(transform.position + (Vector3)moveEndPos, _duration).SetEase(Ease.Linear)).
            AppendInterval(waitTime);
        _Sequnce.SetLoops(-1, LoopType.Yoyo);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D con = collision.contacts[0];
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && con.normal.y < 0.1f)
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
public enum MoveType
{
    LeftRight, UpDown
}