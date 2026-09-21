using UnityEngine;

namespace _Code.LCH._02.Scripts.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "SO/Level/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        [Header("최대 레벨")]
        public int maxLevel = 30;

        [Header("레벨별 필요 경험치 곡선 (X = 레벨, Y = 필요 경험치)")]
        public AnimationCurve expCurve = AnimationCurve.EaseInOut(1, 100, 20, 5000);

        public float GetRequiredExp(int level)
        {
            return expCurve.Evaluate(level);
        }
    }
}