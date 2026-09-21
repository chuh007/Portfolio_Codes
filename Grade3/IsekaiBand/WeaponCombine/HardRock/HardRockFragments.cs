using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class HardRockFragments
    {
        private const int FragmentCount = 8;
        private readonly HardRockBandAttack _source;
        private readonly HardRockStats _stats;
        private readonly BandRuntimeObjects _runtimeObjects;

        public HardRockFragments(HardRockBandAttack source, HardRockStats stats, BandRuntimeObjects runtimeObjects)
        {
            _source = source;
            _stats = stats;
            _runtimeObjects = runtimeObjects;
        }

        public void Spawn(Vector3 position)
        {
            BuildVisualEffect.SpawnCircle(
                position, 1.35f, new Color(1f, 0.32f, 0.12f, 0.68f), 0.3f, 52, true);
            BandRuntimeVisuals.SpawnSignatureBurst(
                position,
                2.1f,
                new Color(1f, 0.26f, 0.08f, 0.7f),
                new Color(0.32f, 0.82f, 1f, 0.72f),
                FragmentCount,
                22.5f,
                0.34f,
                53);

            float startAngle = Random.Range(0f, 60f);
            int fragmentCount = _source.CountProjectiles(FragmentCount);
            for (int i = 0; i < fragmentCount; i++)
            {
                float angle = startAngle + i * (360f / fragmentCount);
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                SpawnFragment(position, direction);
            }
        }

        private void SpawnFragment(Vector3 position, Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject obj = ProjectilePool.Pop(
                "HardRockElectricFragment",
                position,
                Quaternion.Euler(0f, 0f, angle));
            if (obj == null)
                return;

            obj.name = "HardRockElectricFragment";
            obj.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, angle));
            obj.transform.localScale = Vector3.one * 0.62f;

            SpriteRenderer renderer = obj.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = new Color(0.55f, 0.9f, 1f, 0.95f);
                ProjectileRenderLayer.ApplyTo(renderer, 53);
            }
            ProjectileRenderLayer.ApplyTo(obj);

            var fragment = obj.GetComponent<HardRockElectricFragment>() ?? obj.AddComponent<HardRockElectricFragment>();
            fragment.Init(_source, direction, _stats.FragmentSpeed, _source.ScaleCommonRange(_stats.FragmentRange));
            _runtimeObjects.Add(obj);
        }
    }
}
