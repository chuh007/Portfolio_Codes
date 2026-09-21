using _Code.LCH._02.Scripts.UI;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal class TutorialDialogueLayout
    {
        private const float PanelTopMargin = 84f;
        private const float PanelBottomMargin = 54f;
        private const float ObjectiveRightMargin = 54f;
        private readonly TutorialDialogueElements _view;
        private readonly TutorialDialogueView _owner;

        public TutorialDialogueLayout(TutorialDialogueElements view, TutorialDialogueView owner)
        {
            _view = view;
            _owner = owner;
        }

        public void ApplyDialogue(bool placeAtBottom, bool showPortrait)
        {
            _view.FullBodyRect.gameObject.SetActive(false);
            Vector2 anchor = new(0.5f, placeAtBottom ? 0f : 1f);
            float offset = placeAtBottom ? PanelBottomMargin : -PanelTopMargin;
            Position(_view.PanelRect, anchor, new Vector2(0f, offset), new Vector2(1560f, 310f));
            _view.PortraitRect.gameObject.SetActive(showPortrait);
            Position(_view.PortraitRect, Vector2.zero, new Vector2(44f, 20f), new Vector2(286f, 286f));
            ApplyDialogueText(showPortrait);
        }

        public void ApplyFullBody(bool showCharacter, DialogueCharacterSide characterSide)
        {
            Position(_view.PanelRect, new Vector2(0.5f, 0f), new Vector2(0f, PanelBottomMargin), new Vector2(1560f, 310f));
            _view.PortraitRect.gameObject.SetActive(false);
            _view.FullBodyRect.gameObject.SetActive(showCharacter && _view.FullBodyImage.sprite != null);
            bool isRight = characterSide == DialogueCharacterSide.Right;
            Vector2 anchor = isRight ? new Vector2(1f, 0f) : Vector2.zero;
            Vector2 offset = isRight ? _owner.RightFullBodyOffset : _owner.LeftFullBodyOffset;
            Position(_view.FullBodyRect, anchor, offset, _owner.FullBodySize);
            ApplyDialogueText(false);
        }

        private void ApplyDialogueText(bool showPortrait)
        {
            Vector2 nameOffset = showPortrait ? new Vector2(324f, -42f) : new Vector2(52f, -42f);
            Position(_view.NameLabel.rectTransform, new Vector2(0f, 1f), nameOffset, new Vector2(900f, 42f));
            Vector2 messageOffset = showPortrait ? new Vector2(142f, -10f) : new Vector2(0f, -10f);
            Vector2 messageSize = showPortrait ? new Vector2(-560f, -104f) : new Vector2(-280f, -104f);
            Stretch(_view.MessageLabel.rectTransform, messageOffset, messageSize);
            Position((RectTransform)_view.ContinueButton.transform, new Vector2(1f, 0f), new Vector2(-44f, 34f), new Vector2(190f, 60f));
            _view.NameLabel.fontSize = 25f;
            _view.MessageLabel.fontSize = 25f;
        }

        public void ApplyObjective()
        {
            _view.FullBodyRect.gameObject.SetActive(false);
            _view.PortraitRect.gameObject.SetActive(false);
            Position(_view.PanelRect, Vector2.one, new Vector2(-ObjectiveRightMargin, -PanelTopMargin), new Vector2(580f, 168f));
            _view.NameLabel.text = "튜토리얼 목표";
            Position(_view.NameLabel.rectTransform, new Vector2(0f, 1f), new Vector2(30f, -23f), new Vector2(520f, 32f));
            Stretch(_view.MessageLabel.rectTransform, new Vector2(0f, -23f), new Vector2(-60f, -76f));
            _view.NameLabel.fontSize = 20f;
            _view.MessageLabel.fontSize = 21f;
        }

        private static void Position(RectTransform rect, Vector2 anchor, Vector2 offset, Vector2 size)
        {
            RuntimeUGuiFactory.SetRect(rect, anchor, anchor, anchor, offset, size);
        }

        private static void Stretch(RectTransform rect, Vector2 offset, Vector2 size)
        {
            RuntimeUGuiFactory.SetRect(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), offset, size);
        }
    }
}
