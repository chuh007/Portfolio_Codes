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
[NodeDescription(name: "AttackWarning", story: "[Self] prepare [Pattern]", category: "Action", id: "60ee97c5d9327e5ad8b2bc867158bfd9")]
public partial class AttackWarningAction : Action
{
    private const string LogPrefix = "[AttackWarningAction]";
    private const float MinAnimationDuration = 0.01f;
    private const string DashPatternVariableName = "DashPattern";
    private const string AttackPattern1VariableName = "AttackPattern1";
    private const string AttackPattern2VariableName = "AttackPattern2";
    private const string AttackPattern3VariableName = "AttackPattern3";
    private const float RareAttackPattern2AlternativeChance = 0.25f;

    [SerializeReference] public BlackboardVariable<Boss> Self;
    [SerializeReference] public BlackboardVariable<BasePatternSO> Pattern;

    [SerializeField] private string attackReadyStateName = "AttackReady";
    [SerializeField] private string legacyAttackReadyStateName = "AttackWarning";
    [SerializeField] private string attackBlendParameter = "AttackKey";
    [SerializeField] private string fallbackBlendParameter = "Blend";
    [SerializeField] private int animatorLayer = 0;
    [SerializeField] private float authoredReadyDuration = 1f;

    private bool _isEnd;
    private bool _isCanceled;
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

        _selectedPattern = Self.Value.ConsumeForcedPattern();
        bool isForcedPattern = _selectedPattern != null;
        if (!isForcedPattern)
            _selectedPattern = SelectPattern(Self.Value, Pattern?.Value);

        _selectedPattern = ResolveSelectionPattern(Self.Value, _selectedPattern);

        if (_selectedPattern == null)
        {
            Debug.LogWarning(
                $"{LogPrefix} Start failed: selected pattern is null. requested={GetPatternName(Pattern?.Value)}, forced={isForcedPattern}",
                Self.Value);
            return Status.Failure;
        }

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(
            Self.Value.destroyCancellationToken,
            Self.Value.PhaseCancellationToken);

        WarningAction(_cts.Token).Forget();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isCanceled) return Status.Failure;
        if (!_isEnd) return Status.Running;
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

        _selectedPattern = null;
    }

    private async UniTaskVoid WarningAction(CancellationToken token)
    {
        Animator animator = GetAnimator();
        BasePatternSO pattern = _selectedPattern;

        try
        {
            BossPatternSelection.Set(Self.Value, pattern);
            if (pattern.UsesOwnerAttackAnimation)
                PlayReadyAnimation(animator, pattern);

            await pattern.UsePattern(Self.Value, token, PatternUseMode.PrepareOnly);

            if (token.IsCancellationRequested)
            {
                _isCanceled = true;
                return;
            }

            _isEnd = true;
        }
        catch (OperationCanceledException)
        {
            _isCanceled = true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"{LogPrefix} Prepare exception. pattern={GetPatternName(pattern)}", Self.Value);
            Debug.LogException(exception, Self.Value);
            _isCanceled = true;
        }
        finally
        {
            ResetAnimatorSpeed(animator);
        }
    }

    private Animator GetAnimator()
    {
        if (Self?.Value == null) return null;

        EntityRenderer renderer = Self.Value.GetCompo<EntityRenderer>();
        return renderer != null ? renderer.Anim : null;
    }

    private void PlayReadyAnimation(Animator animator, BasePatternSO pattern)
    {
        if (animator == null || pattern == null) return;

        SetAttackBlendValue(animator, pattern.BlendTreeValue);

        float readyDuration = authoredReadyDuration > 0f ? authoredReadyDuration : 1f;
        float speed = Mathf.Max(MinAnimationDuration, readyDuration) /
                      Mathf.Max(MinAnimationDuration, pattern.WarningDuration);

        if (!TryPlayState(animator, attackReadyStateName, speed))
            TryPlayState(animator, legacyAttackReadyStateName, speed);
    }

    private bool TryPlayState(Animator animator, string stateName, float speed)
    {
        if (animator == null || string.IsNullOrWhiteSpace(stateName)) return false;
        if (animatorLayer < 0 || animatorLayer >= animator.layerCount) return false;

        int stateHash = Animator.StringToHash(stateName);
        if (!animator.HasState(animatorLayer, stateHash)) return false;

        animator.speed = Mathf.Max(MinAnimationDuration, speed);
        animator.Play(stateHash, animatorLayer, 0f);
        return true;
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

    private void ResetAnimatorSpeed(Animator animator)
    {
        if (animator == null) return;
        animator.speed = 1f;
    }

    private static string GetPatternName(BasePatternSO pattern)
    {
        return pattern != null ? pattern.name : "null";
    }

    private BasePatternSO SelectPattern(Boss boss, BasePatternSO requestedPattern)
    {
        if (requestedPattern == null)
        {
            BasePatternSO fallback = TryGetAlternativePattern(boss, null);
            return fallback;
        }

        if (BossPatternSelection.IsOnCooldown(boss, requestedPattern))
            return TryGetAlternativePattern(boss, requestedPattern);

        if (!requestedPattern.CanReplaceRepeatedSelection)
            return requestedPattern;

        if (!BossPatternSelection.IsLastPattern(boss, requestedPattern))
            return requestedPattern;

        BasePatternSO alternative = TryGetAlternativePattern(boss, requestedPattern);
        return alternative != null ? alternative : requestedPattern;
    }

    private BasePatternSO ResolveSelectionPattern(Boss boss, BasePatternSO pattern)
    {
        if (pattern == null)
            return null;

        return pattern.ResolveSelectionPattern(boss);
    }

    private BasePatternSO TryGetAlternativePattern(Boss boss, BasePatternSO lastPattern)
    {
        BasePatternSO selectedPattern = null;
        int candidateCount = 0;

        TrySelectAlternative(boss, DashPatternVariableName, lastPattern, ref selectedPattern, ref candidateCount);
        TrySelectAlternative(boss, AttackPattern1VariableName, lastPattern, ref selectedPattern, ref candidateCount);

        if (UnityEngine.Random.value > RareAttackPattern2AlternativeChance && selectedPattern != null)
            return selectedPattern;

        TrySelectAlternative(boss, AttackPattern2VariableName, lastPattern, ref selectedPattern, ref candidateCount);
        TrySelectAlternative(boss, AttackPattern3VariableName, lastPattern, ref selectedPattern, ref candidateCount);

        return selectedPattern;
    }

    private void TrySelectAlternative(
        Boss boss,
        string variableName,
        BasePatternSO lastPattern,
        ref BasePatternSO selectedPattern,
        ref int candidateCount)
    {
        BlackboardVariable<BasePatternSO> variable = boss.GetBlackboardVariable<BasePatternSO>(variableName);
        BasePatternSO pattern = variable?.Value;
        if (pattern == null || pattern == lastPattern || BossPatternSelection.IsOnCooldown(boss, pattern))
            return;

        candidateCount++;
        if (UnityEngine.Random.Range(0, candidateCount) == 0)
            selectedPattern = pattern;
    }
}
