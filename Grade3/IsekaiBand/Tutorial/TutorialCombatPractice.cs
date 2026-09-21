using System.Collections;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal class TutorialCombatPractice
    {

        private readonly TutorialTrainingEnemies _enemies;
        private readonly TutorialProgress _progress;
        private readonly TutorialSequenceController _settings;
        public int KillTarget => _settings.CombinedCombatKillTarget;

        public TutorialCombatPractice(TutorialSequenceController settings, TutorialTrainingEnemies enemies, TutorialProgress progress)
        {
            _settings = settings;
            _enemies = enemies;
            _progress = progress;
        }

        public IEnumerator WaitForLevelUp()
        {
            float respawnTimer = 0f;

            while (!_progress.LevelUpTriggered)
            {
                _enemies.Refresh();

                respawnTimer -= Time.deltaTime;
                if (_enemies.Count < _settings.MaximumActiveEnemies && respawnTimer <= 0f)
                {
                    if (_enemies.Spawn())
                        respawnTimer = _settings.EnemyRespawnDelay;
                    else
                        respawnTimer = 0.2f;
                }

                yield return null;
            }
        }

        public IEnumerator FightWithCombinedWeapon()
        {
            int spawnedEnemyCount = 0;
            float retryTimer = 0f;

            while (_progress.CombinedCombatKillCount < KillTarget)
            {
                _enemies.Refresh();

                retryTimer -= Time.deltaTime;
                while (spawnedEnemyCount < KillTarget
                       && _enemies.Count < _settings.CombinedCombatMaxActiveEnemies
                       && retryTimer <= 0f)
                {
                    if (!_enemies.Spawn(_settings.CombinedCombatSpawnDistance))
                    {
                        retryTimer = 0.2f;
                        break;
                    }

                    spawnedEnemyCount++;
                }

                yield return null;
            }
        }

        public IEnumerator FightFirstEnemy()
        {
            float retryTimer = 0f;

            while (_progress.KillCount < 1)
            {
                _enemies.Refresh();

                retryTimer -= Time.deltaTime;
                if (_enemies.Count == 0 && retryTimer <= 0f)
                {
                    if (!_enemies.Spawn())
                        retryTimer = 0.2f;
                }

                yield return null;
            }
        }
    }
}
