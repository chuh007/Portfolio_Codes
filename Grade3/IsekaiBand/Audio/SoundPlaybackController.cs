using System.Collections.Generic;
using _Code.LCH._02.Scripts.Bus;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class SoundPlaybackController
    {
        private const string CatalogResourcePath = "Audio/SoundCatalog";
        private readonly HashSet<string> _missingSoundKeys = new();
        private readonly SoundCatalogSO _catalog;
        private readonly OneShotSfxPlayer _oneShotSfx;
        private readonly LoopingSfxPlayer _loopingSfx;

        public SoundVolumeController Volumes { get; }
        public BgmPlayer Bgm { get; }

        public SoundPlaybackController(Transform owner)
        {
            _catalog = Resources.Load<SoundCatalogSO>(CatalogResourcePath);
            if (_catalog == null)
                Debug.LogError($"[SoundManager] Resources/{CatalogResourcePath} 사운드 카탈로그를 찾을 수 없습니다.");

            var tracks = new SoundTrackFactory(owner);
            Volumes = new SoundVolumeController(_catalog);
            _oneShotSfx = new OneShotSfxPlayer(tracks, _catalog, Volumes);
            Bgm = new BgmPlayer(tracks, _catalog, Volumes,
                Resources.Load<InstrumentBgmSO>("Audio/InstrumentBgm"),
                Resources.Load<DuoBgmSO>("Audio/DuoBgm"));
            _loopingSfx = new LoopingSfxPlayer(tracks, _catalog, Volumes);
            if (_catalog != null)
                _catalog.PreloadMarkedClips();
            Volumes.OnVolumeChanged += ApplyVolumes;
            Volumes.Apply();
        }

        public void Subscribe()
        {
            Bus<SoundPlayEvent>.OnEvent += HandleSoundPlay;
            Bus<SoundLoopStartEvent>.OnEvent += HandleSoundLoopStart;
            Bus<SoundLoopStopEvent>.OnEvent += HandleSoundLoopStop;
            Bus<WeaponSlotsChangedEvent>.OnEvent += HandleWeaponSlotsChanged;
        }

        public void Unsubscribe()
        {
            Bus<SoundPlayEvent>.OnEvent -= HandleSoundPlay;
            Bus<SoundLoopStartEvent>.OnEvent -= HandleSoundLoopStart;
            Bus<SoundLoopStopEvent>.OnEvent -= HandleSoundLoopStop;
            Bus<WeaponSlotsChangedEvent>.OnEvent -= HandleWeaponSlotsChanged;
            _loopingSfx.StopAll();
        }

        private void HandleSoundPlay(SoundPlayEvent evt)
        {
            if (!TryGetSound(evt.SoundKey, out SoundCatalogEntry entry)) return;

            if (evt.SoundType == SoundType.BGM)
                Bgm.Play(entry);
            else
                _oneShotSfx.Play(entry, evt);
        }

        private void HandleSoundLoopStart(SoundLoopStartEvent evt)
        {
            if (evt.PlaybackKey == null)
            {
                Debug.LogWarning("[SoundManager] 반복 사운드의 재생 키가 비어 있습니다.");
                return;
            }

            if (TryGetSound(evt.SoundKey, out SoundCatalogEntry entry))
                _loopingSfx.Play(entry, evt);
        }

        private void HandleSoundLoopStop(SoundLoopStopEvent evt)
        {
            if (evt.PlaybackKey != null)
                _loopingSfx.Stop(evt.PlaybackKey);
        }

        private void HandleWeaponSlotsChanged(WeaponSlotsChangedEvent evt) => Bgm.SetPlayer(evt.AttackCompo);

        private bool TryGetSound(string soundKey, out SoundCatalogEntry entry)
        {
            entry = default;
            if (_catalog != null && _catalog.TryGet(soundKey, out entry)) return true;

            string safeKey = string.IsNullOrWhiteSpace(soundKey) ? "<empty>" : soundKey;
            if (_missingSoundKeys.Add(safeKey))
                Debug.LogWarning($"[SoundManager] '{safeKey}' 사운드 키가 카탈로그에 없습니다.");
            return false;
        }

        private void ApplyVolumes()
        {
            _oneShotSfx.ApplyVolume();
            _loopingSfx.ApplyVolume();
            Bgm.ApplyVolume();
        }
    }
}
