using _Code.LCH._02.Scripts.Bus;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;

namespace _Code.LCH._02.Scripts.Level
{
    public class LevelSystem
    {
        private readonly LevelDataSO _data;

        private int   _currentLevel = 1;
        private float _currentExp   = 0f;
        private float _requiredExp;

        public int   CurrentLevel => _currentLevel;
        public float CurrentExp   => _currentExp;
        public float RequiredExp  => _requiredExp;
        public float ExpRatio     => _requiredExp > 0f ? _currentExp / _requiredExp : 0f;

        public LevelSystem(LevelDataSO data)
        {
            _data        = data;
            _requiredExp = _data.GetRequiredExp(_currentLevel);
        }

        public void AddExp(float amount)
        {
    
            if (_currentLevel >= _data.maxLevel) return;

            _currentExp += amount;
            Bus<ExpChangeEvent>.Raise(new ExpChangeEvent(ExpRatio));

            while (_currentExp >= _requiredExp && _currentLevel < _data.maxLevel)
            {
                _currentExp  -= _requiredExp;
                _currentLevel++;
                _requiredExp  = _data.GetRequiredExp(_currentLevel);
                Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(SoundKeys.PlayerLevelUp, SoundType.SFX));
                Bus<LevelUpEvent>.Raise(new LevelUpEvent(_currentLevel));
                Bus<ExpChangeEvent>.Raise(new ExpChangeEvent(ExpRatio));
            }
        }
    }
}
