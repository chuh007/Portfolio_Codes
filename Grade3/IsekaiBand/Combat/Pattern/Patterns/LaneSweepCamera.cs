using System.Threading;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LaneSweepCamera
    {
        private readonly Phase2LaneSweepPatternSO _pattern;
        public CinemachineBrain Brain;
        private CinemachineCamera _sourceCamera;
        private CinemachineCamera _zoomCamera;
        public LaneSweepCamera(Phase2LaneSweepPatternSO pattern) => _pattern = pattern;

        public async UniTask ZoomCameraToArena(CancellationToken ct)
        {
            Camera mainCamera = Camera.main;
            _pattern.Camera.Brain = mainCamera != null
                ? mainCamera.GetComponent<CinemachineBrain>()
                : Object.FindAnyObjectByType<CinemachineBrain>();
            _sourceCamera = GetActiveCinemachineCamera(_pattern.Camera.Brain);
            if (_sourceCamera == null || mainCamera == null)
                return;

            float targetSize = Mathf.Max(
                _pattern.Plan.ArenaBounds.height * 0.5f,
                _pattern.Plan.ArenaBounds.width / (2f * Mathf.Max(0.01f, mainCamera.aspect))) + _pattern.CameraPadding;

            _zoomCamera = CreateZoomCamera(mainCamera, targetSize);
            _pattern.Blend.ApplyBrainBlendDuration(_pattern.CameraZoomDuration);
            _zoomCamera.Priority = (int)_sourceCamera.Priority + 10;

            await LaneSweepBlend.WaitForCinemachineBlend(_pattern.CameraZoomDuration, ct);
        }

        public async UniTask RestoreCamera()
        {
            if (_zoomCamera == null)
                return;

            _pattern.Blend.ApplyBrainBlendDuration(_pattern.CameraRestoreDuration);
            _zoomCamera.Priority = (int)(_sourceCamera != null ? _sourceCamera.Priority : 0) - 10;

            await LaneSweepBlend.WaitForCinemachineBlend(_pattern.CameraRestoreDuration, CancellationToken.None);
            Object.Destroy(_zoomCamera.gameObject);
            _zoomCamera = null;
            _sourceCamera = null;
            _pattern.Blend.RestoreBrainBlendDuration();
        }

        public CinemachineCamera CreateZoomCamera(Camera mainCamera, float targetSize)
        {
            var cameraObject = new GameObject("Phase2LaneSweepZoomCamera");
            var zoomCamera = cameraObject.AddComponent<CinemachineCamera>();
            cameraObject.AddComponent<CinemachinePixelPerfect>();
            zoomCamera.OutputChannel = _sourceCamera.OutputChannel;
            zoomCamera.Priority = (int)_sourceCamera.Priority - 10;

            Vector3 cameraPosition = mainCamera.transform.position;
            cameraObject.transform.SetPositionAndRotation(
                new Vector3(_pattern.Plan.ArenaBounds.center.x, _pattern.Plan.ArenaBounds.center.y, cameraPosition.z),
                mainCamera.transform.rotation);

            LensSettings lens = LensSettings.FromCamera(mainCamera);
            lens.OrthographicSize = targetSize;
            lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            zoomCamera.Lens = lens;
            return zoomCamera;
        }

        public static CinemachineCamera GetActiveCinemachineCamera(CinemachineBrain brain)
        {
            if (brain != null && brain.ActiveVirtualCamera is CinemachineCamera activeCamera)
                return activeCamera;

            return Object.FindAnyObjectByType<CinemachineCamera>();
        }
    }
}
