using System.Threading;
using _Work.CHUH.Code.BT;
using _Work.CHUH.Code.Combat.Pattern.Patterns;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.EntityPlus;
using _Work.CHUH.Code.StageSystem;
using _Work.CHUH.Code.UI;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class PianoBossFormEntry
    {
        private readonly PianoBossHumanFormTransition _source;
        private ScreenFlashPresentation.Handle _flashHandle;

        public PianoBossFormEntry(PianoBossHumanFormTransition source) => _source = source;

        public async UniTask PlayAsync(Boss boss, bool skipPresentation, int rewardCount)
        {
            CancellationToken bossCancellationToken = boss.destroyCancellationToken;
            boss.SetRewardCount(rewardCount);
            EntityRenderer bossRenderer = boss.GetCompo<EntityRenderer>();
            EntityMover bossMover = boss.GetCompo<EntityMover>();
            Collider2D[] bossColliders = boss.GetComponentsInChildren<Collider2D>(true);
            BossHealthBarUI bossHealthBar = boss.GetComponentInChildren<BossHealthBarUI>(true);
            PianoBossRuntime runtime = boss.GetComponent<PianoBossRuntime>();

            bossRenderer?.SetAlpha(skipPresentation ? 1f : 0f);
            PianoHumanBody.SetCollidersEnabled(bossColliders, false);
            if (bossMover != null)
            {
                bossMover.StopImmediately();
                bossMover.CanManualMove = false;
            }

            boss.SetPatternExecutionEnabled(false);
            boss.ChangeBtState(BTEnemyState.WAIT);
            boss.EnterPhaseImmediately(_source.PhaseTwoIndex, false);
            EntityHealth bossHealth = boss.GetComponentInChildren<EntityHealth>(true);
            bossHealth?.SetCurrentHealthRatio(_source.PhaseTwoInitialHealthRatio);

            runtime?.PlaceAtArenaTopForEncounter();

            if (!skipPresentation)
                bossHealthBar?.HideUntilFillAnimation();

            Bus<BossSpawnedEvent>.Raise(new BossSpawnedEvent(boss));
            ApplyPhaseTwoCameraSize();

            PianoBossIntroDialoguePresentation introDialogue =
                boss.GetComponent<PianoBossIntroDialoguePresentation>();
            if (!skipPresentation && introDialogue != null)
            {
                bool dialogueCanceled = await introDialogue.PlayAsync(
                    boss.transform,
                    bossCancellationToken);
                if (dialogueCanceled || boss == null || boss.IsDead)
                    return;
            }

            if (!skipPresentation && bossHealthBar != null)
            {
                bool healthBarCanceled = await bossHealthBar.PlayFillAnimationAsync(
                    _source.PhaseTwoHealthBarFillDuration,
                    bossCancellationToken);
                if (healthBarCanceled || boss == null || boss.IsDead)
                    return;
            }

            if (!skipPresentation && _source.PhaseTwoSpawnFlash != null)
            {
                _flashHandle = await _source.PhaseTwoSpawnFlash.ShowAsync(bossCancellationToken);
                if (_flashHandle == null && bossCancellationToken.IsCancellationRequested)
                    return;
            }

            bossRenderer?.SetAlpha(1f);

            if (_flashHandle != null)
            {
                bool flashCanceled = await _source.PhaseTwoSpawnFlash.HideAsync(
                    _flashHandle,
                    bossCancellationToken);
                _flashHandle = null;
                if (flashCanceled || boss == null || boss.IsDead)
                    return;
            }

            PianoHumanBody.SetCollidersEnabled(bossColliders, true);
            if (bossMover != null)
                bossMover.CanManualMove = true;
            PianoHumanFormChange.PlayBgm(_source.PhaseTwoBgmKey);
            boss.SetPatternExecutionEnabled(true);
            boss.ChangeBtState(BTEnemyState.ATTACK_WARNING);
        }

        public void Dispose() => _source.PhaseTwoSpawnFlash?.Destroy(_flashHandle);

        private void ApplyPhaseTwoCameraSize()
        {
            StageController stageController = UnityEngine.Object.FindAnyObjectByType<StageController>();
            stageController?.SetBossCameraOrthographicSize(_source.PhaseTwoCameraOrthographicSize);
        }
    }
}
