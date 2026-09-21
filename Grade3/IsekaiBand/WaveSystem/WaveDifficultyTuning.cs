using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using UnityEngine;

namespace _Work.CHUH.Code.WaveSystem
{
    /// <summary>
    /// 일반 웨이브에서는 이 곡선이 기존 WaveData의 multiplierValue를 대체한다.
    /// 4분까지는 부품 수집, 이후에는 풀강/진화 악기의 성장을 기준으로 하며
    /// 재료 악기를 제거하는 조합 무기는 필수 전력으로 계산하지 않는다.
    /// 특별 웨이브 여부는 각 WaveDataSO가 소유하며 제작된 multiplierValue를 유지한다.
    /// StageData의 체력 성장 배율은 제작된 값 위에 별도로 적용한다.
    /// </summary>
    internal static class WaveDifficultyTuning
    {
        public static void ApplyHealth(Spawner spawner, StageDataSO stage,
            WaveDataSO wave, int waveIndex, float difficulty)
        {
            if (spawner == null) return;
            float stageMultiplier = stage != null ? stage.GetEnemyHealthGrowthMultiplier(waveIndex) : 1f;
            if (UsesAuthoredHealthMultiplier(wave))
            {
                spawner.ClearWaveHealthMultiplierOverride(stageMultiplier);
                return;
            }
            float target = GetWeaponProgressionHealthMultiplier(waveIndex);
            float multiplier = Mathf.LerpUnclamped(1f, target, Mathf.Max(0f, difficulty));
            spawner.SetWaveHealthMultiplierOverride(Mathf.Max(0.1f, multiplier),
                stageMultiplier);
        }

        public static bool IsSpecialWave(WaveDataSO waveData)
            => waveData != null && waveData.IsSpecialWave;

        public static bool UsesAuthoredHealthMultiplier(WaveDataSO waveData)
            => IsSpecialWave(waveData);

        public static float GetWeaponProgressionHealthMultiplier(int waveIndex)
        {
            int wave = Mathf.Max(0, waveIndex);
            // 6분 웨이브(index 12)부터 구간별 증가량을 20% 낮춘다.
            return wave switch
            {
                <= 8 => LerpSegment(wave, 0, 8, 1f, 1.25f),
                <= 11 => LerpSegment(wave, 8, 11, 1.25f, 1.575f),
                <= 14 => LerpSegment(wave, 11, 14, 1.575f, 1.835f),
                <= 19 => LerpSegment(wave, 14, 19, 1.835f, 2.555f),
                _ => LerpSegment(wave, 19, 23, 2.555f, 3.515f)
            };
        }

        private static float LerpSegment(
            int wave,
            int startWave,
            int endWave,
            float startMultiplier,
            float endMultiplier)
        {
            float progress = Mathf.InverseLerp(startWave, endWave, wave);
            return Mathf.Lerp(startMultiplier, endMultiplier, progress);
        }
    }
}
