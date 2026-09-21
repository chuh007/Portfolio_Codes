using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerAttackCompo))]
    public class CombineWeaponController : MonoBehaviour
    {
        [SerializeField] private List<CombineMakeDataSO> combineWeapons = new();
        
        private WeaponBuildManager _buildManager;
        private PlayerAttackCompo _attackCompo;
        private readonly Dictionary<CombineWeaponType, CombineWeaponBase> _activeWeapons = new();
        private readonly List<CombineWeaponType> _creationOrder = new();

        public IReadOnlyList<CombineWeaponType> CreationOrder => _creationOrder;

        private void Awake()
        {
            _buildManager = GetComponent<WeaponBuildManager>();
            _attackCompo = GetComponent<PlayerAttackCompo>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (var weapon in _activeWeapons.Values)
            {
                weapon.AttackAudio.Tick(deltaTime);
                weapon.Tick(deltaTime);
            }
        }

        private void OnDisable()
        {
            foreach (var weapon in _activeWeapons.Values)
                weapon.AttackAudio.StopVocal();
        }

        private void OnDestroy()
        {
            foreach (var weapon in _activeWeapons.Values)
                weapon.Dispose();
            _activeWeapons.Clear();
            _creationOrder.Clear();
        }

        public bool CanCombine(CombineMakeDataSO combineData)
        {
            if (combineData == null || _activeWeapons.ContainsKey(combineData.combineWeapon))
                return false;

            EnsureComponents();
            if (_buildManager == null || _attackCompo == null || combineData.needWeapons == null
                || !CombineWeaponFactory.MatchesRecipe(combineData.combineWeapon, combineData.needWeapons))
                return false;

            var uniqueWeapons = new HashSet<WeaponType>();
            foreach (var type in combineData.needWeapons)
            {
                if (type == WeaponType.None || !uniqueWeapons.Add(type)
                    || !_buildManager.IsInstrumentComplete(type)
                    || !_attackCompo.HasWeapon(type))
                    return false;
            }
            return true;
        }

        public bool TryCombine(CombineMakeDataSO combineData)
        {
            if (!CanCombine(combineData)) return false;

            CombineWeaponBase combinedWeapon = CombineWeaponFactory.Create(combineData.combineWeapon);
            if (combinedWeapon == null) return false;

            var ingredientAttacks = new Dictionary<WeaponType, PlayerAttackBase>();
            foreach (WeaponType type in combineData.needWeapons)
                ingredientAttacks[type] = _attackCompo.GetAttack(type);

            var sourceData = _attackCompo.GetWeaponData(combinedWeapon.PrimaryWeaponType);
            if (sourceData?.basicAttackData == null) return false;

            combinedWeapon.Init(sourceData.basicAttackData, transform, null);
            combinedWeapon.ConfigureIngredients(ingredientAttacks);

            if (!_attackCompo.RemoveWeapons(combineData.needWeapons))
            {
                combinedWeapon.Dispose();
                return false;
            }

            _buildManager.ConsumeInstruments(combineData.needWeapons);
            _activeWeapons.Add(combineData.combineWeapon, combinedWeapon);
            _creationOrder.Add(combineData.combineWeapon);
            GetComponent<PlayerCommonBuildCompo>()?.RefreshAttackModifiers();

            if (combineData.combineWeapon == CombineWeaponType.FullBand)
            {
                Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                    SoundKeys.FullBandBgm,
                    SoundType.BGM));
            }

            return true;
        }

        public bool HasCombinedWeapon(CombineWeaponType type)
            => _activeWeapons.ContainsKey(type);

        public IEnumerable<CombineWeaponBase> GetActiveWeapons() => _activeWeapons.Values;

        public bool TryCombine(CombineWeaponType type)
        {
            foreach (CombineMakeDataSO combineData in combineWeapons)
            {
                if (combineData != null && combineData.combineWeapon == type)
                    return TryCombine(combineData);
            }
            return false;
        }

        public List<CombineMakeDataSO> GetAvailableCombinations()
        {
            var available = new List<CombineMakeDataSO>();
            foreach (CombineMakeDataSO combineData in combineWeapons)
            {
                if (CanCombine(combineData))
                    available.Add(combineData);
            }
            return available;
        }

        public IReadOnlyList<CombineMakeDataSO> GetAllCombinations()
            => combineWeapons;

        private void EnsureComponents()
        {
            if (_attackCompo == null)
                _attackCompo = GetComponent<PlayerAttackCompo>();
            if (_buildManager == null && _attackCompo != null)
                _buildManager = WeaponBuildManager.GetOrCreate(_attackCompo);
        }
    }
}
