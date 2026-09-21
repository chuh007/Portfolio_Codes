using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.States
{
    public class EnemyDeadState : EnemyState
    {
        private const float DeadAnimationFallbackDelay = 1.5f;
        private static readonly int DeadStateHash = Animator.StringToHash("Base Layer.Dead");

        private EntityAnimatorTrigger _entityAnimatorTrigger;
        private EntityMover _mover;
        private bool _fadeStarted;
        
        public EnemyDeadState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _renderer = entity.GetCompo<EntityRenderer>();
            _entityAnimatorTrigger = entity.GetCompo<EntityAnimatorTrigger>();
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _fadeStarted = false;
            _mover.StopImmediately();
            _renderer.Anim.speed = 1f;
            _renderer.Anim.Play(DeadStateHash, 0, 0f);
            _entityAnimatorTrigger.OnAnimationEnd += HandleDeadAnimEnd;

            StartDeadFadeFallback(
                _enemy.PoolLifecycleVersion,
                _enemy.GetCancellationTokenOnDestroy()).Forget();
        }

        public override void Exit()
        {
            _entityAnimatorTrigger.OnAnimationEnd -= HandleDeadAnimEnd;
            base.Exit();
        }

        private void HandleDeadAnimEnd()
        {
            TryStartDeadFade(_enemy.PoolLifecycleVersion);
        }

        private async UniTaskVoid StartDeadFadeFallback(
            int lifecycleVersion,
            System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(DeadAnimationFallbackDelay),
                    DelayType.DeltaTime,
                    PlayerLoopTiming.Update,
                    cancellationToken);
                TryStartDeadFade(lifecycleVersion);
            }
            catch (System.OperationCanceledException)
            {
            }
        }

        private void TryStartDeadFade(int lifecycleVersion)
        {
            if (_fadeStarted || !CanContinueFade(lifecycleVersion))
                return;

            _fadeStarted = true;
            DeadFade(lifecycleVersion).Forget();
        }

        private async UniTaskVoid DeadFade(int lifecycleVersion)
        {
            float time = 0;
            while (time < 0.5f)
            {
                if (!CanContinueFade(lifecycleVersion))
                    return;

                time += Time.deltaTime;
                float cur = 1f - time / 0.5f;
                _renderer.SetAlpha(cur);
                await UniTask.Yield();
            }

            if (!CanContinueFade(lifecycleVersion))
                return;

            _renderer.SetAlpha(0);
            _enemy.ReturnToPool();
        }

        private bool CanContinueFade(int lifecycleVersion)
        {
            return _enemy != null
                   && _enemy.IsDead
                   && _enemy.IsCurrentPoolLifecycle(lifecycleVersion);
        }
    }
}
