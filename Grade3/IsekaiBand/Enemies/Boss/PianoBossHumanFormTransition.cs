using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Combat;
using _Work.CHUH.Code.Combat.Pattern;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

namespace _Work.CHUH.Code.Enemies.Boss
{
    public sealed class PianoBossHumanFormTransition : MonoBehaviour, IEntityComponent,
        IAfterInitalize, IMinimumHealthPolicy, IHealthDepletionHandler
    {
        [SerializeField] private PoolItemSO phaseTwoBossPoolItem;
        [SerializeField] private string phaseOneBgmKey = SoundKeys.PianoBossPhase1Bgm;
        [SerializeField] private string phaseTwoBgmKey = SoundKeys.PianoBossPhase2Bgm;
        [SerializeField, Range(0f, 0.95f)] private float transitionHealthRatio = 0.5f;
        [SerializeField, Range(0.05f, 1f)] private float phaseTwoInitialHealthRatio = 1f;
        [SerializeField] private BasePatternSO phaseOneClosingRingPattern;
        [SerializeField] private BossDefeatPresentation phaseOneDefeatPresentation = new();
        [SerializeField, Min(0f)] private float phaseOneVanishDuration = 0.45f;
        [SerializeField, Min(1f)] private float phaseOneVanishVerticalScaleMultiplier = 1.8f;
        [SerializeField, Min(0f)] private float phaseTransitionDelay = 3f;
        [SerializeField, Min(1f)] private float phaseTwoCameraOrthographicSize = 7f;
        [SerializeField] private ScreenFlashPresentation phaseTwoSpawnFlash = new();
        [SerializeField, Min(0f)] private float phaseTwoHealthBarFillDuration = 3f;
        [SerializeField, Min(0)] private int phaseTwoIndex = 1;

        private PianoHumanEncounter _encounter;
        private PianoHumanEncounter Encounter => _encounter ??= new PianoHumanEncounter(this);

        public bool IsConfigured => enabled && phaseTwoBossPoolItem != null;
        public float MinimumHealth => Encounter.MinimumHealth;
        internal PoolItemSO PhaseTwoBossPoolItem => phaseTwoBossPoolItem;
        internal string PhaseOneBgmKey => phaseOneBgmKey;
        internal string PhaseTwoBgmKey => phaseTwoBgmKey;
        internal float TransitionHealthRatio => transitionHealthRatio;
        internal float PhaseTwoInitialHealthRatio => phaseTwoInitialHealthRatio;
        internal BasePatternSO PhaseOneClosingRingPattern => phaseOneClosingRingPattern;
        internal BossDefeatPresentation PhaseOneDefeatPresentation => phaseOneDefeatPresentation;
        internal float PhaseOneVanishDuration => phaseOneVanishDuration;
        internal float PhaseOneVanishVerticalScaleMultiplier => phaseOneVanishVerticalScaleMultiplier;
        internal float PhaseTransitionDelay => phaseTransitionDelay;
        internal float PhaseTwoCameraOrthographicSize => phaseTwoCameraOrthographicSize;
        internal ScreenFlashPresentation PhaseTwoSpawnFlash => phaseTwoSpawnFlash;
        internal float PhaseTwoHealthBarFillDuration => phaseTwoHealthBarFillDuration;
        internal int PhaseTwoIndex => phaseTwoIndex;

        public void Initialize(Entity entity) => Encounter.Body.Initialize(entity);

        public void AfterInitialize() => Encounter.AfterInitialize();

        public void BeginEncounter(int rewardCount) => Encounter.BeginEncounter(rewardCount);

        public bool TryHandleHealthDepleted() => Encounter.TryHandleHealthDepleted();

#if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.pKey.wasPressedThisFrame)
                return;

            Encounter.BeginTransition(true);
        }
#endif

        private void OnDisable() => Encounter.Disable();

        private void OnDestroy() => Encounter.Dispose();
    }
}
