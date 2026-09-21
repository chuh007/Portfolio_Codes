using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaPixelPerfectOverride
    {
        private CinemachinePixelPerfect _pixelPerfectCorrection;
        private bool _hasPixelPerfectCorrectionState;
        private bool _wasPixelPerfectCorrectionEnabled;

        public void Suspend(CinemachineCamera camera)
        {
            if (camera == null || _hasPixelPerfectCorrectionState)
                return;

            _pixelPerfectCorrection = camera.GetComponent<CinemachinePixelPerfect>();
            if (_pixelPerfectCorrection == null)
                return;

            _wasPixelPerfectCorrectionEnabled = _pixelPerfectCorrection.enabled;
            _hasPixelPerfectCorrectionState = true;
            _pixelPerfectCorrection.enabled = false;
        }

        public void Restore()
        {
            if (!_hasPixelPerfectCorrectionState)
                return;

            if (_pixelPerfectCorrection != null)
                _pixelPerfectCorrection.enabled = _wasPixelPerfectCorrectionEnabled;

            _pixelPerfectCorrection = null;
            _hasPixelPerfectCorrectionState = false;
        }
    }
}
