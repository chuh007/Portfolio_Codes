using System;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.WeaponCombine;
using Chuh007Lib.Bus;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class StageBossMusic
    {
        private readonly MonoBehaviour _owner;
        private readonly Func<Transform> _resolvePlayer;
        private readonly Func<float> _readFadeDuration;
        private Enemy _activeBoss1Encounter;

        public StageBossMusic(MonoBehaviour owner, Func<Transform> resolvePlayer, Func<float> readFadeDuration)
        {
            _owner = owner;
            _resolvePlayer = resolvePlayer;
            _readFadeDuration = readFadeDuration;
        }

        public void Play(Enemy boss)
        {
            _activeBoss1Encounter = boss;
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(SoundKeys.Boss1Bgm, SoundType.BGM));
        }

        public void HandleEnemyDead(EnemyDeadEvent evt)
        {
            if (evt.Enemy == _activeBoss1Encounter)
            {
                _activeBoss1Encounter = null;
                RestoreInGameBgmAfterBoss1Async().Forget();
            }
        }

        public void StopOnDestroy()
        {
            if (_activeBoss1Encounter != null && SoundManager.HasInstance)
                FadeOutBoss1Bgm();
        }

        private void FadeOutBoss1Bgm()
        {
            SoundManager.FadeOutBgmAsync(
                    _readFadeDuration(),
                    default)
                .Forget();
        }

        private async UniTask RestoreInGameBgmAfterBoss1Async()
        {
            bool interrupted = await SoundManager.FadeOutBgmAsync(
                _readFadeDuration(),
                _owner.destroyCancellationToken);
            if (interrupted || _owner == null)
                return;

            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(ResolveGameplayBgmKey(), SoundType.BGM));
        }

        private string ResolveGameplayBgmKey()
        {
            Transform playerTransform = _resolvePlayer();
            return playerTransform != null
                   && playerTransform.TryGetComponent(out CombineWeaponController combineController)
                   && combineController.HasCombinedWeapon(CombineWeaponType.FullBand)
                ? SoundKeys.FullBandBgm
                : SoundKeys.InGameBgm;
        }
    }
}
