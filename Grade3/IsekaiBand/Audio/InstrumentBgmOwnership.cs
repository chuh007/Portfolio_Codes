using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.Audio
{
    internal class InstrumentBgmOwnership
    {
        private static readonly WeaponType[] Instruments =
            { WeaponType.Vocal, WeaponType.Guitar, WeaponType.Bass, WeaponType.Keyboard, WeaponType.Drum };

        private PlayerAttackCompo _player;
        private WeaponBuildManager _buildManager;
        private float _nextPlayerSearchTime;

        public void SetPlayer(PlayerAttackCompo player)
        {
            if (_player == player) return;
            _player = player;
            _buildManager = null;
        }

        public int Read()
        {
            if (_player == null && Time.unscaledTime >= _nextPlayerSearchTime)
            {
                SetPlayer(Object.FindFirstObjectByType<PlayerAttackCompo>());
                _nextPlayerSearchTime = Time.unscaledTime + 0.5f;
            }
            if (_player == null) return 0;
            if (_buildManager == null)
                _buildManager = _player.GetComponent<WeaponBuildManager>();

            int owned = 0;
            foreach (WeaponType instrument in Instruments)
            {
                if (_player.HasWeapon(instrument))
                    owned |= GetInstrumentBit(instrument);
            }

            return owned;
        }

        public int GetCollectedPartCount(WeaponType instrument)
            => _buildManager != null
                ? Mathf.Clamp(_buildManager.GetCollectedPartCount(instrument), 0, InstrumentPartRules.RequiredPartCount)
                : 0;

        public static int GetInstrumentBit(WeaponType instrument)
        {
            return instrument switch
            {
                WeaponType.Vocal => 1 << 0,
                WeaponType.Guitar => 1 << 1,
                WeaponType.Bass => 1 << 2,
                WeaponType.Keyboard => 1 << 3,
                WeaponType.Drum => 1 << 4,
                _ => 0
            };
        }
    }
}
