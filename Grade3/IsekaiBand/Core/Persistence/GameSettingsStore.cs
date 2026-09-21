using System;
using UnityEngine;

namespace _Work.CHUH.Code.Core.Persistence
{
    /// <summary>사용자 설정의 캐시 및 변경 상태를 관리한다.</summary>
    public static class GameSettingsStore
    {
        private static GameSettingsSaveData _data;
        private static bool _isLoaded;
        private static bool _isDirty;

        public static bool HasUnsavedChanges => _isDirty;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntimeState() => ResetCachedData();

        public static GameSettingsSnapshot GetSnapshot()
        {
            GameSettingsSaveData data = GetData();
            return new GameSettingsSnapshot(data.masterVolume, data.bgmVolume, data.sfxVolume);
        }

        public static void SetMasterVolume(float volume)
        {
            GameSettingsSaveData data = GetData();
            SetVolume(ref data.masterVolume, volume);
        }

        public static void SetBgmVolume(float volume)
        {
            GameSettingsSaveData data = GetData();
            SetVolume(ref data.bgmVolume, volume);
        }

        public static void SetSfxVolume(float volume)
        {
            GameSettingsSaveData data = GetData();
            SetVolume(ref data.sfxVolume, volume);
        }

        public static void Flush()
        {
            if (!_isDirty) return;

            try
            {
                GameSettingsStorage.Save(GetData());
                _isDirty = false;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[GameSettingsStore] 설정 저장에 실패했습니다.\n{exception}");
            }
        }

        internal static void ResetCachedData()
        {
            _data = null;
            _isLoaded = false;
            _isDirty = false;
        }

        private static GameSettingsSaveData GetData()
        {
            if (_isLoaded) return _data;

            _isLoaded = true;
            _data = GameSettingsStorage.Load(out bool hasLegacySettings);
            if (hasLegacySettings)
                _isDirty = true;
            _data.Normalize();
            return _data;
        }

        private static void SetVolume(ref float target, float volume)
        {
            float clampedVolume = Mathf.Clamp01(volume);
            if (Mathf.Approximately(target, clampedVolume)) return;

            target = clampedVolume;
            _isDirty = true;
        }
    }
}
