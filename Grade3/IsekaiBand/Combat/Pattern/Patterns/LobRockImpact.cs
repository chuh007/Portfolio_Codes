using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Bus;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LobRockImpact
    {
        private readonly MiddleBossLobRockPatternSO _pattern;
        private readonly Collider2D[] _colliders = new Collider2D[16];
        public LobRockImpact(MiddleBossLobRockPatternSO pattern) => _pattern = pattern;

        public void PlayImpactSound()
        {
            if (string.IsNullOrWhiteSpace(_pattern.ImpactSoundKey))
                return;

            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                _pattern.ImpactSoundKey,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));
        }

        public async UniTask PlayImpactEffectAsync(Vector2 position, CancellationToken ct)
        {
            if (_pattern.ImpactSprites == null || _pattern.ImpactSprites.Length == 0)
                return;

            GameObject effect = CreateImpactEffectObject(position, out SpriteRenderer renderer);
            try
            {
                for (int i = 0; i < _pattern.ImpactSprites.Length; i++)
                {
                    if (ct.IsCancellationRequested)
                        return;

                    Sprite sprite = _pattern.ImpactSprites[i];
                    if (sprite != null)
                    {
                        renderer.sprite = sprite;
                        renderer.transform.localPosition = -sprite.bounds.center;
                    }

                    bool canceled = await UniTask
                        .WaitForSeconds(_pattern.ImpactFrameInterval, cancellationToken: ct)
                        .SuppressCancellationThrow();
                    if (canceled)
                        return;
                }
            }
            finally
            {
                if (effect != null)
                    UnityEngine.Object.Destroy(effect);
            }
        }

        public GameObject CreateImpactEffectObject(Vector2 position, out SpriteRenderer renderer)
        {
            var root = new GameObject("MiddleBossRockImpact");
            root.transform.position = position;
            root.transform.localScale = Vector3.one * _pattern.ImpactEffectScale;

            var spriteObject = new GameObject("Sprite");
            spriteObject.transform.SetParent(root.transform, false);
            renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = _pattern.ImpactSortingOrder;
            if (!string.IsNullOrWhiteSpace(_pattern.ImpactSortingLayerName))
                renderer.sortingLayerName = _pattern.ImpactSortingLayerName;

            return root;
        }

        public void DamageTargets(Enemy owner, Vector2 landingPosition, DamageData damage)
        {
            if (_pattern.ImpactRadius <= 0f)
                return;

            int count = Physics2D.OverlapCircle(landingPosition, _pattern.ImpactRadius, _pattern.WhatIsTarget, _colliders);
            Vector2 direction = ((Vector2)landingPosition - (Vector2)owner.transform.position).normalized;

            for (int i = 0; i < count; i++)
            {
                Collider2D hitCollider = _colliders[i];
                if (hitCollider == null)
                    continue;

                IDamageable damageable = GetDamageable(hitCollider);
                damageable?.TakeDamage(damage, direction, owner);
            }
        }

        public static IDamageable GetDamageable(Collider2D hitCollider)
        {
            if (hitCollider.TryGetComponent(out IDamageable damageable))
                return damageable;

            return hitCollider.GetComponentInParent<IDamageable>();
        }
    }
}
