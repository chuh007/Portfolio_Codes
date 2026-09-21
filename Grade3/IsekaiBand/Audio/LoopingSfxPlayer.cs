using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class LoopingSfxPlayer
    {
        private readonly struct Playback
        {
            public Playback(AudioSource track, float gain)
            {
                Track = track;
                Gain = gain;
            }

            public AudioSource Track { get; }
            public float Gain { get; }
        }

        private readonly Dictionary<object, Playback> _playbacks = new();
        private readonly Stack<AudioSource> _trackPool = new();
        private readonly SoundTrackFactory _tracks;
        private readonly SoundVolumeController _volumes;
        private readonly AudioMixerGroup _output;

        public LoopingSfxPlayer(SoundTrackFactory tracks, SoundCatalogSO catalog, SoundVolumeController volumes)
        {
            _tracks = tracks;
            _volumes = volumes;
            _output = catalog != null ? catalog.SfxOutput : null;
        }

        public void Play(SoundCatalogEntry entry, SoundLoopStartEvent evt)
        {
            Stop(evt.PlaybackKey);
            if (entry.Volume <= 0f) return;

            AudioSource track = AcquireTrack();
            float gain = entry.Volume * Mathf.Max(0f, evt.VolumeMultiplier);
            track.clip = entry.Clip;
            track.pitch = SoundTrackFactory.NormalizePitch(evt.PitchMultiplier);
            track.volume = gain * _volumes.SfxTrackVolume;
            track.Play();
            _playbacks.Add(evt.PlaybackKey, new Playback(track, gain));
        }

        public void Stop(object playbackKey)
        {
            if (_playbacks.Remove(playbackKey, out Playback playback))
                ReleaseTrack(playback.Track);
        }

        public void StopAll()
        {
            foreach (Playback playback in _playbacks.Values)
                ReleaseTrack(playback.Track);
            _playbacks.Clear();
        }

        public void ApplyVolume()
        {
            foreach (Playback playback in _playbacks.Values)
            {
                if (playback.Track != null)
                    playback.Track.volume = playback.Gain * _volumes.SfxTrackVolume;
            }
        }

        private AudioSource AcquireTrack()
        {
            while (_trackPool.Count > 0)
            {
                AudioSource track = _trackPool.Pop();
                if (track != null) return track;
            }
            return _tracks.Create("Looping SFX Track", true, _output);
        }

        private void ReleaseTrack(AudioSource track)
        {
            if (track == null) return;

            track.Stop();
            track.clip = null;
            track.pitch = 1f;
            _trackPool.Push(track);
        }
    }
}
