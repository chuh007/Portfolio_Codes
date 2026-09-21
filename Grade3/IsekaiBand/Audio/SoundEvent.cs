using Chuh007Lib.Bus;

namespace _Work.CHUH.Code.Audio
{
    public enum SoundType
    {
        SFX,
        BGM
    }

    public readonly struct SoundPlayEvent : IEvent
    {
        public SoundPlayEvent(
            string soundKey,
            SoundType soundType,
            float volumeMultiplier = 1f,
            float pitchMultiplier = 1f,
            bool suppressDuplicateThisFrame = false,
            float duplicateSuppressionWindowSeconds = 0f)
        {
            SoundKey = soundKey;
            SoundType = soundType;
            VolumeMultiplier = volumeMultiplier;
            PitchMultiplier = pitchMultiplier;
            SuppressDuplicateThisFrame = suppressDuplicateThisFrame;
            DuplicateSuppressionWindowSeconds = duplicateSuppressionWindowSeconds;
        }

        public string SoundKey { get; }
        public SoundType SoundType { get; }
        public float VolumeMultiplier { get; }
        public float PitchMultiplier { get; }
        public bool SuppressDuplicateThisFrame { get; }
        public float DuplicateSuppressionWindowSeconds { get; }
    }

    public readonly struct SoundLoopStartEvent : IEvent
    {
        public SoundLoopStartEvent(
            object playbackKey,
            string soundKey,
            float volumeMultiplier = 1f,
            float pitchMultiplier = 1f)
        {
            PlaybackKey = playbackKey;
            SoundKey = soundKey;
            VolumeMultiplier = volumeMultiplier;
            PitchMultiplier = pitchMultiplier;
        }

        public object PlaybackKey { get; }
        public string SoundKey { get; }
        public float VolumeMultiplier { get; }
        public float PitchMultiplier { get; }
    }

    public readonly struct SoundLoopStopEvent : IEvent
    {
        public SoundLoopStopEvent(object playbackKey)
        {
            PlaybackKey = playbackKey;
        }

        public object PlaybackKey { get; }
    }
}
