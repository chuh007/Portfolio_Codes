using _Work.CHUH.Code.Enemies.Boss;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossDeathLoop", story: "[Boss] death loop [Second] with [Animator]", category: "Action", id: "c412065f379c2c733a4277a473a7e63c")]
public partial class BossDeathLoopAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<float> Second;
    [SerializeReference] public BlackboardVariable<Animator> Animator;

    [SerializeField] private string deadBoolParameter = "DEAD";
    [SerializeField] private string deadLoopStateName = "dead_loop";
    [SerializeField] private string deadStateName = "Dead";
    [SerializeField] private int animatorLayer = 0;

    private Boss _boss;
    private Animator _animator;
    private float _elapsed;
    private float _loopDuration;
    private float _deadDuration;
    private int _layerIndex;
    private bool _isPlayingDead;

    protected override Status OnStart()
    {
        _boss = Boss?.Value;
        _animator = Animator?.Value;
        if (_boss == null || Second == null || _animator == null)
            return Status.Failure;

        _elapsed = 0f;
        _loopDuration = Mathf.Max(0f, Second.Value);
        _deadDuration = 0f;
        _isPlayingDead = false;
        _layerIndex = Mathf.Clamp(animatorLayer, 0, _animator.layerCount - 1);
        _animator.speed = 1f;
        _animator.SetBool(deadBoolParameter, true);

        if (!TryPlayState(deadLoopStateName))
            return PlayDeadOnce();

        return _loopDuration <= 0f ? PlayDeadOnce() : Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_boss == null)
            return Status.Failure;

        _elapsed += Time.deltaTime;

        if (!_isPlayingDead)
            return _elapsed >= _loopDuration ? PlayDeadOnce() : Status.Running;

        return _elapsed >= _deadDuration ? CompleteDeath() : Status.Running;
    }

    private Status PlayDeadOnce()
    {
        _elapsed = 0f;
        _isPlayingDead = true;

        if (!TryPlayState(deadStateName))
            return CompleteDeath();

        _deadDuration = GetCurrentStateDuration();
        return _deadDuration <= 0f ? CompleteDeath() : Status.Running;
    }

    private bool TryPlayState(string stateName)
    {
        if (_animator == null || string.IsNullOrWhiteSpace(stateName))
            return false;

        int stateHash = UnityEngine.Animator.StringToHash(stateName);
        if (!_animator.HasState(_layerIndex, stateHash))
            return false;

        _animator.Play(stateHash, _layerIndex, 0f);
        _animator.Update(0f);
        return true;
    }

    private float GetCurrentStateDuration()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(_layerIndex);
        return Mathf.Max(0f, stateInfo.length);
    }

    private Status CompleteDeath()
    {
        _boss.CompleteDelayedDeath();
        return _boss.IsDelayedDeathCompleted ? Status.Success : Status.Running;
    }
}

