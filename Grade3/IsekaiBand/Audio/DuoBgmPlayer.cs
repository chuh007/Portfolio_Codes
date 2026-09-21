using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.WeaponCombine;
using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class DuoBgmPlayer
    {
        private readonly DuoBgmSO _data;
        private readonly SoundTrackFactory _factory;
        private readonly AudioMixerGroup _output;
        private readonly HashSet<AudioClip> _failedClips = new();
        private PlayerAttackCompo _player;
        private CombineWeaponController _combinations;
        private AudioSource _current;
        private AudioSource _outgoing;
        private float _currentVolume;
        private float _outgoingVolume;
        private float _blend = 1f;
        private float _volume;
        private float _nextPlayerSearchTime;

        public bool HasSelection { get; private set; }
        public bool IsPlaying => _current != null && _current.isPlaying;
        public float TransitionDuration => _data.TransitionDuration;

        public DuoBgmPlayer(DuoBgmSO data, SoundTrackFactory factory, AudioMixerGroup output)
        {
            _data = data;
            _factory = factory;
            _output = output;
        }

        public void SetPlayer(PlayerAttackCompo player)
        {
            if (_player == player) return;
            _player = player;
            _combinations = null;
        }

        public void Tick(float deltaTime)
        {
            HasSelection = TryGetSelection(out DuoBgmEntry entry);
            if (HasSelection && Prepare(entry.clip))
                Play(entry);

            float step = TransitionDuration > 0f ? deltaTime / TransitionDuration : 1f;
            _blend = Mathf.MoveTowards(_blend, 1f, step);
            if (_blend >= 1f && _outgoing != null)
            {
                _outgoing.Stop();
                _outgoing.clip = null;
            }
            ApplyVolume(_volume);
        }

        private bool TryGetSelection(out DuoBgmEntry entry)
        {
            if (_player == null && Time.unscaledTime >= _nextPlayerSearchTime)
            {
                SetPlayer(Object.FindFirstObjectByType<PlayerAttackCompo>());
                _nextPlayerSearchTime = Time.unscaledTime + 0.5f;
            }
            if (_player != null && _combinations == null)
                _combinations = _player.GetComponent<CombineWeaponController>();

            if (_combinations != null)
            {
                var order = _combinations.CreationOrder;
                for (int i = order.Count - 1; i >= 0; i--)
                {
                    if (_combinations.HasCombinedWeapon(order[i]) && _data.TryGet(order[i], out entry)
                        && !_failedClips.Contains(entry.clip))
                        return true;
                }
            }
            entry = default;
            return false;
        }

        private bool Prepare(AudioClip clip)
        {
            if (clip.loadState == AudioDataLoadState.Unloaded)
                clip.LoadAudioData();
            if (clip.loadState != AudioDataLoadState.Failed)
                return clip.loadState == AudioDataLoadState.Loaded;

            if (_failedClips.Add(clip))
                Debug.LogWarning($"[DuoBgmPlayer] '{clip.name}' 듀오 BGM을 불러오지 못했습니다.", _data);
            HasSelection = false;
            return false;
        }

        private void Play(DuoBgmEntry entry)
        {
            if (_current != null && _current.clip == entry.clip && _current.isPlaying) return;

            if (_current == null)
                _current = _factory.Create("Duo BGM Track", true, _output);
            else if (_current.isPlaying)
            {
                if (_outgoing == null)
                    _outgoing = _factory.Create("Duo BGM Transition", true, _output);
                (_current, _outgoing) = (_outgoing, _current);
                _outgoingVolume = _currentVolume * _blend;
            }

            _current.Stop();
            _current.clip = entry.clip;
            _currentVolume = Mathf.Clamp01(entry.volume);
            _blend = _outgoing != null && _outgoing.isPlaying ? 0f : 1f;
            ApplyVolume(_volume);
            _current.Play();
        }

        public void ApplyVolume(float volume)
        {
            _volume = volume;
            if (_current != null)
                _current.volume = _currentVolume * _blend * volume;
            if (_outgoing != null)
                _outgoing.volume = _outgoingVolume * (1f - _blend) * volume;
        }

        public void Stop()
        {
            HasSelection = false;
            _blend = 1f;
            if (_current != null)
            {
                _current.Stop();
                _current.clip = null;
            }
            if (_outgoing != null)
            {
                _outgoing.Stop();
                _outgoing.clip = null;
            }
        }
    }
}
