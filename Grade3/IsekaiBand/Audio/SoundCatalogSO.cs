using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    [Serializable]
    public struct SoundCatalogEntry
    {
        [SerializeField] private string key;
        [SerializeField] private AudioClip clip;
        [SerializeField, Range(0f, 1f)] private float volume;
        [SerializeField] private bool preload;
        [SerializeField, Min(0f)] private float minimumPlaybackInterval;
        [SerializeField, Min(0)] private int maxSimultaneousVoices;

        public string Key => key;
        public AudioClip Clip => clip;
        public float Volume => volume;
        public bool Preload => preload;
        public float MinimumPlaybackInterval => minimumPlaybackInterval;
        public int MaxSimultaneousVoices => maxSimultaneousVoices;
    }

    [CreateAssetMenu(fileName = "SoundCatalog", menuName = "SO/Audio/Sound Catalog")]
    public sealed class SoundCatalogSO : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup sfxOutput;
        [SerializeField] private AudioMixerGroup bgmOutput;
        [SerializeField] private SoundCatalogEntry[] entries = Array.Empty<SoundCatalogEntry>();

        private Dictionary<string, SoundCatalogEntry> _lookup;

        public AudioMixerGroup SfxOutput => sfxOutput;
        public AudioMixerGroup BgmOutput => bgmOutput;

        public bool TryGet(string soundKey, out SoundCatalogEntry entry)
        {
            EnsureLookup();
            if (string.IsNullOrWhiteSpace(soundKey))
            {
                entry = default;
                return false;
            }

            return _lookup.TryGetValue(soundKey, out entry);
        }

        public void PreloadMarkedClips()
        {
            for (int i = 0; i < entries.Length; i++)
            {
                SoundCatalogEntry entry = entries[i];
                if (entry.Preload && entry.Clip != null)
                    entry.Clip.LoadAudioData();
            }
        }

        private void OnEnable()
        {
            _lookup = null;
        }

        private void OnValidate()
        {
            _lookup = null;
        }

        private void EnsureLookup()
        {
            if (_lookup != null)
                return;

            _lookup = new Dictionary<string, SoundCatalogEntry>(StringComparer.Ordinal);
            for (int i = 0; i < entries.Length; i++)
            {
                SoundCatalogEntry entry = entries[i];
                if (string.IsNullOrWhiteSpace(entry.Key) || entry.Clip == null)
                    continue;

                _lookup[entry.Key] = entry;
            }
        }
    }
}
