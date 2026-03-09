using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour, ISwitchable
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveRange = 5f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector2 StartPos;

    private bool isActived = false;
    private void Awake()
    {
        StartPos = transform.localPosition;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        if (isActived) return;
        Debug.Log("작동");
        transform.DOMove(transform.position + transform.up * moveRange, duration);
        isActived = true;
    }
    public void Deactivate()
    {
        if (!isActived) return;
        Debug.Log("작동 해제");
        transform.DOMove(StartPos, duration);
        isActived = false;
    }
}
