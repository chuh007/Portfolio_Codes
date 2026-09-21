using System;
using UnityEngine;

namespace _Work.CHUH.Code.Core.Persistence
{
    public readonly struct GameSettingsSnapshot
    {
        public GameSettingsSnapshot(float masterVolume, float bgmVolume, float sfxVolume)
        {
            MasterVolume = masterVolume;
            BgmVolume = bgmVolume;
            SfxVolume = sfxVolume;
        }

        public float MasterVolume { get; }
        public float BgmVolume { get; }
        public float SfxVolume { get; }
    }

    [Serializable]
    internal sealed class GameSettingsSaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public float masterVolume = 1f;
        public float bgmVolume = 1f;
        public float sfxVolume = 1f;

        public void Normalize()
        {
            version = CurrentVersion;
            masterVolume = Mathf.Clamp01(masterVolume);
            bgmVolume = Mathf.Clamp01(bgmVolume);
            sfxVolume = Mathf.Clamp01(sfxVolume);
        }
    }
}
