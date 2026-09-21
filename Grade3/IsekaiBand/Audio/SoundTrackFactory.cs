using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class SoundTrackFactory
    {
        private const float MinimumPitch = 0.01f;
        private const float MaximumPitch = 3f;
        private const float PitchTrackPrecision = 1000f;
        private readonly Transform _owner;

        public SoundTrackFactory(Transform owner) => _owner = owner;

        public AudioSource Create(string trackName, bool loop, AudioMixerGroup output)
        {
            var trackObject = new GameObject(trackName);
            trackObject.transform.SetParent(_owner, false);

            AudioSource source = trackObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = output;
            return source;
        }

        public static float NormalizePitch(float pitchMultiplier)
        {
            if (float.IsNaN(pitchMultiplier) || float.IsInfinity(pitchMultiplier))
                return 1f;

            float clampedPitch = Mathf.Clamp(pitchMultiplier, MinimumPitch, MaximumPitch);
            return Mathf.Round(clampedPitch * PitchTrackPrecision) / PitchTrackPrecision;
        }
    }
}
