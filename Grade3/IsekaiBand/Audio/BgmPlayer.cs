using System.Threading;
using _Code.LCH._02.Scripts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class BgmPlayer
    {
        private readonly AudioSource _track;
        private readonly SoundCatalogSO _catalog;
        private readonly SoundVolumeController _volumes;
        private readonly InstrumentBgmPlayer _instruments;
        private readonly DuoBgmPlayer _duos;
        private SoundCatalogEntry _instrumentEntry;
        private bool _usingInstruments;
        private bool _allowDuos;
        private float _duoLevel;
        private float _clipVolume = 1f;
        private float _fadeMultiplier = 1f;
        private int _transitionVersion;

        public BgmPlayer(SoundTrackFactory tracks, SoundCatalogSO catalog, SoundVolumeController volumes,
            InstrumentBgmSO instrumentBgm = null, DuoBgmSO duoBgm = null)
        {
            _volumes = volumes;
            _catalog = catalog;
            _track = tracks.Create("BGM Track", true, catalog != null ? catalog.BgmOutput : null);
            if (instrumentBgm != null)
                _instruments = new InstrumentBgmPlayer(instrumentBgm, tracks, catalog != null ? catalog.BgmOutput : null);
            if (duoBgm != null)
                _duos = new DuoBgmPlayer(duoBgm, tracks, catalog != null ? catalog.BgmOutput : null);
        }

        public void Play(SoundCatalogEntry entry)
        {
            _transitionVersion++;
            _fadeMultiplier = 1f;
            _allowDuos = entry.Key == SoundKeys.InGameBgm;
            if (!_allowDuos)
            {
                _duoLevel = 0f;
                _duos?.Stop();
            }
            else
            {
                _instrumentEntry = entry;
                _duos?.Tick(0f);
            }
            if (_instruments != null && _instruments.TryPlay(entry.Key))
            {
                _usingInstruments = true;
                _instrumentEntry = entry;
                _clipVolume = entry.Volume;
                _track.Stop();
                _track.clip = null;
                ApplyVolume();
                return;
            }

            _usingInstruments = false;
            if (!TryPlayBossInstruments(entry.Key))
                _instruments?.Stop();
            if (_track.clip == entry.Clip && _track.isPlaying)
            {
                ApplyVolume();
                return;
            }

            _track.Stop();
            _track.clip = entry.Clip;
            _clipVolume = entry.Volume;
            ApplyVolume();
            _track.Play();
        }

        private bool TryPlayBossInstruments(string soundKey)
        {
            bool isBoss = soundKey == SoundKeys.Boss1Bgm || soundKey == SoundKeys.PianoBossPhase1Bgm
                          || soundKey == SoundKeys.PianoBossPhase2Bgm;
            if (!isBoss || _instruments == null || _catalog == null
                || !_catalog.TryGet(SoundKeys.InGameBgm, out SoundCatalogEntry entry)
                || !_instruments.TryPlay(entry.Key)) return false;

            _instrumentEntry = entry;
            return true;
        }

        public void Stop()
        {
            _transitionVersion++;
            _fadeMultiplier = 1f;
            _usingInstruments = false;
            _allowDuos = false;
            _duoLevel = 0f;
            _duos?.Stop();
            _instruments?.Stop();
            _track.Stop();
            _track.clip = null;
        }

        public void ApplyVolume()
        {
            float volume = _volumes.BgmTrackVolume * _fadeMultiplier;
            _instruments?.ApplyVolume(_instrumentEntry.Volume * volume * (1f - _duoLevel));
            _duos?.ApplyVolume(_instrumentEntry.Volume * volume * _duoLevel);
            if (_track != null)
                _track.volume = _clipVolume * volume * (1f - _duoLevel);
        }

        public void SetPlayer(PlayerAttackCompo player)
        {
            _instruments?.Ownership.SetPlayer(player);
            _duos?.SetPlayer(player);
        }

        public void Tick(float deltaTime)
        {
            if (_allowDuos && _duos != null)
            {
                _duos.Tick(deltaTime);
                float target = _duos.HasSelection && _duos.IsPlaying ? 1f : 0f;
                float step = _duos.TransitionDuration > 0f ? deltaTime / _duos.TransitionDuration : 1f;
                _duoLevel = Mathf.MoveTowards(_duoLevel, target, step);
                if (target == 0f && _duoLevel <= 0f)
                    _duos.Stop();
                ApplyVolume();
            }
            if (_instruments == null || !_instruments.IsPlaying) return;

            _instruments.Tick(deltaTime);
            if (_usingInstruments && !_instruments.IsPlaying)
                Play(_instrumentEntry);
        }

        public async UniTask<bool> FadeOutAsync(float duration, CancellationToken cancellationToken)
        {
            if (!_usingInstruments && (_track == null || !_track.isPlaying)) return false;

            int transitionVersion = ++_transitionVersion;
            float startMultiplier = _fadeMultiplier;
            float fadeDuration = Mathf.Max(0f, duration);
            if (fadeDuration <= 0f)
            {
                StopIfCurrent(transitionVersion);
                return false;
            }

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                if (transitionVersion != _transitionVersion) return true;

                _fadeMultiplier = Mathf.Lerp(startMultiplier, 0f, Mathf.Clamp01(elapsed / fadeDuration));
                ApplyVolume();

                bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken)
                    .SuppressCancellationThrow();
                if (canceled)
                {
                    if (transitionVersion == _transitionVersion)
                    {
                        _fadeMultiplier = startMultiplier;
                        ApplyVolume();
                    }
                    return true;
                }

                if (transitionVersion != _transitionVersion) return true;
                elapsed += Time.unscaledDeltaTime;
            }

            StopIfCurrent(transitionVersion);
            return false;
        }

        private void StopIfCurrent(int transitionVersion)
        {
            if (transitionVersion != _transitionVersion) return;

            _fadeMultiplier = 0f;
            ApplyVolume();
            _usingInstruments = false;
            _allowDuos = false;
            _duoLevel = 0f;
            _duos?.Stop();
            _instruments?.Stop();
            _track.Stop();
            _track.clip = null;
        }
    }
}
