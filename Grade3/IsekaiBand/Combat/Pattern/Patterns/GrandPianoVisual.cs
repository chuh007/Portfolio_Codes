using System.Threading;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Visual;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class GrandPianoVisual
    {
        private readonly PianoBossHumanGrandPianoDropPatternSO _pattern;

        public GrandPianoVisual(PianoBossHumanGrandPianoDropPatternSO pattern) => _pattern = pattern;

        public GameObject CreateGrandPiano(Vector2 position)
        {
            if (_pattern.GrandPianoPrefab == null && _pattern.GrandPianoSprite == null)
                return null;

            GameObject root = ProjectilePool.Pop(
                "PianoHumanFallingGrandPiano",
                position,
                Quaternion.identity);
            if (root == null)
                return null;

            root.name = "PianoHumanFallingGrandPiano";
            root.transform.localScale = Vector3.one * _pattern.PianoScale;

            GameObject visual = root.transform.childCount > 0
                ? root.transform.GetChild(0).gameObject
                : root;
            visual.name = "GrandPianoVisual";
            if (visual != root)
                visual.transform.localPosition = Vector3.zero;

            SpriteRenderer renderer = visual.GetComponentInChildren<SpriteRenderer>(true);
            if (renderer == null)
                renderer = visual.AddComponent<SpriteRenderer>();
            if (renderer.sprite == null)
                renderer.sprite = _pattern.GrandPianoSprite;

            if (renderer.sprite != null)
            {
                Bounds spriteBounds = renderer.sprite.bounds;
                visual.transform.localPosition = -spriteBounds.center
                                                 + Vector3.up * (spriteBounds.size.y * _pattern.VisualLiftRatio);
            }

            SpriteRenderer[] renderers = visual.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer childRenderer in renderers)
            {
                childRenderer.color = _pattern.PianoColor;
                BossProjectileRenderLayer.ApplyTo(childRenderer, _pattern.VisualOrder);
                BossProjectileOutline.ApplyTo(childRenderer);
            }

            return root;
        }

        public void SampleFall(GameObject piano, Vector2 start, Vector2 end, float normalizedTime)
        {
            if (piano == null)
                return;

            float easedTime = normalizedTime * normalizedTime;
            piano.transform.position = Vector2.LerpUnclamped(start, end, easedTime);
            piano.transform.rotation = Quaternion.Euler(
                0f,
                0f,
                Mathf.Lerp(_pattern.StartRotation, 0f, easedTime));
        }

        public async UniTask FadeOutAsync(GameObject piano, CancellationToken ct)
        {
            if (piano == null || _pattern.FadeOutDuration <= 0f)
                return;

            SpriteRenderer[] renderers = piano.GetComponentsInChildren<SpriteRenderer>(true);
            if (renderers.Length == 0)
                return;

            var originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                originalColors[i] = renderers[i].color;

            float elapsed = 0f;
            while (elapsed < _pattern.FadeOutDuration)
            {
                float alphaRatio = 1f - Mathf.Clamp01(elapsed / _pattern.FadeOutDuration);
                ApplyAlpha(renderers, originalColors, alphaRatio);
                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            ApplyAlpha(renderers, originalColors, 0f);
        }

        public static void ApplyAlpha(
            SpriteRenderer[] renderers,
            Color[] originalColors,
            float alphaRatio)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null)
                    continue;

                Color color = originalColors[i];
                color.a *= alphaRatio;
                renderers[i].color = color;
            }
        }
    }
}
