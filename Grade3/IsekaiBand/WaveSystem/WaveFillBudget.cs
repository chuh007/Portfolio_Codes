using UnityEngine;

namespace _Work.CHUH.Code.WaveSystem
{
    internal class WaveFillBudget
    {
        private readonly float _duration;
        private readonly int _maxBatchSize;
        private int _pacedDeficit;
        private float _spawnBudget;

        public WaveFillBudget(int deficit, float duration, int maxBatchSize)
        {
            _pacedDeficit = deficit;
            _duration = Mathf.Max(0.1f, duration);
            _maxBatchSize = Mathf.Max(1, maxBatchSize);
        }

        public int GetSpawnAttempts(int missingCount, float deltaTime)
        {
            // 보충 도중 부족분이 커지면 속도를 높이고, 보충 완료 전까지 낮추지 않는다.
            _pacedDeficit = Mathf.Max(_pacedDeficit, missingCount);
            float rate = Mathf.Max(1f, Mathf.Max(0, _pacedDeficit) / _duration);
            _spawnBudget = Mathf.Min(_maxBatchSize, _spawnBudget + deltaTime * rate);
            return Mathf.Min(missingCount, Mathf.Min(_maxBatchSize, Mathf.FloorToInt(_spawnBudget)));
        }

        public void Consume(int spawnedCount)
        {
            if (spawnedCount > 0) _spawnBudget -= spawnedCount;
            else _spawnBudget = 0f;
        }
    }
}
