using UnityEngine;

namespace _Work.CHUH.Code.Item
{
    public static class UpgradeChestRewardCalculator
    {
        private const int MaxRewardCount = 5;

        // 행: 상자의 보상 단계, 열: 지급 개수 1~5개의 기본 가중치.
        private static readonly float[,] BaseWeights =
        {
            { 70f, 15f, 10f, 4f, 1f },
            { 45f, 30f, 15f, 7f, 3f },
            { 25f, 25f, 25f, 15f, 10f },
            { 10f, 20f, 25f, 25f, 20f },
            { 5f, 10f, 20f, 25f, 40f }
        };

        public static int Roll(int rewardLevel, float luck, float luckWeightPerPoint, float randomValue)
        {
            int row = Mathf.Clamp(rewardLevel, 1, MaxRewardCount) - 1;
            float luckWeight = Mathf.Max(0f, luck) * Mathf.Max(0f, luckWeightPerPoint);
            float totalWeight = 0f;

            // 기본 가중치 × (1 + 행운 × 보정 계수 × (지급 개수 - 1)).
            // 합계를 기준으로 추첨하므로 행운이 커져도 확률이 음수가 되지 않는다.
            for (int i = 0; i < MaxRewardCount; i++)
                totalWeight += BaseWeights[row, i] * (1f + luckWeight * i);

            float roll = Mathf.Clamp01(randomValue) * totalWeight;
            for (int i = 0; i < MaxRewardCount - 1; i++)
            {
                float weight = BaseWeights[row, i] * (1f + luckWeight * i);
                if (roll < weight) return i + 1;
                roll -= weight;
            }

            return MaxRewardCount;
        }
    }
}
