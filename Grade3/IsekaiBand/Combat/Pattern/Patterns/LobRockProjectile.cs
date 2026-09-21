using System.Threading;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Visual;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LobRockProjectile
    {
        private readonly MiddleBossLobRockPatternSO _pattern;

        public LobRockProjectile(MiddleBossLobRockPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayRockArcAsync(Vector2 launchPosition, Vector2 landingPosition, CancellationToken ct)
        {
            GameObject rock = CreateRockObject(launchPosition);
            try
            {
                float duration = Mathf.Max(0.01f, _pattern.TravelDuration);
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    float t = Mathf.Clamp01(elapsed / duration);
                    SampleRockArc(rock, launchPosition, landingPosition, t);

                    elapsed += Time.deltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                SampleRockArc(rock, launchPosition, landingPosition, 1f);
            }
            finally
            {
                if (rock != null)
                    ProjectilePool.Push(rock);
            }
        }

        public GameObject CreateRockObject(Vector2 position)
        {
            if (_pattern.RockPrefab == null && _pattern.RockSprite == null)
                return null;

            GameObject rock = ProjectilePool.Pop(
                "MiddleBossLobRock",
                position,
                Quaternion.identity);
            if (rock == null)
                return null;

            rock.name = "MiddleBossLobRock";
            ApplyRockPresentation(rock);
            return rock;
        }

        public void ApplyRockPresentation(GameObject rock)
        {
            if (rock == null)
                return;

            rock.transform.localScale = Vector3.one * _pattern.RockScale;

            SpriteRenderer[] renderers = rock.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer renderer = renderers[i];
                if (renderer == null)
                    continue;

                if (renderer.sprite == null)
                    renderer.sprite = _pattern.RockSprite;

                ApplyRockRendererPresentation(renderer);
            }
        }

        public void ApplyRockRendererPresentation(SpriteRenderer renderer)
        {
            if (renderer == null)
                return;

            renderer.color = _pattern.RockColor;
            BossProjectileRenderLayer.ApplyTo(renderer, _pattern.RockSortingOrder);
            BossProjectileOutline.ApplyTo(renderer);
        }

        public void SampleRockArc(GameObject rock, Vector2 launchPosition, Vector2 landingPosition, float t)
        {
            if (rock == null)
                return;

            Vector3 position = Vector2.Lerp(launchPosition, landingPosition, t);
            position.y += Mathf.Sin(t * Mathf.PI) * _pattern.ArcHeight;
            rock.transform.position = position;
            rock.transform.rotation = Quaternion.Euler(0f, 0f, _pattern.SpinDegrees * t);
        }
    }
}
