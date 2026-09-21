using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.FullBand
{
    internal static class FullBandLightningVisual
    {
        private const string LightningPrefabPath = "LCH/RuntimePrefabs/ElectricLightningStrike";
        private const string LightningAnimationState = "ElectricLightningStrike";
        private static GameObject _lightningPrefab;

        public static void Spawn(Vector3 position, float radius, int strikeIndex)
        {
            GameObject prefab = _lightningPrefab != null
                ? _lightningPrefab
                : _lightningPrefab = Resources.Load<GameObject>(LightningPrefabPath);

            if (prefab != null)
            {
                GameObject obj = AttackVisualPool.Rent(prefab, "FullBandTargetedLightning", position, Quaternion.identity);
                obj.name = "FullBandTargetedLightning";
                obj.transform.localScale = Vector3.one
                                           * Mathf.Max(1.15f, radius / 0.65f);
                obj.GetComponent<PooledAttackVisual>().PlayAnimation(LightningAnimationState, 0.4f);
            }

            Color color = strikeIndex % 2 == 0
                ? new Color(0.34f, 0.9f, 1f, 0.9f)
                : new Color(1f, 0.9f, 0.24f, 0.9f);
            BuildVisualEffect.SpawnLine(
                position + Vector3.up * 4.2f,
                position,
                color,
                0.22f,
                0.2f,
                64);
            BuildVisualEffect.SpawnCircle(
                position,
                radius,
                color,
                0.28f,
                63,
                true);
        }
    }
}
