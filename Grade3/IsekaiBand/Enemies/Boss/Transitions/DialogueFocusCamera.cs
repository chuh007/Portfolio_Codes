using System;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class DialogueFocusCamera
    {
        private readonly float _focusOrthographicSize;
        private readonly float _transitionDuration;

        private CinemachineBrain _brain;
        private CinemachineCamera _sourceCamera;
        private CinemachineCamera _focusCamera;
        private CinemachineBlendDefinition _previousBlend;
        private bool _hasPreviousBlend;
        public CinemachineCamera Camera => _focusCamera;
        public CinemachineCamera Source => _sourceCamera;

        public DialogueFocusCamera(float focusOrthographicSize, float transitionDuration)
        {
            _focusOrthographicSize = Mathf.Max(1f, focusOrthographicSize);
            _transitionDuration = Mathf.Max(0f, transitionDuration);
        }

        public bool EnsureCamera()
        {
            if (_focusCamera != null)
                return true;

            Camera mainCamera = UnityEngine.Camera.main;
            _brain = mainCamera != null
                ? mainCamera.GetComponent<CinemachineBrain>()
                : Object.FindAnyObjectByType<CinemachineBrain>();
            _sourceCamera = _brain?.ActiveVirtualCamera as CinemachineCamera
                            ?? Object.FindAnyObjectByType<CinemachineCamera>();
            if (mainCamera == null || _sourceCamera == null)
                return false;

            GameObject cameraObject = new("PianoBossDialogueCamera");
            _focusCamera = cameraObject.AddComponent<CinemachineCamera>();
            cameraObject.AddComponent<CinemachinePixelPerfect>();
            _focusCamera.OutputChannel = _sourceCamera.OutputChannel;
            _focusCamera.Priority = (int)_sourceCamera.Priority - 10;
            cameraObject.transform.SetPositionAndRotation(
                mainCamera.transform.position,
                mainCamera.transform.rotation);

            LensSettings lens = LensSettings.FromCamera(mainCamera);
            lens.OrthographicSize = _focusOrthographicSize;
            lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            _focusCamera.Lens = lens;

            if (_brain != null)
            {
                _previousBlend = _brain.DefaultBlend;
                _hasPreviousBlend = true;
                _brain.DefaultBlend = new CinemachineBlendDefinition(
                    CinemachineBlendDefinition.Styles.EaseInOut,
                    _transitionDuration);
            }

            return true;
        }

        public void Dispose()
        {
            if (_focusCamera != null)
                Object.Destroy(_focusCamera.gameObject);

            _focusCamera = null;
            _sourceCamera = null;

            if (_brain != null && _hasPreviousBlend)
                _brain.DefaultBlend = _previousBlend;

            _brain = null;
            _hasPreviousBlend = false;
        }
    }
}
