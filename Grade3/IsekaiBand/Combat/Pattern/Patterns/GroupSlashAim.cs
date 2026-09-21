using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class GroupSlashAim
    {
        private readonly GroupSlashPressurePatternSO _pattern;

        public GroupSlashAim(GroupSlashPressurePatternSO pattern) => _pattern = pattern;

        public Vector2 GetTargetSlashPosition(Enemy owner)
        {
            Vector2 basePosition = owner.target != null
                ? owner.target.transform.position
                : owner.transform.position;

            Vector2 position = basePosition + PickAimOffset();
            StageHelper stageHelper = StageHelper.Instance;
            return stageHelper != null ? stageHelper.ClampToMapBound(position, _pattern.MapPadding) : position;
        }

        public Vector2 PickAimOffset()
        {
            float min = Mathf.Min(_pattern.AimOffsetMin, _pattern.AimOffsetMax);
            float max = Mathf.Max(_pattern.AimOffsetMin, _pattern.AimOffsetMax);
            if (max <= Mathf.Epsilon)
                return Vector2.zero;

            float radius = Random.Range(min, max);
            float angle = Random.Range(0f, Mathf.PI * 2f);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        public static Vector2 GetDirection(Vector2 from, Vector2 to)
        {
            Vector2 direction = to - from;
            return NormalizeDirection(direction);
        }

        public static Vector2 NormalizeDirection(Vector2 direction)
        {
            return direction.sqrMagnitude > Mathf.Epsilon ? direction.normalized : Vector2.right;
        }
    }
}
