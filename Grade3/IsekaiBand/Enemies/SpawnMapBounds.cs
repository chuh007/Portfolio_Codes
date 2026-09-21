using _Work.CHUH.Code.StageSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Enemies
{
    internal static class SpawnMapBounds
    {
        public static Vector2 ClampToMapBounds(SpawnerPositionContext context, Vector2 pos, float padding)
        {
            StageHelper helper = StageHelper.Instance;
            if (helper != null && helper.IsBossArenaActive)
                return helper.ClampToMapBound(pos, padding);

            if (context.IsInfiniteMap || !context.HasMapBounds)
                return pos;

            float maxX = Mathf.Max(0f, context.MapHalfWidth - padding);
            float maxY = Mathf.Max(0f, context.MapHalfHeight - padding);

            return new Vector2(
                Mathf.Clamp(pos.x, -maxX, maxX),
                Mathf.Clamp(pos.y, -maxY, maxY));
        }

        public static bool IsInMapBounds(SpawnerPositionContext context, Vector2 pos, float padding)
        {
            StageHelper helper = StageHelper.Instance;
            if (helper != null && helper.IsBossArenaActive)
                return helper.CheckMapBound(pos, padding);

            if (context.IsInfiniteMap)
                return true;

            float maxX = Mathf.Max(0f, context.MapHalfWidth - padding);
            float maxY = Mathf.Max(0f, context.MapHalfHeight - padding);

            return pos.x >= -maxX && pos.x <= maxX
                   && pos.y >= -maxY && pos.y <= maxY;
        }
    }
}
