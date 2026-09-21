using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class FallingNotesLanding
    {
        private readonly PianoBossHumanFallingNotesPatternSO _pattern;

        public FallingNotesLanding(PianoBossHumanFallingNotesPatternSO pattern) => _pattern = pattern;

        public Vector2[] CreateLandingPositions(Enemy owner)
        {
            int count = Mathf.Max(1, _pattern.NoteCount);
            var positions = new Vector2[count];
            Vector2 center = owner != null ? owner.transform.position : Vector2.zero;
            Rect bounds = GetArenaBounds(center);
            float minRadius = Mathf.Min(_pattern.MinimumSpawnRadius, _pattern.MaximumSpawnRadius);
            float maxRadius = Mathf.Max(_pattern.MinimumSpawnRadius, _pattern.MaximumSpawnRadius);
            float angleStep = 360f / count;
            float startAngle = Random.Range(0f, 360f);

            for (int i = 0; i < count; i++)
            {
                float jitter = Random.Range(-angleStep, angleStep) * _pattern.AngularJitter;
                float angle = (startAngle + angleStep * i + jitter) * Mathf.Deg2Rad;
                float radius = Random.Range(minRadius, maxRadius);
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                positions[i] = ClampToBounds(center + offset, bounds, _pattern.ImpactRadius);
            }

            return positions;
        }

        public Rect GetArenaBounds(Vector2 fallbackCenter)
        {
            StageHelper helper = StageHelper.Instance;
            if (helper != null && helper.IsBossArenaActive)
            {
                Vector2 size = Vector2.Max(
                    Vector2.one,
                    helper.BossArenaSize - Vector2.one * (_pattern.ArenaPadding * 2f));
                return new Rect(helper.BossArenaCenter - size * 0.5f, size);
            }

            Vector2 fallbackSize = Vector2.Max(Vector2.one, _pattern.FallbackArenaSize);
            return new Rect(fallbackCenter - fallbackSize * 0.5f, fallbackSize);
        }

        public static Vector2 ClampToBounds(Vector2 position, Rect bounds, float padding)
        {
            float safePaddingX = Mathf.Min(Mathf.Max(0f, padding), bounds.width * 0.5f);
            float safePaddingY = Mathf.Min(Mathf.Max(0f, padding), bounds.height * 0.5f);
            return new Vector2(
                Mathf.Clamp(position.x, bounds.xMin + safePaddingX, bounds.xMax - safePaddingX),
                Mathf.Clamp(position.y, bounds.yMin + safePaddingY, bounds.yMax - safePaddingY));
        }

        public static PianoBossHumanFallingNotesContext GetOrAddContext(Enemy owner)
        {
            if (owner.TryGetComponent(out PianoBossHumanFallingNotesContext context))
                return context;

            context = owner.gameObject.AddComponent<PianoBossHumanFallingNotesContext>();
            context.hideFlags = HideFlags.HideInInspector;
            return context;
        }

        public static bool TryConsumeLandingPositions(Enemy owner, out Vector2[] positions)
        {
            positions = null;
            return owner != null
                   && owner.TryGetComponent(out PianoBossHumanFallingNotesContext context)
                   && context.TryConsume(out positions);
        }
    }
}
