using System;
using System.Threading;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Tutorial;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Enemies.Boss
{
    [DisallowMultipleComponent]
    public sealed class PianoBossIntroDialoguePresentation : MonoBehaviour
    {
        [SerializeField] private TutorialDialogueView dialogueView;

        [Header("Dialogue")]
        [SerializeField] private PianoBossDialogueSequenceSO dialogueSequence;

        [Header("Camera")]
        [SerializeField, Min(1f)] private float focusOrthographicSize = 3.5f;
        [SerializeField, Min(0f)] private float cameraTransitionDuration = 0.4f;

        private PlayerMovementCompo _lockedMovement;
        private DialogueCameraFocus _cameraFocus;
        private CancellationTokenSource _playCts;
        private bool _isPlaying;

        public async UniTask<bool> PlayAsync(
            Transform bossTarget,
            CancellationToken cancellationToken)
        {
            if (_isPlaying)
                return false;

            if (dialogueView == null)
                dialogueView = GetComponent<TutorialDialogueView>();

            Player player = FindAnyObjectByType<Player>();
            if (dialogueView == null
                || dialogueSequence == null
                || !dialogueSequence.HasLines
                || player == null
                || bossTarget == null)
            {
                Debug.LogWarning(
                    $"[{nameof(PianoBossIntroDialoguePresentation)}] Dialogue references were not found.",
                    this);
                return false;
            }

            _isPlaying = true;
            _lockedMovement = player.GetComponent<PlayerMovementCompo>();
            _lockedMovement?.LockInput();
            _playCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CancellationToken playToken = _playCts.Token;
            _cameraFocus = new DialogueCameraFocus(
                focusOrthographicSize,
                cameraTransitionDuration);

            try
            {
                bool canceled = await PianoBossDialogueSequencePlayer.PlayAsync(
                    dialogueSequence,
                    dialogueView,
                    player.transform,
                    bossTarget,
                    _cameraFocus,
                    playToken);
                if (canceled)
                    return true;

                await _cameraFocus.RestoreAsync();
                _cameraFocus = null;
                return false;
            }
            finally
            {
                EndPresentation();
            }
        }

        private void OnDisable()
        {
            _playCts?.Cancel();
            EndPresentation();
        }

        private void EndPresentation()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;
            dialogueView?.Hide();
            _cameraFocus?.Dispose();
            _cameraFocus = null;
            _lockedMovement?.UnlockInput();
            _lockedMovement = null;
            _playCts?.Dispose();
            _playCts = null;
        }
    }

}
