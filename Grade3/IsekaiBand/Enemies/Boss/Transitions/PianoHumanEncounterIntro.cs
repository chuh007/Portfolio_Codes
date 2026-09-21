using System.Threading;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class PianoHumanEncounterIntro
    {
        private readonly PianoBossHumanFormTransition _source;
        private readonly PianoHumanEncounter _encounter;
        private CancellationTokenSource _encounterIntroCts;
        private PlayerMovementCompo _introLockedMovement;

        public PianoHumanEncounterIntro(PianoBossHumanFormTransition source, PianoHumanEncounter encounter)
        {
            _source = source;
            _encounter = encounter;
        }

        public void Begin()
        {
            Cancel();

            _encounter.Body.Prepare();

            Player player = UnityEngine.Object.FindAnyObjectByType<Player>();
            _introLockedMovement = player != null
                ? player.GetComponent<PlayerMovementCompo>()
                : null;
            _introLockedMovement?.LockInput();
            GameplayUiBlockService.Acquire(this);

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(
                _encounter.Body.Owner.destroyCancellationToken);
            _encounterIntroCts = cts;
            PlayEncounterIntroAsync(cts).Forget();
        }

        private async UniTaskVoid PlayEncounterIntroAsync(CancellationTokenSource cts)
        {
            try
            {
                // 스포너가 같은 호출에서 보스 아레나 카메라를 먼저 구성하도록 한 프레임 양보한다.
                bool canceled = await UniTask.Yield(
                        PlayerLoopTiming.Update,
                        cts.Token)
                    .SuppressCancellationThrow();
                if (canceled)
                    return;

                PianoBossIntroDialoguePresentation introDialogue =
                    _source.GetComponent<PianoBossIntroDialoguePresentation>();
                if (introDialogue != null)
                    await introDialogue.PlayAsync(_source.transform, cts.Token);
            }
            finally
            {
                if (_encounterIntroCts == cts)
                {
                    bool wasCanceled = cts.IsCancellationRequested;
                    _encounterIntroCts = null;
                    cts.Dispose();
                    ReleaseIntroLocks();

                    if (!wasCanceled
                        && _encounter.Body.Owner != null
                        && _encounter.IsActive
                        && !_encounter.IsTransitioning)
                    {
                        PianoHumanFormChange.PlayBgm(_source.PhaseOneBgmKey);
                        _encounter.Body.Resume();
                    }
                }
            }
        }

        public void Cancel()
        {
            if (_encounterIntroCts != null)
            {
                _encounterIntroCts.Cancel();
                _encounterIntroCts.Dispose();
                _encounterIntroCts = null;
            }

            ReleaseIntroLocks();
        }

        private void ReleaseIntroLocks()
        {
            GameplayUiBlockService.Release(this);
            _introLockedMovement?.UnlockInput();
            _introLockedMovement = null;
        }
    }
}
