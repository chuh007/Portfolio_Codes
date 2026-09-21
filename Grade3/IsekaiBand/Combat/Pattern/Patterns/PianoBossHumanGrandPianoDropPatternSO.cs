using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(
        fileName = "PianoBossHumanGrandPianoDropPattern",
        menuName = "SO/Pattern/PianoBoss Human/Grand Piano Drop",
        order = 5)]
    public sealed class PianoBossHumanGrandPianoDropPatternSO : PianoBossHumanPatternBaseSO
    {
        [Header("Grand Piano")]
        [SerializeField] private GameObject grandPianoPrefab;
        [SerializeField] private Sprite grandPianoSprite;
        [SerializeField] private Color pianoColor = Color.white;
        [SerializeField, Min(0.05f)] private float pianoScale = 1.65f;
        [SerializeField, Min(0f)] private float fallHeight = 4.5f;
        [SerializeField, Min(0.01f)] private float fallDuration = 1.05f;
        [SerializeField] private float startRotation = 8f;
        [SerializeField, Min(0f)] private float impactHoldDuration = 0.75f;
        [SerializeField, Min(0f)] private float fadeOutDuration = 0.25f;
        [SerializeField, Range(0f, 1f)] private float visualLiftRatio = 0.25f;

        [Header("Impact Audio")]
        [SerializeField] private string impactSoundKey = SoundKeys.PianoBossGrandPianoImpact;
        [SerializeField, Min(1)] private int randomImpactNoteCount = 5;

        [Header("Wide Impact")]
        [SerializeField, Min(0.05f)] private float impactRadius = 4f;
        [SerializeField, Min(0f)] private float damageMultiplier = 1.1f;
        [SerializeField, Min(0f)] private float arenaPadding = 0.25f;

        [Header("Impact Visual")]
        [SerializeField, Min(0.05f)] private float shockwaveRadius = 4.4f;
        [SerializeField, Min(0.08f)] private float shockwaveDuration = 0.45f;
        [SerializeField] private Color shockwaveColor = new(0.76f, 0.2f, 1f, 0.72f);
        [SerializeField] private int shockwaveSortingOrder = 11;

        [Header("Camera Shake")]
        [SerializeField, Min(0f)] private float cameraShakeStrength = 0.18f;
        [SerializeField, Min(0f)] private float cameraShakeDuration = 0.22f;

        private GrandPianoSequence _sequence;
        private GrandPianoLanding _landing;
        private GrandPianoVisual _visual;
        private GrandPianoImpact _impact;
        internal GrandPianoSequence Sequence => _sequence ??= new GrandPianoSequence(this);
        internal GrandPianoLanding Landing => _landing ??= new GrandPianoLanding(this);
        internal GrandPianoVisual Visual => _visual ??= new GrandPianoVisual(this);
        internal GrandPianoImpact Impact => _impact ??= new GrandPianoImpact(this);
        internal GameObject GrandPianoPrefab => grandPianoPrefab;
        internal Sprite GrandPianoSprite => grandPianoSprite;
        internal Color PianoColor => pianoColor;
        internal float PianoScale => pianoScale;
        internal float FallHeight => fallHeight;
        internal float FallDuration => fallDuration;
        internal float StartRotation => startRotation;
        internal float ImpactHoldDuration => impactHoldDuration;
        internal float FadeOutDuration => fadeOutDuration;
        internal float VisualLiftRatio => visualLiftRatio;
        internal string ImpactSoundKey => impactSoundKey;
        internal int RandomImpactNoteCount => randomImpactNoteCount;
        internal float ImpactRadius => impactRadius;
        internal float DamageMultiplier => damageMultiplier;
        internal float ArenaPadding => arenaPadding;
        internal float ShockwaveRadius => shockwaveRadius;
        internal float ShockwaveDuration => shockwaveDuration;
        internal Color ShockwaveColor => shockwaveColor;
        internal int ShockwaveSortingOrder => shockwaveSortingOrder;
        internal float CameraShakeStrength => cameraShakeStrength;
        internal float CameraShakeDuration => cameraShakeDuration;

        public override bool UsesOwnerAttackAnimation => false;
        internal LayerMask TargetMask => whatIsTarget;
        internal int VisualOrder => visualSortingOrder;
        protected override UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            StopOwner(owner);

            Vector2 landingPosition = Landing.ResolveLandingPosition(owner);
            PianoBossHumanGrandPianoDropContext context = GrandPianoLanding.GetOrAddContext(owner);
            context.Store(landingPosition);
            return UniTask.CompletedTask;
        }

        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnExecutePattern(owner, ct);
        internal DamageData GetNoteDamage(Enemy owner, float multiplier) => CreateNoteDamage(owner, multiplier);
        internal Sprite NoteSprite(int index) => GetNoteSprite(index);
        internal float GetTelegraphDuration(float duration) => ResolveTelegraphDuration(duration);
    }
}
