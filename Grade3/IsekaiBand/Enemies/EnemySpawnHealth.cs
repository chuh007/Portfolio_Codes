using _Work.CHUH.Code.EntityPlus;
using UnityEngine;
using BossEnemy = _Work.CHUH.Code.Enemies.Boss.Boss;

namespace _Work.CHUH.Code.Enemies
{
    internal class EnemySpawnHealth
    {
        private const string HealthStatName = "Health";
        private const float HealthGrowthMultiplier = 0.8f;
        private float _waveHealthMultiplierOverride = 1f;
        private float _waveFinalHealthMultiplier = 1f;
        private bool _hasWaveHealthMultiplierOverride;
        private int _middleBossEncounterCount;

        public float ResolveMultiplier(float multiplier, bool applyGrowth)
            => applyGrowth && _hasWaveHealthMultiplierOverride ? _waveHealthMultiplierOverride : multiplier;

        public void SetWaveHealthMultiplierOverride(
            float multiplier,
            float finalHealthMultiplier = 1f)
        {
            _waveHealthMultiplierOverride = Mathf.Max(0.1f, multiplier);
            _waveFinalHealthMultiplier = Mathf.Max(0.1f, finalHealthMultiplier);
            _hasWaveHealthMultiplierOverride = true;
        }

        public void ClearWaveHealthMultiplierOverride(float finalHealthMultiplier = 1f)
        {
            _hasWaveHealthMultiplierOverride = false;
            _waveFinalHealthMultiplier = Mathf.Max(0.1f, finalHealthMultiplier);
        }

        public void ApplyMiddleBossHealthScaling(Enemy enemy)
        {
            if (enemy is not MiddleBoss middleBoss || !middleBoss.ScaleHealthByEncounterCount)
                return;

            _middleBossEncounterCount++;

            EntityStat stat = enemy.GetCompo<EntityStat>();
            if (stat == null || !stat.TryGetStatByName(HealthStatName, out var healthStat))
                return;

            healthStat.BaseValue *= _middleBossEncounterCount;
            enemy.GetComponentInChildren<EntityHealth>(true)?.ResetToStatHealth();
        }

        public void ApplySpawnHealthScaling(Enemy enemy, float healthMultiplier, bool applyHealthGrowth)
        {
            if (!applyHealthGrowth || enemy is BossEnemy)
                return;

            EntityStat stat = enemy.GetCompo<EntityStat>();
            if (stat == null || !stat.TryGetStatByName(HealthStatName, out var hpStat))
                return;

            float safeHealthMultiplier = healthMultiplier > 0f ? healthMultiplier : 1f;
            float adjustedHealthMultiplier = 1f
                                             + (safeHealthMultiplier - 1f)
                                             * HealthGrowthMultiplier;
            adjustedHealthMultiplier *= _waveFinalHealthMultiplier;

            hpStat.BaseValue *= adjustedHealthMultiplier;

            EntityHealth health = enemy.GetComponentInChildren<EntityHealth>();
            health?.ResetToStatHealth();
        }

        public void ApplyOverallEnemyHealthMultiplier(Enemy enemy, float overallMultiplier)
        {
            float multiplier = Mathf.Max(0.1f, overallMultiplier);
            if (enemy == null || Mathf.Approximately(multiplier, 1f))
                return;

            EntityStat stat = enemy.GetCompo<EntityStat>();
            if (stat == null || !stat.TryGetStatByName(HealthStatName, out var healthStat))
                return;

            healthStat.BaseValue *= multiplier;
            enemy.GetComponentInChildren<EntityHealth>(true)?.ResetToStatHealth();
        }

    }
}
