using System;
using UnityEngine;

namespace _Work.CHUH.Code.Core.Persistence
{
    internal static class PersistentProgressStorage
    {
        private const string PlayerPrefsKey = "BandRoguelike.PersistentProgress";

        public static PersistentProgressData Load()
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
                return new PersistentProgressData();

            try
            {
                return JsonUtility.FromJson<PersistentProgressData>(json) ??
                       new PersistentProgressData();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[PersistentProgressStore] 저장 데이터를 읽지 못해 새 데이터로 시작합니다.\n{exception}");
                return new PersistentProgressData();
            }
        }

        public static void Save(PersistentProgressData data)
        {
            data.version = PersistentProgressData.CurrentVersion;
            PlayerPrefs.SetString(PlayerPrefsKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }
    }
}
