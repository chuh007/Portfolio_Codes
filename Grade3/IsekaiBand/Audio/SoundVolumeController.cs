using System;
using _Work.CHUH.Code.Core.Persistence;
using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class SoundVolumeController
    {
        private const float MinimumAudibleVolume = 0.0001f;
        private const float MuteDecibels = -80f;
        private readonly AudioMixer _mixer;
        private float _masterVolume;
        private float _sfxVolume;
        private float _bgmVolume;

        public event Action OnVolumeChanged;
        public float SfxTrackVolume => _mixer != null ? 1f : _sfxVolume;
        public float BgmTrackVolume => _mixer != null ? 1f : _bgmVolume;

        public SoundVolumeController(SoundCatalogSO catalog)
        {
            if (catalog != null)
            {
                _mixer = catalog.SfxOutput != null
                    ? catalog.SfxOutput.audioMixer
                    : catalog.BgmOutput != null ? catalog.BgmOutput.audioMixer : null;
            }

            GameSettingsSnapshot settings = GameSettingsStore.GetSnapshot();
            _masterVolume = settings.MasterVolume;
            _sfxVolume = settings.SfxVolume;
            _bgmVolume = settings.BgmVolume;
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            GameSettingsStore.SetMasterVolume(_masterVolume);
            Apply();
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            GameSettingsStore.SetSfxVolume(_sfxVolume);
            Apply();
        }

        public void SetBgmVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp01(volume);
            GameSettingsStore.SetBgmVolume(_bgmVolume);
            Apply();
        }

        public void Apply()
        {
            if (_mixer != null)
            {
                AudioListener.volume = 1f;
                SetMixerVolume("Master", _masterVolume);
                SetMixerVolume("SFX", _sfxVolume);
                SetMixerVolume("BGM", _bgmVolume);
            }
            else
            {
                AudioListener.volume = _masterVolume;
            }
            OnVolumeChanged?.Invoke();
        }

        private void SetMixerVolume(string parameterName, float volume)
        {
            float decibels = volume <= MinimumAudibleVolume
                ? MuteDecibels
                : Mathf.Log10(volume) * 20f;
            _mixer.SetFloat(parameterName, decibels);
        }
    }
}
