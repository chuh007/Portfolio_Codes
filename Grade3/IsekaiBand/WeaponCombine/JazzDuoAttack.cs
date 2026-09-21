using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class JazzDuoAttack : CombineWeaponBase
    {
        private const string FairyPrefabPath = "LCH/RuntimePrefabs/PianoFairyBasic";
        private static GameObject _fairyPrefab;
        private readonly JazzDuoStats _stats = new();
        private readonly JazzDuoZones _zones;
        private readonly JazzDuoNotes _notes;
        private GameObject _fairy;

        public override CombineWeaponType CombinationType => CombineWeaponType.JazzDuo;
        public override WeaponType PrimaryWeaponType => WeaponType.Keyboard;


        public JazzDuoAttack()
        {
            _zones = new JazzDuoZones(this, _stats,
                (hit, position, amount, radius) => DamageEnemy(hit, position, amount, radius));
            _notes = new JazzDuoNotes(this, _stats, _zones);
        }

        public override void Init(PlayerBasicAttackDataSo data, Transform ownerTransform,
            System.Func<Vector3, Quaternion, GameObject> spawner)
        {
            base.Init(data, ownerTransform, spawner);
            _zones.Init();
            CreateFairy();
        }

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => _stats.Configure(ingredients, this);

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (_fairy != null)
                _fairy.transform.position = OwnerPosition + Vector3.up * 1.35f;
            _zones.Tick();
        }

        public override void Dispose()
        {
            _zones.Stop();
            if (_fairy != null)
                Object.Destroy(_fairy);
            _zones.Clear();
            base.Dispose();
        }

        protected override void OnAttack()
            => _notes.Fire(_fairy != null ? _fairy.transform.position : OwnerPosition);

        internal void DamageZone(Vector3 position, float radius) => _zones.DamageZone(position, radius);

        private void CreateFairy()
        {
            GameObject prefab = _fairyPrefab != null ? _fairyPrefab : _fairyPrefab = Resources.Load<GameObject>(FairyPrefabPath);
            _fairy = prefab != null
                ? Object.Instantiate(prefab, OwnerPosition + Vector3.up * 1.35f, Quaternion.identity)
                : new GameObject("JazzPianoFairy");
            _fairy.name = "JazzPianoFairy";
            _fairy.transform.localScale = Vector3.one * 0.13f;
        }
    }
}
