using System;
using System.Collections;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using TMPro;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal class TutorialDialogueTypewriter
    {

        private readonly MonoBehaviour _owner;
        private readonly TextMeshProUGUI _label;
        private readonly Func<float> _charactersPerSecond;
        private Coroutine _typewriterCoroutine;
        public bool IsTyping { get; private set; }

        public TutorialDialogueTypewriter(MonoBehaviour owner, TextMeshProUGUI label, Func<float> charactersPerSecond)
        {
            _owner = owner;
            _label = label;
            _charactersPerSecond = charactersPerSecond;
        }

        public void Start(string message)
        {
            Stop();

            _label.text = message ?? string.Empty;
            _label.maxVisibleCharacters = 0;
            _label.ForceMeshUpdate();
            int characterCount = _label.textInfo.characterCount;
            if (characterCount <= 0)
            {
                _label.maxVisibleCharacters = int.MaxValue;
                return;
            }

            IsTyping = true;
            _typewriterCoroutine = _owner.StartCoroutine(PlayTypewriter(characterCount));
        }

        private IEnumerator PlayTypewriter(int characterCount)
        {
            float visibleCharacterProgress = 0f;
            int visibleCharacterCount = 0;

            while (visibleCharacterCount < characterCount)
            {
                visibleCharacterProgress += Time.unscaledDeltaTime
                                            * Mathf.Max(1f, _charactersPerSecond());
                int nextVisibleCharacterCount = Mathf.Min(
                    characterCount,
                    Mathf.FloorToInt(visibleCharacterProgress));
                if (nextVisibleCharacterCount > visibleCharacterCount)
                {
                    int previousVisibleCharacterCount = visibleCharacterCount;
                    visibleCharacterCount = nextVisibleCharacterCount;
                    _label.maxVisibleCharacters = visibleCharacterCount;

                    if (ContainsAudibleCharacter(
                            previousVisibleCharacterCount,
                            visibleCharacterCount))
                    {
                        Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                            SoundKeys.DialogueTyping,
                            SoundType.SFX));
                    }
                }

                yield return null;
            }

            _label.maxVisibleCharacters = int.MaxValue;
            IsTyping = false;
            _typewriterCoroutine = null;
        }

        private bool ContainsAudibleCharacter(int startIndex, int endIndex)
        {
            TMP_TextInfo textInfo = _label.textInfo;
            int clampedEndIndex = Mathf.Min(endIndex, textInfo.characterCount);
            for (int i = Mathf.Max(0, startIndex); i < clampedEndIndex; i++)
            {
                if (!char.IsWhiteSpace(textInfo.characterInfo[i].character))
                    return true;
            }

            return false;
        }

        public void Complete()
        {
            Stop();
            if (_label != null)
                _label.maxVisibleCharacters = int.MaxValue;
        }

        public void Stop()
        {
            if (_typewriterCoroutine != null)
            {
                _owner.StopCoroutine(_typewriterCoroutine);
                _typewriterCoroutine = null;
            }

            IsTyping = false;
        }
    }
}
