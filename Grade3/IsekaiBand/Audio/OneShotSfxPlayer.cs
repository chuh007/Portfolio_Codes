using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class OneShotSfxPlayer
    {
        private readonly Dictionary<float, AudioSource> _pitchedTracks = new();
        private readonly Dictionary<string, int> _lastFrameByKey = new();
        private readonly Dictionary<string, float> _lastPlaybackTimeByKey = new();
        private readonly Dictionary<string, List<double>> _voiceEndTimesByKey = new();
        private readonly SoundTrackFactory _tracks;
        private readonly SoundVolumeController _volumes;
        private readonly AudioMixerGroup _output;
        private readonly AudioSource _track;

        public OneShotSfxPlayer(SoundTrackFactory tracks, SoundCatalogSO catalog, SoundVolumeController volumes)
        {
            _tracks = tracks;
            _volumes = volumes;
            _output = catalog != null ? catalog.SfxOutput : null;
            _track = tracks.Create("SFX Track", false, _output);
        }

        public void Play(SoundCatalogEntry entry, SoundPlayEvent evt)
        {
            if (entry.Volume <= 0f) return;

            float pitch = SoundTrackFactory.NormalizePitch(evt.PitchMultiplier);
            if (evt.SoundType == SoundType.SFX && ShouldSuppress(entry, evt, pitch)) return;

            AudioSource track = Mathf.Approximately(pitch, 1f) ? _track : GetPitchedTrack(pitch);
            track.PlayOneShot(entry.Clip, entry.Volume * Mathf.Max(0f, evt.VolumeMultiplier));
        }

        private bool ShouldSuppress(SoundCatalogEntry entry, SoundPlayEvent evt, float pitch)
        {
            int currentFrame = Time.frameCount;
            bool suppress = evt.SuppressDuplicateThisFrame
                            && _lastFrameByKey.TryGetValue(evt.SoundKey, out int lastPlayedFrame)
                            && lastPlayedFrame == currentFrame;
            float suppressionWindow = Mathf.Max(entry.MinimumPlaybackInterval,
                Mathf.Max(0f, evt.DuplicateSuppressionWindowSeconds));
            float playbackTime = Time.unscaledTime;
            if (suppressionWindow > 0f
                && _lastPlaybackTimeByKey.TryGetValue(evt.SoundKey, out float lastPlaybackTime)
                && playbackTime - lastPlaybackTime < suppressionWindow)
                suppress = true;

            if (suppress) return true;
            if (!TryReserveVoice(entry, evt.SoundKey, pitch)) return true;

            _lastFrameByKey[evt.SoundKey] = currentFrame;
            _lastPlaybackTimeByKey[evt.SoundKey] = playbackTime;
            return false;
        }

        private bool TryReserveVoice(SoundCatalogEntry entry, string soundKey, float pitch)
        {
            if (entry.MaxSimultaneousVoices <= 0) return true;

            if (!_voiceEndTimesByKey.TryGetValue(soundKey, out var endTimes))
            {
                endTimes = new List<double>();
                _voiceEndTimesByKey.Add(soundKey, endTimes);
            }

            // 오디오 시간과 피치를 기준으로 이미 끝난 재생만 비운다.
            double now = AudioSettings.dspTime;
            for (int i = endTimes.Count - 1; i >= 0; i--)
            {
                if (endTimes[i] <= now)
                    endTimes.RemoveAt(i);
            }
            if (endTimes.Count >= entry.MaxSimultaneousVoices) return false;

            endTimes.Add(now + entry.Clip.length / pitch);
            return true;
        }

        private AudioSource GetPitchedTrack(float pitch)
        {
            if (_pitchedTracks.TryGetValue(pitch, out AudioSource track)) return track;

            track = _tracks.Create($"SFX Track (Pitch {pitch:0.###})", false, _output);
            track.pitch = pitch;
            track.volume = _volumes.SfxTrackVolume;
            _pitchedTracks.Add(pitch, track);
            return track;
        }

        public void ApplyVolume()
        {
            float volume = _volumes.SfxTrackVolume;
            _track.volume = volume;
            foreach (AudioSource track in _pitchedTracks.Values)
            {
                if (track != null)
                    track.volume = volume;
            }
        }
    }
}
