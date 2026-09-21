using System;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    [Serializable]
    public struct InstrumentBgmStem
    {
        public WeaponType instrument;
        public AudioClip clip;
        public AudioClip[] incompleteClips;
        [Min(0f)] public float volume;

        public float GetVolume(int collectedParts, float volumePerUpgrade)
            => Mathf.Max(0f, volume) + Mathf.Max(0f, volumePerUpgrade)
               * Mathf.Clamp(collectedParts, 0, InstrumentPartRules.RequiredPartCount);

        public AudioClip GetClip(int collectedParts)
            => incompleteClips == null || incompleteClips.Length == 0
               || collectedParts >= InstrumentPartRules.RequiredPartCount
                ? clip
                : incompleteClips[Mathf.Max(0, collectedParts)];
    }

    [CreateAssetMenu(fileName = "InstrumentBgm", menuName = "SO/Audio/Instrument BGM")]
    public class InstrumentBgmSO : ScriptableObject
    {
        [SerializeField] private string soundKey = SoundKeys.InGameBgm;
        [SerializeField, Min(0f)] private float transitionDuration = 0.35f;
        [SerializeField, Range(0f, 1f)] private float soloVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float fullBandVolume = 1f;
        [SerializeField, Min(0f)] private float volumePerUpgrade = 0.1f;
        [SerializeField] private AudioMixerGroup output;
        [SerializeField] private InstrumentBgmStem[] stems = Array.Empty<InstrumentBgmStem>();

        public float TransitionDuration => Mathf.Max(0f, transitionDuration);
        public int Count => stems?.Length ?? 0;
        public float VolumePerUpgrade => Mathf.Max(0f, volumePerUpgrade);
        public AudioMixerGroup Output => output;
        public InstrumentBgmStem GetStem(int index) => stems[index];
        public bool Matches(string key) => !string.IsNullOrWhiteSpace(soundKey) && soundKey == key;
        public float GetEnsembleVolume(float audibleCount)
            => Mathf.Lerp(soloVolume, fullBandVolume, (audibleCount - 1f) / 4f);

        public float GetMaximumVolume()
        {
            float maximum = 1f;
            for (int i = 0; i < Count; i++)
                maximum = Mathf.Max(maximum, stems[i].GetVolume(InstrumentPartRules.RequiredPartCount, VolumePerUpgrade));
            return maximum;
        }

        public bool ValidateClips()
        {
            if (Count == 0) return false;

            AudioClip first = stems[0].clip;
            if (first == null || first.samples <= 0) return false;

            int instruments = 0;
            foreach (InstrumentBgmStem stem in stems)
            {
                int bit = InstrumentBgmOwnership.GetInstrumentBit(stem.instrument);
                if (bit == 0 || (instruments & bit) != 0 || stem.clip == null
                    || !MatchesFormat(stem.clip, first))
                    return false;

                if (stem.incompleteClips != null && stem.incompleteClips.Length > 0)
                {
                    if (stem.incompleteClips.Length != InstrumentPartRules.RequiredPartCount) return false;
                    foreach (AudioClip incomplete in stem.incompleteClips)
                    {
                        if (!MatchesFormat(incomplete, first)) return false;
                    }
                }

                instruments |= bit;
            }
            return true;
        }

        private static bool MatchesFormat(AudioClip clip, AudioClip first)
            => clip != null && clip.samples == first.samples && clip.frequency == first.frequency
               && clip.channels == first.channels;
    }
}
