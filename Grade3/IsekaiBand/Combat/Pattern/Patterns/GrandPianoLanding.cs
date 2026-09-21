using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class GrandPianoLanding
    {
        private readonly PianoBossHumanGrandPianoDropPatternSO _pattern;

        public GrandPianoLanding(PianoBossHumanGrandPianoDropPatternSO pattern) => _pattern = pattern;

        public Vector2 ResolveLandingPosition(Enemy owner)
        {
            Vector2 position = owner != null && owner.target != null
                ? owner.target.transform.position
                : owner != null
                    ? owner.transform.position
                    : Vector2.zero;

            StageHelper stageHelper = StageHelper.Instance;
            if (stageHelper == null || !stageHelper.IsBossArenaActive)
                return position;

            Vector2 halfSize = stageHelper.BossArenaSize * 0.5f;
            float padding = Mathf.Max(_pattern.ImpactRadius, _pattern.ArenaPadding);
            Vector2 minimum = stageHelper.BossArenaCenter - halfSize + Vector2.one * padding;
            Vector2 maximum = stageHelper.BossArenaCenter + halfSize - Vector2.one * padding;
            if (minimum.x > maximum.x || minimum.y > maximum.y)
                return stageHelper.BossArenaCenter;

            return new Vector2(
                Mathf.Clamp(position.x, minimum.x, maximum.x),
                Mathf.Clamp(position.y, minimum.y, maximum.y));
        }

        public static PianoBossHumanGrandPianoDropContext GetOrAddContext(Enemy owner)
        {
            if (owner.TryGetComponent(out PianoBossHumanGrandPianoDropContext context))
                return context;

            context = owner.gameObject.AddComponent<PianoBossHumanGrandPianoDropContext>();
            context.hideFlags = HideFlags.HideInInspector;
            return context;
        }

        public static bool TryConsumeLandingPosition(Enemy owner, out Vector2 position)
        {
            position = default;
            return owner != null
                   && owner.TryGetComponent(out PianoBossHumanGrandPianoDropContext context)
                   && context.TryConsume(out position);
        }
    }
}
