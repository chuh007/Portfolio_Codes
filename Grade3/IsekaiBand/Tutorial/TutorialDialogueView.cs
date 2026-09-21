using TMPro;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    [DisallowMultipleComponent]
    public sealed class TutorialDialogueView : MonoBehaviour
    {
        [System.Serializable]
        internal struct SpeakerVisual
        {
            public TutorialSpeaker speaker;
            public string displayName;
            public Sprite portrait;
        }

        [SerializeField] private TMP_FontAsset fontAsset;
        [SerializeField] private TMP_FontAsset buttonFontAsset;
        [SerializeField] private Sprite settingsFrameSprite;
        [SerializeField] private Sprite guidePortrait;
        [SerializeField] private string guideName = "조마리";
        [SerializeField] private SpeakerVisual[] speakerVisuals;
        [SerializeField] private Vector2 fullBodySize = new(680f, 880f);
        [SerializeField] private Vector2 leftFullBodyOffset = new(80f, 100f);
        [SerializeField] private Vector2 rightFullBodyOffset = new(-80f, 100f);
        [SerializeField, Min(1f)] private float typewriterCharactersPerSecond = 25f;

        private TutorialDialoguePresenter _presenter;
        internal string GuideName => guideName;
        internal Sprite GuidePortrait => guidePortrait;
        internal SpeakerVisual[] SpeakerVisuals => speakerVisuals;
        internal Vector2 FullBodySize => fullBodySize;
        internal Vector2 LeftFullBodyOffset => leftFullBodyOffset;
        internal Vector2 RightFullBodyOffset => rightFullBodyOffset;
        public bool AdvanceRequested => _presenter != null && _presenter.AdvanceRequested;

        private void Awake()
        {
            TutorialDialogueElements elements = TutorialDialogueBuilder.Build(
                transform, fontAsset, buttonFontAsset, settingsFrameSprite, guidePortrait, guideName);
            _presenter = new TutorialDialoguePresenter(elements, this, () => typewriterCharactersPerSecond);
            Hide();
        }

        public void ShowDialogue(string message, string buttonText = "다음", bool blockGameplayUi = true)
            => ShowDialogue(TutorialSpeaker.Piano, message, buttonText, blockGameplayUi);

        public void ShowDialogue(TutorialSpeaker speaker, string message, string buttonText = "다음",
            bool blockGameplayUi = true)
            => _presenter.ShowDialogue(speaker, message, buttonText, false, blockGameplayUi);

        public void ShowBottomDialogue(TutorialSpeaker speaker, string message, string buttonText = "다음")
            => _presenter.ShowDialogue(speaker, message, buttonText, true, true);

        public void ShowBottomDialogue(string displayName, Sprite characterSprite, string message,
            string buttonText = "다음", bool showCharacter = true,
            DialogueCharacterSide characterSide = DialogueCharacterSide.Left)
            => _presenter.ShowFullBody(displayName, characterSprite, message, buttonText, showCharacter, characterSide);

        public void ShowObjective(string message) => ShowObjective(TutorialSpeaker.Piano, message);
        public void ShowObjective(TutorialSpeaker speaker, string message) => _presenter.ShowObjective(speaker, message);
        public void Hide() => _presenter?.Hide();
        internal System.Action CapturePresentation() => _presenter.CapturePresentation();
        private void OnDisable() => Hide();
        private void OnDestroy() => _presenter?.Dispose();
    }
}
