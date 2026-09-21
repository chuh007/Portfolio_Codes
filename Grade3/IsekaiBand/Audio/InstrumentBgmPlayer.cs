using UnityEngine;
using UnityEngine.Audio;

namespace _Work.CHUH.Code.Audio
{
    internal class InstrumentBgmPlayer
    {
        private const string MixerGainParameter = "InstrumentBgmGain";
        private readonly InstrumentBgmSO _data;
        private readonly SoundTrackFactory _factory;
        private readonly AudioMixerGroup _output;
        private readonly InstrumentBgmOwnership _ownership = new();
        private InstrumentBgmTrack[] _tracks;
        private float[] _levels;
        private float _volume = 1f;
        private float _ensembleLevel = 1f;
        private float _gainHeadroom = 1f;
        private bool _scheduled;
        private bool _loadFailed;
        private double _startTime;

        public bool IsPlaying { get; private set; }
        public InstrumentBgmOwnership Ownership => _ownership;

        public InstrumentBgmPlayer(InstrumentBgmSO data, SoundTrackFactory factory, AudioMixerGroup output)
        {
            _data = data;
            _factory = factory;
            _output = output;
        }

        public bool TryPlay(string soundKey)
        {
            if (_data == null || !_data.Matches(soundKey) || _loadFailed) return false;
            if (IsPlaying) return true;
            if (!_data.ValidateClips())
            {
                Fail("악기와 클립을 확인해 주세요. 모든 트랙의 길이·샘플레이트·채널 수가 같아야 합니다.");
                return false;
            }

            if (!PrepareOutput()) return false;
            if (_tracks == null) CreateTracks();
            IsPlaying = true;
            Tick(0f);
            return IsPlaying;
        }

        private bool PrepareOutput()
        {
            _gainHeadroom = _data.GetMaximumVolume();
            if (_data.Output == null && _gainHeadroom <= 1f) return true;
            if (_data.Output != null && _output != null
                && _data.Output.audioMixer == _output.audioMixer
                && _data.Output.audioMixer.SetFloat(MixerGainParameter, Mathf.Log10(_gainHeadroom) * 20f))
                return true;

            Fail("1을 넘는 악기 음량을 재생할 BGM 믹서 그룹과 InstrumentBgmGain 매개변수를 확인해 주세요.");
            return false;
        }

        private void CreateTracks()
        {
            _tracks = new InstrumentBgmTrack[_data.Count];
            _levels = new float[_data.Count];
            for (int i = 0; i < _tracks.Length; i++)
            {
                _tracks[i] = new InstrumentBgmTrack(_data.GetStem(i), _factory,
                    _data.Output != null ? _data.Output : _output, _data.VolumePerUpgrade);
            }
        }

        public void Tick(float deltaTime)
        {
            if (!IsPlaying) return;

            int owned = _ownership.Read();
            float audibleCount = 0f;
            float step = _data.TransitionDuration > 0f ? deltaTime / _data.TransitionDuration : 1f;
            for (int i = 0; i < _tracks.Length; i++)
            {
                int bit = InstrumentBgmOwnership.GetInstrumentBit(_data.GetStem(i).instrument);
                float target = (owned & bit) != 0 ? 1f : 0f;
                _levels[i] = _scheduled ? Mathf.MoveTowards(_levels[i], target, step) : target;
                audibleCount += _levels[i];
                if ((!_scheduled || target > 0f)
                    && !_tracks[i].Prepare(_ownership.GetCollectedPartCount(_data.GetStem(i).instrument)))
                {
                    Fail("악기 편곡을 불러오지 못했습니다.");
                    return;
                }
                if (_scheduled)
                    _tracks[i].Tick(AudioSettings.dspTime, _startTime, _data.TransitionDuration);
            }
            // 사라지는 악기까지 음량에 반영해 독주 볼륨이 너무 일찍 커지지 않게 한다.
            _ensembleLevel = _data.GetEnsembleVolume(audibleCount);
            ApplyVolume(_volume);
            if (_scheduled) return;

            foreach (InstrumentBgmTrack track in _tracks)
            {
                if (!track.IsReady) return;
            }

            // 소유하지 않은 트랙도 같은 시계로 진행시켜 중간에 합류해도 박자가 맞게 한다.
            _startTime = AudioSettings.dspTime + InstrumentBgmTrack.ScheduleLeadTime;
            foreach (InstrumentBgmTrack track in _tracks)
                track.Start(_startTime);
            _scheduled = true;
        }

        public void ApplyVolume(float volume)
        {
            _volume = volume;
            if (_tracks == null) return;
            // 소스는 0~1로 유지하고 전용 믹서에서 배율을 복원해 강화 음량이 잘리지 않게 한다.
            for (int i = 0; i < _tracks.Length; i++)
                _tracks[i].ApplyVolume(_levels[i] * _ensembleLevel * volume / _gainHeadroom);
        }

        public void Stop()
        {
            IsPlaying = false;
            _scheduled = false;
            if (_tracks == null) return;
            foreach (InstrumentBgmTrack track in _tracks)
                track.Stop();
        }

        private void Fail(string message)
        {
            Stop();
            _loadFailed = true;
            Debug.LogWarning($"[InstrumentBgmPlayer] {message}", _data);
        }
    }
}
