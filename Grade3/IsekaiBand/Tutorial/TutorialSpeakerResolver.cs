using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    public enum DialogueCharacterSide { Left, Right }
    public enum TutorialSpeaker { Piano, Vocal, Drum, Bass, Guitar }

    internal class TutorialSpeakerResolver
    {
        private readonly TutorialDialogueElements _view;
        private readonly TutorialDialogueView _owner;

        public TutorialSpeakerResolver(TutorialDialogueElements view, TutorialDialogueView owner)
        {
            _view = view;
            _owner = owner;
        }

        public void Apply(TutorialSpeaker speaker)
        {
            string displayName = speaker == TutorialSpeaker.Piano && !string.IsNullOrWhiteSpace(_owner.GuideName)
                ? _owner.GuideName : GetDefaultDisplayName(speaker);
            Sprite portrait = _owner.GuidePortrait;
            if (_owner.SpeakerVisuals != null)
            {
                foreach (TutorialDialogueView.SpeakerVisual visual in _owner.SpeakerVisuals)
                {
                    if (visual.speaker != speaker) continue;
                    if (!string.IsNullOrWhiteSpace(visual.displayName)) displayName = visual.displayName;
                    if (visual.portrait != null) portrait = visual.portrait;
                    break;
                }
            }
            _view.NameLabel.text = displayName;
            _view.PortraitImage.sprite = portrait;
        }

        public void Apply(string displayName, Sprite portrait)
        {
            _view.NameLabel.text = string.IsNullOrWhiteSpace(displayName) ? _owner.GuideName : displayName;
            Sprite sprite = portrait != null ? portrait : _owner.GuidePortrait;
            _view.PortraitImage.sprite = sprite;
            _view.FullBodyImage.sprite = sprite;
        }

        private static string GetDefaultDisplayName(TutorialSpeaker speaker)
        {
            return speaker switch
            {
                TutorialSpeaker.Piano => "조마리",
                TutorialSpeaker.Vocal => "백화연(보컬)",
                TutorialSpeaker.Drum => "손시은(드럼)",
                TutorialSpeaker.Bass => "안청(베이스)",
                TutorialSpeaker.Guitar => "정예솔(일렉기타)",
                _ => string.Empty
            };
        }
    }
}
