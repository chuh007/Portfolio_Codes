using System;
using System.Threading;
using _Code.LCH._02.Scripts.Core;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class PianoHumanFormChange
    {
        private readonly PianoBossHumanFormTransition _source;
        private readonly PianoHumanEncounter _encounter;
        private CancellationTokenSource _transitionCts;

        public PianoHumanFormChange(PianoBossHumanFormTransition source, PianoHumanEncounter encounter)
        {
            _source = source;
            _encounter = encounter;
        }

        public void Begin(bool skipPresentation)
        {
            _transitionCts = CancellationTokenSource.CreateLinkedTokenSource(
                _encounter.Body.Owner.destroyCancellationToken);
            TransitionToBossFormAsync(skipPresentation, _transitionCts.Token).Forget();
        }

        private async UniTaskVoid TransitionToBossFormAsync(
            bool skipPresentation,
            CancellationToken cancellationToken)
        {
            Spawner spawner = UnityEngine.Object.FindAnyObjectByType<Spawner>();
            if (spawner == null)
            {
                Debug.LogError($"[{nameof(PianoBossHumanFormTransition)}] Spawner was not found.", _source);
                _encounter.RestoreAfterFailedTransition();
                return;
            }

            Entity target = _encounter.Body.Owner.target;
            Vector2 fallbackSpawnPosition = target != null
                ? target.transform.position
                : _encounter.Body.Owner.transform.position;

            _encounter.Body.Prepare();

            var entry = new PianoBossFormEntry(_source);
            try
            {
                if (!skipPresentation)
                    GameplayUiBlockService.Acquire(entry);

                if (!skipPresentation && await PianoHumanDeparture.PlayAsync(
                        _source, _encounter.Body, cancellationToken))
                    return;

                Boss boss = spawner.ReplaceWithBossForm(
                    _encounter.Body.Owner,
                    _source.PhaseTwoBossPoolItem,
                    fallbackSpawnPosition);
                if (boss == null)
                {
                    Debug.LogError(
                        $"[{nameof(PianoBossHumanFormTransition)}] Failed to spawn {_source.PhaseTwoBossPoolItem.name}.",
                        _source);
                    _encounter.RestoreAfterFailedTransition();
                    if (!skipPresentation)
                        PlayBgm(_source.PhaseOneBgmKey);

                    return;
                }

                await entry.PlayAsync(boss, skipPresentation, _encounter.RewardCount);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                GameplayUiBlockService.Release(entry);
                entry.Dispose();
            }
        }

        public void Cancel()
        {
            if (_transitionCts == null)
                return;

            _transitionCts.Cancel();
            _transitionCts.Dispose();
            _transitionCts = null;
        }

        public static void PlayBgm(string soundKey)
        {
            if (!string.IsNullOrWhiteSpace(soundKey))
                Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(soundKey, SoundType.BGM));
        }
    }
}
