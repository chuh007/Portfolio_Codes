using UnityEngine;

namespace _Work.CHUH.Code.Core.Persistence
{
    /// <summary>
    /// 현재 게임이 소유한 설정과 진행 데이터를 모두 삭제한다.
    /// </summary>
    public static class GameDataResetService
    {
        public static void DeleteAllData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            PersistentProgressStore.ResetCachedData();
            GameSettingsStore.ResetCachedData();
        }
    }
}
