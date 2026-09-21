using System;
using System.Collections.Generic;
using _Work.CHUH.Code.Combat.Pattern;
using UnityEngine;
using UnityEngine.Events;

namespace _Work.CHUH.Code.Enemies.Boss
{
    public enum BossPhaseTriggerType
    {
        Manual,
        HealthPercent,
        LostHealthLines
    }

    [Serializable]
    public class BossPhasePatternBinding
    {
        [SerializeField] private string blackboardVariableName = "AttackPattern1";
        [SerializeField] private BasePatternSO pattern;

        public string BlackboardVariableName => blackboardVariableName;
        public BasePatternSO Pattern => pattern;
    }

    [Serializable]
    public class BossPhaseData
    {
        [SerializeField] private string phaseName = "Phase";
        [SerializeField] private BossPhaseTriggerType triggerType = BossPhaseTriggerType.Manual;
        [SerializeField, Range(0f, 1f)] private float healthPercentThreshold = 0.5f;
        [SerializeField, Min(1)] private int lostHealthLinesThreshold = 1;
        [SerializeField, Min(0f)] private float transitionDelay = 0f;
        [SerializeField] private bool stopMoveDuringTransition = true;
        [SerializeField] private bool forceWaitStateOnTransition = true;
        [SerializeField] private List<BossPhasePatternBinding> patternBindings = new();
        [SerializeField] private bool useImmediatePatternOnEnter;
        [SerializeField] private BasePatternSO immediatePattern;
        [SerializeField] private UnityEvent onTransitionStart = new();
        [SerializeField] private UnityEvent onPhaseEnter = new();
        [SerializeField] private UnityEvent onTransitionEnd = new();

        public string PhaseName => phaseName;
        public BossPhaseTriggerType TriggerType => triggerType;
        public float HealthPercentThreshold => healthPercentThreshold;
        public int LostHealthLinesThreshold => lostHealthLinesThreshold;
        public float TransitionDelay => transitionDelay;
        public bool StopMoveDuringTransition => stopMoveDuringTransition;
        public bool ForceWaitStateOnTransition => forceWaitStateOnTransition;
        public IReadOnlyList<BossPhasePatternBinding> PatternBindings => patternBindings;
        public bool UseImmediatePatternOnEnter => useImmediatePatternOnEnter;
        public BasePatternSO ImmediatePattern => immediatePattern;
        public UnityEvent OnTransitionStart => onTransitionStart;
        public UnityEvent OnPhaseEnter => onPhaseEnter;
        public UnityEvent OnTransitionEnd => onTransitionEnd;
    }

    [Serializable]
    public class BossPhaseIndexEvent : UnityEvent<int> { }
}
