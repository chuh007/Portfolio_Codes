using System.Threading;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Visual;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class FallingNotesVisual
    {
        private readonly PianoBossHumanFallingNotesPatternSO _pattern;

        public FallingNotesVisual(PianoBossHumanFallingNotesPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayNoteFallAsync(
            Vector2 landingPosition,
            Sprite sprite,
            float duration,
            CancellationToken ct)
        {
            Vector2 startPosition = landingPosition + Vector2.up * _pattern.FallHeight;
            GameObject note = CreateFallingNote(startPosition, sprite);
            try
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    float t = Mathf.Clamp01(elapsed / duration);
                    SampleFall(note, startPosition, landingPosition, t);
                    elapsed += Time.deltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                SampleFall(note, startPosition, landingPosition, 1f);
            }
            finally
            {
                if (note != null)
                    ProjectilePool.Push(note);
            }
        }

        public GameObject CreateFallingNote(Vector2 position, Sprite sprite)
        {
            if (sprite == null)
                return null;

            GameObject note = ProjectilePool.Pop(
                "PianoHumanFallingNote",
                position,
                Quaternion.identity);
            if (note == null)
                return null;

            note.name = "PianoHumanFallingNote";
            note.transform.localScale = Vector3.one * _pattern.FallingNoteScale;

            SpriteRenderer renderer = ProjectilePool.GetOrAddComponent<SpriteRenderer>(note);
            if (renderer == null)
            {
                Debug.LogError("[PianoBossHumanFallingNotesPatternSO] 낙하 음표 SpriteRenderer를 생성하지 못했습니다.", note);
                ProjectilePool.Push(note);
                return null;
            }

            renderer.sprite = sprite;
            renderer.color = _pattern.FallingNoteColor;
            BossProjectileRenderLayer.ApplyTo(renderer, _pattern.VisualOrder);
            BossProjectileOutline.ApplyTo(renderer);
            return note;
        }

        public void SampleFall(GameObject note, Vector2 start, Vector2 end, float t)
        {
            if (note == null)
                return;

            float eased = t * t;
            note.transform.position = Vector2.Lerp(start, end, eased);
            note.transform.rotation = Quaternion.Euler(0f, 0f, _pattern.SpinDegrees * t);
        }
    }
}
