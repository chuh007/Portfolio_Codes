using System;
using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class InstrumentBgmTrack
    {
        public const double ScheduleLeadTime = 0.1;
        private readonly InstrumentBgmStem _stem;
        private readonly SoundTrackFactory _factory;
        private readonly AudioMixerGroup _output;
        private readonly float _volumePerUpgrade;
        private AudioSource _current;
        private AudioSource _next;
        private AudioClip _requestedClip;
        private bool _transitioning;
        private double _transitionStart;
        private float _blend;
        private float _requestedVolume;
        private float _currentVolume;
        private float _nextVolume;

        public bool IsReady => _requestedClip != null && _requestedClip.loadState == AudioDataLoadState.Loaded;

        public InstrumentBgmTrack(InstrumentBgmStem stem, SoundTrackFactory factory, AudioMixerGroup output,
            float volumePerUpgrade)
        {
            _stem = stem;
            _factory = factory;
            _output = output;
            _volumePerUpgrade = volumePerUpgrade;
            _current = CreateSource($"BGM {stem.instrument}");
        }

        public bool Prepare(int collectedParts)
        {
            _requestedClip = _stem.GetClip(collectedParts);
            _requestedVolume = _stem.GetVolume(collectedParts, _volumePerUpgrade);
            if (_requestedClip.loadState == AudioDataLoadState.Failed) return false;
            return _requestedClip.loadState != AudioDataLoadState.Unloaded || _requestedClip.LoadAudioData();
        }

        public void Start(double startTime)
        {
            _current.clip = _requestedClip;
            _currentVolume = _requestedVolume;
            _current.timeSamples = 0;
            _current.PlayScheduled(startTime);
        }

        public void Tick(double now, double origin, float transitionDuration)
        {
            if (_transitioning)
            {
                _blend = now < _transitionStart ? 0f : transitionDuration > 0f
                    ? Mathf.Clamp01((float)((now - _transitionStart) / transitionDuration)) : 1f;
                if (_blend < 1f) return;

                _current.Stop();
                _current.clip = null;
                (_current, _next) = (_next, _current);
                _currentVolume = _nextVolume;
                _transitioning = false;
                _blend = 0f;
            }

            if (!IsReady || (_requestedClip == _current.clip
                             && Mathf.Approximately(_requestedVolume, _currentVolume))) return;
            _next ??= CreateSource($"BGM {_stem.instrument} Alternate");
            _next.clip = _requestedClip;
            _nextVolume = _requestedVolume;
            _next.volume = 0f;
            _transitionStart = Math.Max(now + ScheduleLeadTime, origin);
            // 새 편곡도 공통 시작 시각에서 흐른 샘플 수로 맞춰 곡을 이어서 연주한다.
            double samples = (_transitionStart - origin) * _requestedClip.frequency;
            _next.timeSamples = (int)(Math.Round(samples) % _requestedClip.samples);
            _next.PlayScheduled(_transitionStart);
            _transitioning = true;
        }

        public void ApplyVolume(float volume)
        {
            _current.volume = _currentVolume * volume * (1f - _blend);
            if (_next != null) _next.volume = _transitioning ? _nextVolume * volume * _blend : 0f;
        }

        public void Stop()
        {
            _current.Stop();
            _current.volume = 0f;
            if (_next != null)
            {
                _next.Stop();
                _next.volume = 0f;
            }
            _transitioning = false;
            _blend = 0f;
            _requestedClip = null;
        }

        private AudioSource CreateSource(string name)
        {
            AudioSource source = _factory.Create(name, true, _output);
            source.priority = 0;
            source.volume = 0f;
            return source;
        }
    }
}
