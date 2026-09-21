using System;
using UnityEngine;

namespace _Work.CHUH.Code.Core.Persistence
{
    internal static class GameSettingsStorage
    {
        private const string PlayerPrefsKey = "BandRoguelike.GameSettings";
        private const string LegacyMasterVolumeKey = "Setting_Master";
        private const string LegacyBgmVolumeKey = "Setting_BGM";
        private const string LegacySfxVolumeKey = "Setting_SFX";

        public static GameSettingsSaveData Load(out bool hasLegacySettings)
        {
            hasLegacySettings = false;
            string json = PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    return JsonUtility.FromJson<GameSettingsSaveData>(json) ??
                           new GameSettingsSaveData();
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"[GameSettingsStore] 설정 데이터를 읽지 못해 기본값으로 시작합니다.\n{exception}");
                }
            }
            return LoadLegacySettings(out hasLegacySettings);
        }

        private static GameSettingsSaveData LoadLegacySettings(out bool hasLegacySettings)
        {
            var data = new GameSettingsSaveData();
            hasLegacySettings = PlayerPrefs.HasKey(LegacyMasterVolumeKey) ||
                                PlayerPrefs.HasKey(LegacyBgmVolumeKey) ||
                                PlayerPrefs.HasKey(LegacySfxVolumeKey);
            if (!hasLegacySettings) return data;

            data.masterVolume = PlayerPrefs.GetFloat(LegacyMasterVolumeKey, 1f);
            data.bgmVolume = PlayerPrefs.GetFloat(LegacyBgmVolumeKey, 1f);
            data.sfxVolume = PlayerPrefs.GetFloat(LegacySfxVolumeKey, 1f);
            return data;
        }

        public static void Save(GameSettingsSaveData data)
        {
            data.version = GameSettingsSaveData.CurrentVersion;
            PlayerPrefs.SetString(PlayerPrefsKey, JsonUtility.ToJson(data));
            PlayerPrefs.DeleteKey(LegacyMasterVolumeKey);
            PlayerPrefs.DeleteKey(LegacyBgmVolumeKey);
            PlayerPrefs.DeleteKey(LegacySfxVolumeKey);
            PlayerPrefs.Save();
        }
    }
}
