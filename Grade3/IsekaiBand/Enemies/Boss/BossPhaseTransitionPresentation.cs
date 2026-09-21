using System;
using System.Threading;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    [Serializable]
    public class BossPhaseTransitionPresentation
    {
        [SerializeField] private bool moveToArenaCenterOnPhaseChange = true;
        [SerializeField] private bool playDeadAnimationOnPhaseChange = true;
        [SerializeField] private string deadStateName = "Dead";
        [SerializeField] private string deadBoolParameter = "DEAD";
        [SerializeField, Min(0f)] private float animationDuration = 1.6f;
        [SerializeField] private string returnStateName = "Idle";
        [SerializeField] private string returnBoolParameter = "IDLE";

        public async UniTask<bool> PlayAsync(Boss boss, EntityMover mover, CancellationToken cancellationToken)
        {
            if (boss == null)
                return false;

            if (!playDeadAnimationOnPhaseChange)
            {
                if (moveToArenaCenterOnPhaseChange)
                    MoveToBossArenaCenter(boss, mover);

                return false;
            }

            Animator animator = boss.GetComponentInChildren<Animator>();
            if (!BossPhaseAnimator.TryGetState(animator, deadStateName, out int layerIndex, out int stateHash))
            {
                if (moveToArenaCenterOnPhaseChange)
                    MoveToBossArenaCenter(boss, mover);

                return false;
            }

            BossPhaseAnimator.SetBoolIfExists(animator, returnBoolParameter, false);
            BossPhaseAnimator.SetBoolIfExists(animator, deadBoolParameter, true);

            bool canceled = await BossPhaseAnimator.PlayAsync(animator, layerIndex, stateHash, animationDuration, false, cancellationToken);
            if (canceled) return true;

            Renderer[] renderers = boss.GetComponentsInChildren<Renderer>();
            bool[] rendererStates = SetRenderersEnabled(renderers, false);

            if (moveToArenaCenterOnPhaseChange)
                MoveToBossArenaCenter(boss, mover);

            RestoreRenderersEnabled(renderers, rendererStates);

            canceled = await BossPhaseAnimator.PlayAsync(animator, layerIndex, stateHash, animationDuration, true, cancellationToken);
            if (canceled) return true;

            BossPhaseAnimator.SetBoolIfExists(animator, deadBoolParameter, false);
            BossPhaseAnimator.SetBoolIfExists(animator, returnBoolParameter, true);

            if (BossPhaseAnimator.TryGetState(animator, returnStateName, out layerIndex, out stateHash))
                animator.Play(stateHash, layerIndex, 0f);

            return false;
        }

        private static void MoveToBossArenaCenter(Boss boss, EntityMover mover)
        {
            StageHelper stageHelper = StageHelper.Instance;
            if (stageHelper == null || !stageHelper.IsBossArenaActive)
                return;

            Vector2 center = stageHelper.BossArenaCenter;
            boss.transform.position = new Vector3(center.x, center.y, boss.transform.position.z);
            mover?.StopImmediately();
        }

        private static bool[] SetRenderersEnabled(Renderer[] renderers, bool isEnabled)
        {
            bool[] states = new bool[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null)
                    continue;

                states[i] = renderers[i].enabled;
                renderers[i].enabled = isEnabled;
            }

            return states;
        }

        private static void RestoreRenderersEnabled(Renderer[] renderers, bool[] states)
        {
            for (int i = 0; i < renderers.Length && i < states.Length; i++)
            {
                if (renderers[i] != null)
                    renderers[i].enabled = states[i];
            }
        }
    }
}
