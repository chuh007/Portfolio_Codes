using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal class TutorialDialoguePresenter
    {
        private readonly TutorialDialogueElements _view;
        private readonly TutorialSpeakerResolver _speaker;
        private readonly TutorialDialogueLayout _layout;
        private readonly TutorialDialogueTypewriter _typewriter;
        private PlayerMovementCompo _lockedMovement;
        private System.Action _restorePresentation;
        public bool AdvanceRequested { get; private set; }

        public TutorialDialoguePresenter(TutorialDialogueElements view, TutorialDialogueView owner, System.Func<float> typingSpeed)
        {
            _view = view;
            _speaker = new TutorialSpeakerResolver(view, owner);
            _layout = new TutorialDialogueLayout(view, owner);
            _typewriter = new TutorialDialogueTypewriter(owner, view.MessageLabel, typingSpeed);
            _view.ContinueButton.onClick.AddListener(HandleContinue);
        }

        public void ShowDialogue(TutorialSpeaker speaker, string message, string buttonText, bool placeAtBottom,
            bool blockGameplayUi)
        {
            _restorePresentation = () => ShowDialogue(speaker, message, buttonText, placeAtBottom, blockGameplayUi);
            PrepareDialogue(message, buttonText, blockGameplayUi);
            _speaker.Apply(speaker);
            _layout.ApplyDialogue(placeAtBottom, true);
        }

        public void ShowFullBody(string displayName, Sprite sprite, string message, string buttonText,
            bool showCharacter, DialogueCharacterSide characterSide)
        {
            _restorePresentation = () => ShowFullBody(displayName, sprite, message, buttonText, showCharacter, characterSide);
            PrepareDialogue(message, buttonText, true);
            _speaker.Apply(displayName, sprite);
            _layout.ApplyFullBody(showCharacter, characterSide);
        }

        private void PrepareDialogue(string message, string buttonText, bool blockGameplayUi)
        {
            LockPlayerInput();
            if (blockGameplayUi)
                GameplayUiBlockService.Acquire(this);
            else
                GameplayUiBlockService.Release(this);

            AdvanceRequested = false;
            _view.CanvasRoot.SetActive(true);
            _view.Blocker.SetActive(true);
            _view.ContinueButton.gameObject.SetActive(true);
            _view.ContinueLabel.text = buttonText;
            _view.PanelGroup.blocksRaycasts = true;
            _typewriter.Start(message);
        }

        public void ShowObjective(TutorialSpeaker speaker, string message)
        {
            _restorePresentation = () => ShowObjective(speaker, message);
            ReleasePlayerInput();
            GameplayUiBlockService.Release(this);
            _typewriter.Stop();
            AdvanceRequested = false;
            _view.CanvasRoot.SetActive(true);
            _view.Blocker.SetActive(false);
            _view.ContinueButton.gameObject.SetActive(false);
            _view.MessageLabel.text = $"- {message}";
            _view.MessageLabel.maxVisibleCharacters = int.MaxValue;
            _view.PanelGroup.blocksRaycasts = false;
            _speaker.Apply(speaker);
            _layout.ApplyObjective();
        }

        public void Hide()
        {
            _restorePresentation = null;
            ReleasePlayerInput();
            GameplayUiBlockService.Release(this);
            _typewriter.Stop();
            if (_view.CanvasRoot != null) _view.CanvasRoot.SetActive(false);
        }

        public System.Action CapturePresentation()
        {
            System.Action restore = _restorePresentation ?? Hide;
            bool wasTyping = _typewriter.IsTyping;
            bool advanceRequested = AdvanceRequested;
            return () =>
            {
                restore();
                if (!wasTyping) _typewriter.Complete();
                AdvanceRequested = advanceRequested;
            };
        }

        private void HandleContinue()
        {
            if (_typewriter.IsTyping)
            {
                _typewriter.Complete();
                return;
            }
            AdvanceRequested = true;
        }

        private void LockPlayerInput()
        {
            if (_lockedMovement != null) return;
            _lockedMovement = Object.FindAnyObjectByType<PlayerMovementCompo>();
            _lockedMovement?.LockInput();
        }

        private void ReleasePlayerInput()
        {
            if (_lockedMovement == null) return;
            _lockedMovement.UnlockInput();
            _lockedMovement = null;
        }

        public void Dispose()
        {
            Hide();
            if (_view.ContinueButton != null) _view.ContinueButton.onClick.RemoveListener(HandleContinue);
        }
    }
}
