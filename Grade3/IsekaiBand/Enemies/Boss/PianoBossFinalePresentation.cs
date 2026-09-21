using System.Threading;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Tutorial;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    [DisallowMultipleComponent]
    public sealed class PianoBossFinalePresentation : MonoBehaviour,
        IBossDeathCompletionPresentation
    {
        [SerializeField] private TutorialDialogueView dialogueView;

        [Header("Dialogue")]
        [SerializeField] private PianoBossDialogueSequenceSO dialogueSequence;

        [Header("Camera")]
        [SerializeField, Min(1f)] private float focusOrthographicSize = 3.5f;
        [SerializeField, Min(0f)] private float cameraTransitionDuration = 0.4f;

        [Header("Audio")]
        [SerializeField, Min(0f)] private float bgmFadeOutDuration = 1.15f;

        private CancellationTokenSource _deathCts;
        private UniTask<bool> _bgmFadeTask;
        private PlayerMovementCompo _lockedMovement;
        private DialogueCameraFocus _cameraFocus;
        private bool _fadeStarted;
        private bool _isPlaying;

        public void OnDeathStarted(CancellationToken cancellationToken)
        {
            if (_fadeStarted)
                return;

            _fadeStarted = true;
            _deathCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _bgmFadeTask = SoundManager.FadeOutBgmAsync(
                bgmFadeOutDuration,
                _deathCts.Token);
        }

        public async UniTask<bool> PlayBeforeCompletionAsync(
            CancellationToken cancellationToken)
        {
            if (_isPlaying)
                return false;

            if (!_fadeStarted)
                OnDeathStarted(cancellationToken);

            if (dialogueView == null)
                dialogueView = GetComponent<TutorialDialogueView>();

            _isPlaying = true;
            CancellationToken playToken = _deathCts?.Token ?? cancellationToken;

            try
            {
                if (dialogueView != null
                    && dialogueSequence != null
                    && dialogueSequence.HasLines)
                {
                    Player player = FindAnyObjectByType<Player>();
                    _lockedMovement = player != null
                        ? player.GetComponent<PlayerMovementCompo>()
                        : null;
                    _lockedMovement?.LockInput();

                    _cameraFocus = new DialogueCameraFocus(
                        focusOrthographicSize,
                        cameraTransitionDuration);

                    bool canceled = await PianoBossDialogueSequencePlayer.PlayAsync(
                        dialogueSequence,
                        dialogueView,
                        player != null ? player.transform : null,
                        transform,
                        _cameraFocus,
                        playToken);
                    if (canceled)
                        return true;
                    await _cameraFocus.RestoreAsync();
                    _cameraFocus = null;
                }
                else
                {
                    Debug.LogWarning(
                        $"[{nameof(PianoBossFinalePresentation)}] Dialogue references were not found.",
                        this);
                }

                return _fadeStarted && await _bgmFadeTask;
            }
            finally
            {
                EndPresentation();
            }
        }

        private void OnEnable()
        {
            _fadeStarted = false;
            _bgmFadeTask = default;
        }

        private void OnDisable()
        {
            _deathCts?.Cancel();
            EndPresentation();
        }

        private void EndPresentation()
        {
            _isPlaying = false;
            dialogueView?.Hide();
            _cameraFocus?.Dispose();
            _cameraFocus = null;
            _lockedMovement?.UnlockInput();
            _lockedMovement = null;
            _deathCts?.Cancel();
            _deathCts?.Dispose();
            _deathCts = null;
        }
    }
}
