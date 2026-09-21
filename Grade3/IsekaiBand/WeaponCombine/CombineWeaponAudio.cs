using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class CombineWeaponAudio
    {
        private readonly object _vocalPlaybackKey = new();
        private float _vocalTimeRemaining;
        private bool _vocalPlaying;

        public void Play(string soundKey)
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                soundKey,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));
        }

        public void PlayVocal(float duration = float.PositiveInfinity)
        {
            if (duration <= 0f) return;

            _vocalTimeRemaining = Mathf.Max(_vocalTimeRemaining, duration);
            if (_vocalPlaying) return;

            _vocalPlaying = true;
            Bus<SoundLoopStartEvent>.Raise(new SoundLoopStartEvent(
                _vocalPlaybackKey,
                SoundKeys.VocalAttackLoop));
        }

        public void Tick(float deltaTime)
        {
            if (!_vocalPlaying) return;

            _vocalTimeRemaining -= deltaTime;
            if (_vocalTimeRemaining <= 0f)
                StopVocal();
        }

        public void StopVocal()
        {
            _vocalTimeRemaining = 0f;
            if (!_vocalPlaying) return;

            _vocalPlaying = false;
            Bus<SoundLoopStopEvent>.Raise(new SoundLoopStopEvent(_vocalPlaybackKey));
        }
    }
}
