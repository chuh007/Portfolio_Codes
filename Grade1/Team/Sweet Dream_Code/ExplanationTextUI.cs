using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExplanationTextUI : MonoBehaviour, IInteractable
    // 테스트 용 스크립트,  추후 수정 예정
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string TypingText;
    [SerializeField] private float TypingTime;
    private bool isExplanationUION = false;
    private BoxCollider2D ColCompo;
    private Player _player;

    private void Awake()
    {
        ColCompo = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        text.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player = collision.GetComponentInParent<Player>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isExplanationUION) HideExplanationText();
    }
    private void ShowExplanationText()
    {
        _player.playerInput.RockInput(true);
        Sequence sequence = DOTween.Sequence();
        text.text = TypingText;
        ColCompo.enabled = false;
        sequence.Append(image.DOFade(0.6f, 0.25f));

        sequence.OnComplete(() =>
        {
            text.enabled = true;
            isExplanationUION = true;
            TMPDOText(text, TypingTime);
        });
    }
    private void HideExplanationText()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0f, 0.25f));
        text.enabled = false;
        sequence.OnComplete(() =>
        {
            isExplanationUION = false;
        });
    }
    
    private void TMPDOText(TextMeshProUGUI text, float duration)
    {
        text.maxVisibleCharacters = 0;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(x => text.maxVisibleCharacters = (int)x, 0f, text.text.Length, duration).SetEase(Ease.Linear));
        sequence.OnComplete(() =>
        {
            _player.playerInput.RockInput(false);
            ColCompo.enabled = true;
        });
    }


    public void EndInteract()
    {

    }

    public void Interact()
    {
        if(!isExplanationUION)
        {
            ShowExplanationText();
        }
        else
        {
            HideExplanationText();
        }
    }
}
