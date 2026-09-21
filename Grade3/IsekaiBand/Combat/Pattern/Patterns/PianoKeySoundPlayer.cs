using System;
using System.Collections.Generic;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    public sealed class PianoKeySoundPlayer : MonoBehaviour
    {
        [Serializable]
        private struct WhiteKeySound
        {
            [SerializeField] private string noteName;
            [SerializeField] private string soundKey;

            public string NoteName => noteName;
            public string SoundKey => soundKey;
        }

        [SerializeField] private WhiteKeySound[] whiteKeySounds = Array.Empty<WhiteKeySound>();
        [SerializeField, Min(0f)] private float volumeMultiplier = 1.5f;

        private readonly List<int> _randomNoteIndices = new();
        private readonly System.Random _random = new();

        public void Play(int keyIndex)
        {
            if (keyIndex < 0 || keyIndex >= whiteKeySounds.Length)
                return;

            string soundKey = whiteKeySounds[keyIndex].SoundKey;
            if (string.IsNullOrWhiteSpace(soundKey))
                return;

            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                soundKey,
                SoundType.SFX,
                volumeMultiplier));
        }

        public void PlayRandomChord(int noteCount)
        {
            _randomNoteIndices.Clear();
            for (int i = 0; i < whiteKeySounds.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(whiteKeySounds[i].SoundKey))
                    _randomNoteIndices.Add(i);
            }

            int actualNoteCount = Math.Min(Math.Max(0, noteCount), _randomNoteIndices.Count);
            for (int i = 0; i < actualNoteCount; i++)
            {
                int selectedIndex = _random.Next(i, _randomNoteIndices.Count);
                (_randomNoteIndices[i], _randomNoteIndices[selectedIndex]) =
                    (_randomNoteIndices[selectedIndex], _randomNoteIndices[i]);
                Play(_randomNoteIndices[i]);
            }
        }
    }
}
