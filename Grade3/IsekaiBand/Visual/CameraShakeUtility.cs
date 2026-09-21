using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Visual
{
    public static class CameraShakeUtility
    {
        private const int ImpactImpulseChannel = 1 << 28;

        public static void PlayImpact(Vector3 sourcePosition, float strength, float duration)
        {
            float safeStrength = Mathf.Max(0f, strength);
            float safeDuration = Mathf.Max(0f, duration);
            if (safeStrength <= 0f || safeDuration <= 0f)
                return;

            EnsureImpulseListeners();

            var definition = new CinemachineImpulseDefinition
            {
                ImpulseChannel = ImpactImpulseChannel,
                ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble,
                ImpulseDuration = safeDuration,
                ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform
            };

            definition.CreateAndReturnEvent(
                sourcePosition,
                new Vector3(safeStrength, safeStrength * 0.55f, 0f));
        }

        private static void EnsureImpulseListeners()
        {
            CinemachineCamera[] cameras = Object.FindObjectsByType<CinemachineCamera>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

            foreach (CinemachineCamera camera in cameras)
            {
                CinemachineImpulseListener listener = FindDedicatedListener(camera);
                if (listener == null)
                    listener = camera.gameObject.AddComponent<CinemachineImpulseListener>();

                listener.ApplyAfter = CinemachineCore.Stage.Noise;
                listener.ChannelMask = ImpactImpulseChannel;
                listener.Gain = 1f;
                listener.Use2DDistance = true;
                listener.UseCameraSpace = true;
                listener.SignalCombinationMode =
                    CinemachineImpulseListener.SignalCombinationModes.Additive;
            }
        }

        private static CinemachineImpulseListener FindDedicatedListener(CinemachineCamera camera)
        {
            CinemachineImpulseListener[] listeners = camera.GetComponents<CinemachineImpulseListener>();
            foreach (CinemachineImpulseListener listener in listeners)
            {
                if (listener.ChannelMask == ImpactImpulseChannel)
                    return listener;
            }

            return null;
        }
    }
}
