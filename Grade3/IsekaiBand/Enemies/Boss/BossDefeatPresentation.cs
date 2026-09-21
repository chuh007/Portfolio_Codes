using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    [Serializable]
    public sealed class BossDefeatPresentation
    {
        private const int ImpulseChannel = 1 << 29;

        [SerializeField, Min(0f)] private float duration = 1.15f;
        [SerializeField, Range(0.01f, 1f)] private float timeScale = 0.2f;
        [SerializeField, Min(0f)] private float shakeStrength = 0.65f;
        [SerializeField, Min(1)] private int shakePulseCount = 1;

        public async UniTask<bool> PlayAsync(Vector3 sourcePosition, CancellationToken cancellationToken)
        {
            float presentationDuration = Mathf.Max(0f, duration);
            if (presentationDuration <= 0f)
                return false;

            EnsureImpulseListeners();

            float previousTimeScale = Time.timeScale;
            float previousFixedDeltaTime = Time.fixedDeltaTime;
            float appliedTimeScale = Mathf.Min(previousTimeScale, Mathf.Clamp(timeScale, 0.01f, 1f));

            CinemachineImpulseManager impulseManager = CinemachineImpulseManager.Instance;
            int pulseCount = Mathf.Max(1, shakePulseCount);
            float pulseDuration = presentationDuration / pulseCount;
            float impulseDuration = impulseManager.IgnoreTimeScale
                ? pulseDuration
                : pulseDuration * appliedTimeScale;
            var impulses = new List<CinemachineImpulseManager.ImpulseEvent>(pulseCount * 2);

            try
            {
                Time.timeScale = appliedTimeScale;
                if (previousTimeScale > 0f)
                    Time.fixedDeltaTime = previousFixedDeltaTime * (appliedTimeScale / previousTimeScale);

                for (int i = 0; i < pulseCount; i++)
                {
                    float horizontalSign = i % 2 == 0 ? 1f : -1f;
                    float verticalSign = i % 4 < 2 ? 1f : -1f;
                    impulses.Add(CreateImpulse(
                        sourcePosition,
                        Vector3.right * (shakeStrength * horizontalSign),
                        impulseDuration,
                        CinemachineImpulseDefinition.ImpulseShapes.Explosion));
                    impulses.Add(CreateImpulse(
                        sourcePosition,
                        Vector3.up * (shakeStrength * 0.75f * verticalSign),
                        impulseDuration,
                        CinemachineImpulseDefinition.ImpulseShapes.Rumble));

                    bool canceled = await UniTask.Delay(
                            TimeSpan.FromSeconds(pulseDuration),
                            DelayType.UnscaledDeltaTime,
                            PlayerLoopTiming.Update,
                            cancellationToken)
                        .SuppressCancellationThrow();
                    if (canceled)
                        return true;
                }

                return false;
            }
            finally
            {
                float currentImpulseTime = impulseManager.CurrentTime;
                for (int i = 0; i < impulses.Count; i++)
                    impulses[i]?.Cancel(currentImpulseTime, true);
                Time.fixedDeltaTime = previousFixedDeltaTime;
                Time.timeScale = previousTimeScale;
            }
        }

        private static CinemachineImpulseManager.ImpulseEvent CreateImpulse(
            Vector3 sourcePosition,
            Vector3 velocity,
            float impulseDuration,
            CinemachineImpulseDefinition.ImpulseShapes shape)
        {
            var definition = new CinemachineImpulseDefinition
            {
                ImpulseChannel = ImpulseChannel,
                ImpulseShape = shape,
                ImpulseDuration = impulseDuration,
                ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform
            };

            return definition.CreateAndReturnEvent(sourcePosition, velocity);
        }

        private static void EnsureImpulseListeners()
        {
            CinemachineCamera[] cameras = UnityEngine.Object.FindObjectsByType<CinemachineCamera>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

            foreach (CinemachineCamera camera in cameras)
            {
                CinemachineImpulseListener dedicatedListener = null;
                CinemachineImpulseListener[] listeners = camera.GetComponents<CinemachineImpulseListener>();
                foreach (CinemachineImpulseListener listener in listeners)
                {
                    if (listener.ChannelMask != ImpulseChannel)
                        continue;

                    dedicatedListener = listener;
                    break;
                }

                if (dedicatedListener == null)
                    dedicatedListener = camera.gameObject.AddComponent<CinemachineImpulseListener>();

                dedicatedListener.ApplyAfter = CinemachineCore.Stage.Noise;
                dedicatedListener.ChannelMask = ImpulseChannel;
                dedicatedListener.Gain = 1f;
                dedicatedListener.Use2DDistance = true;
                dedicatedListener.UseCameraSpace = true;
                dedicatedListener.SignalCombinationMode =
                    CinemachineImpulseListener.SignalCombinationModes.Additive;
            }
        }
    }
}
