using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Audio;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class OrthodoxRockLightning
    {
        private const string LightningPrefabPath = "LCH/RuntimePrefabs/ElectricLightningStrike";
        private const string LightningAnimationState = "ElectricLightningStrike";
        private static GameObject _lightningPrefab;
        private readonly OrthodoxRockBandAttack _source;
        private readonly OrthodoxRockStats _stats;
        private readonly Collider2D[] _hits;
        private readonly System.Action<Collider2D, Vector3, float, float> _damageEnemy;

        public OrthodoxRockLightning(OrthodoxRockBandAttack source, OrthodoxRockStats stats,
            Collider2D[] hits, System.Action<Collider2D, Vector3, float, float> damageEnemy)
        {
            _source = source;
            _stats = stats;
            _hits = hits;
            _damageEnemy = damageEnemy;
        }

        public void StrikeLightning()
        {
            _source.AttackAudio.Play(SoundKeys.ElectricProjectileDischarge);
            Vector2 offset = Random.insideUnitCircle * (_source.ScaleCommonRange(_stats.VenueRadius) * 0.82f);
            Vector3 position = _source.Position + (Vector3)offset;
            int count = Physics2D.OverlapCircle(position, _source.ScaleCommonRange(_stats.LightningRadius), _source.TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy")) continue;
                _damageEnemy(hit, position, _stats.LightningDamage, _source.ScaleCommonRange(_stats.LightningRadius));
            }

            SpawnLightningVisual(position);
            BuildVisualEffect.SpawnLine(
                position + Vector3.up * 3.2f, position,
                new Color(0.55f, 0.9f, 1f, 1f), 0.18f, 0.18f, 55);
            BuildVisualEffect.SpawnCircle(
                position, _source.ScaleCommonRange(_stats.LightningRadius), new Color(0.35f, 0.75f, 1f, 0.58f), 0.24f, 50, true);
        }

        private void SpawnLightningVisual(Vector3 position)
        {
            GameObject prefab = _lightningPrefab != null
                ? _lightningPrefab
                : _lightningPrefab = Resources.Load<GameObject>(LightningPrefabPath);
            if (prefab == null) return;

            GameObject obj = AttackVisualPool.Rent(prefab, "ElectricLightningStrike", position, Quaternion.identity);
            obj.name = "ElectricLightningStrike";
            obj.transform.localScale = Vector3.one * Mathf.Max(1f, _source.ScaleCommonRange(_stats.LightningRadius) / 0.72f);

            obj.GetComponent<PooledAttackVisual>().PlayAnimation(LightningAnimationState, 0.34f);
        }
    }
}
