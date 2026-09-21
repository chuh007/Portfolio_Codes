using _Work.CHUH.Code.Combat.Pattern;
using _Work.CHUH.Code.Enemies.Boss;
using System;
using System.Threading;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UsePattern", story: "[Self] use Pattern", category: "Action", id: "97822e70c409ff8070178ed4e53903bf")]
public partial class UsePatternAction : Action
{
    private const string LogPrefix = "[UsePatternAction]";

    [SerializeReference] public BlackboardVariable<Boss> Self;

    [SerializeField] private string attackStateName = "Attack";
    [SerializeField] private string attackBlendParameter = "AttackKey";
    [SerializeField] private string fallbackBlendParameter = "Blend";
    [SerializeField] private string attackBoolParameter = "ATTACK";
    [SerializeField] private int animatorLayer = 0;
    
    private bool _isEnd = false;
    private bool _isCanceled = false;
    private CancellationTokenSource _cts;
    private BasePatternSO _selectedPattern;
    
    protected override Status OnStart()
    {
        _isEnd = false;
        _isCanceled = false;

        if (Self?.Value == null)
        {
            Debug.LogWarning($"{LogPrefix} Start failed: Self is null.");
            return Status.Failure;
        }

        if (Self.Value.IsPhaseTransitioning || !Self.Value.IsPatternExecutionEnabled)
        {
            return Status.Failure;
        }

        _selectedPattern = BossPatternSelection.Consume(Self.Value);
        if (_selectedPattern == null)
        {
            Debug.LogWarning($"{LogPrefix} Start failed: no selected pattern to consume.", Self.Value);
            return Status.Failure;
        }

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(
            Self.Value.destroyCancellationToken,
            Self.Value.PhaseCancellationToken);
        
        PatternAction(_cts.Token).Forget();
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isCanceled) return Status.Failure;
        if(!_isEnd) return Status.Running;
        return Status.Success;
    }

    protected override void OnEnd()
    {
        ResetAnimatorSpeed(GetAnimator());
        
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private async UniTaskVoid PatternAction(CancellationToken token)
    {
        Animator animator = GetAnimator();
        BasePatternSO pattern = _selectedPattern;

        try
        {
            bool usesOwnerAttackAnimation = pattern.UsesOwnerAttackAnimation;
            SetAttackBool(animator, usesOwnerAttackAnimation);
            if (usesOwnerAttackAnimation)
                PlayAttackAnimation(animator, pattern);

            await pattern.UsePattern(Self.Value, token, PatternUseMode.ExecuteOnly);

            if (token.IsCancellationRequested)
            {
                _isCanceled = true;
                return;
            }

            BossPatternSelection.RecordUsed(Self.Value, pattern);
            Self.Value.BeginPatternRecovery(pattern.RecoveryDelay);
            _isEnd = true;
        }
        catch (OperationCanceledException)
        {
            _isCanceled = true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"{LogPrefix} PatternAction exception. pattern={GetPatternName(pattern)}", Self.Value);
            Debug.LogException(exception, Self.Value);
            _isCanceled = true;
        }
        finally
        {
            SetAttackBool(animator, false);
            ResetAnimatorSpeed(animator);
        }
    }

    private Animator GetAnimator()
    {
        if (Self?.Value == null) return null;

        EntityRenderer renderer = Self.Value.GetCompo<EntityRenderer>();
        return renderer != null ? renderer.Anim : null;
    }

    private void PlayAttackAnimation(Animator animator, BasePatternSO pattern)
    {
        if (animator == null || pattern == null) return;

        SetAttackBlendValue(animator, pattern.BlendTreeValue);
        string stateName = string.IsNullOrWhiteSpace(pattern.ExecuteAnimationStateName)
            ? attackStateName
            : pattern.ExecuteAnimationStateName;
        TryPlayState(animator, stateName);
    }

    private bool TryPlayState(Animator animator, string stateName)
    {
        if (animator == null || string.IsNullOrWhiteSpace(stateName)) return false;
        if (animatorLayer < 0 || animatorLayer >= animator.layerCount) return false;

        if (!TryGetAnimatorStateOnLayer(animator, animatorLayer, stateName, out int stateHash)) return false;

        animator.speed = 1f;
        animator.Play(stateHash, animatorLayer, 0f);
        return true;
    }

    private static bool TryGetAnimatorStateOnLayer(
        Animator animator,
        int layerIndex,
        string stateName,
        out int stateHash)
    {
        stateHash = Animator.StringToHash($"{animator.GetLayerName(layerIndex)}.{stateName}");
        if (animator.HasState(layerIndex, stateHash))
            return true;

        stateHash = Animator.StringToHash(stateName);
        return animator.HasState(layerIndex, stateHash);
    }

    private void SetAttackBlendValue(Animator animator, int blendValue)
    {
        if (animator == null) return;
        if (TrySetFloat(animator, attackBlendParameter, blendValue)) return;
        TrySetFloat(animator, fallbackBlendParameter, blendValue);
    }

    private bool TrySetFloat(Animator animator, string parameterName, float value)
    {
        if (animator == null || string.IsNullOrWhiteSpace(parameterName)) return false;

        int hash = Animator.StringToHash(parameterName);
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash != hash || parameter.type != AnimatorControllerParameterType.Float)
                continue;

            animator.SetFloat(hash, value);
            return true;
        }

        return false;
    }

    private void SetAttackBool(Animator animator, bool value)
    {
        if (animator == null || string.IsNullOrWhiteSpace(attackBoolParameter)) return;

        int hash = Animator.StringToHash(attackBoolParameter);
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash != hash || parameter.type != AnimatorControllerParameterType.Bool)
                continue;

            animator.SetBool(hash, value);
            return;
        }
    }

    private void ResetAnimatorSpeed(Animator animator)
    {
        if (animator == null) return;
        animator.speed = 1f;
    }

    private static string GetPatternName(BasePatternSO pattern)
    {
        return pattern != null ? pattern.name : "null";
    }
}
